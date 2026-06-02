using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Interfaces;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace BeatCharacterMod.BeatCharacterModCode.Patches;

[HarmonyPatch]
public class PatchTempoCost
{
    [HarmonyPatch(typeof(PlayerCombatState), "HasEnoughResourcesFor")]
    [HarmonyPrefix]
    public static bool HasEnoughResourcesPrefix(PlayerCombatState __instance, CardModel card, ref bool __result,
        ref UnplayableReason reason, Player ____player)
    {
        int energyCost = Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
        int starCost = Math.Max(0, card.GetStarCostWithModifiers());
        if (energyCost > __instance.Energy && card.CombatState != null && Hook.ShouldPayExcessEnergyCostWithStars(card.CombatState, ____player))
        {
            starCost += (energyCost - __instance.Energy) * 2;
            energyCost = __instance.Energy;
        }
        
        int tempoCost = 0;
        
        if (card is ITempoCostCard { Tempo: >0 } beatCard)
        {
            tempoCost = Math.Max(0, beatCard.GetTempoCostWithModifiers());
        }
        
        if (energyCost > 0 && !card.EnergyCost.CostsX
                           && MelodicFlowTracker.GetMelodicFlowState(____player) == MelodicState.Silence)
        {
            energyCost -= 1;
            tempoCost += 1;
        }
        
        reason = UnplayableReason.None;
        if (energyCost > __instance.Energy)
            reason |= UnplayableReason.EnergyCostTooHigh;
        if (starCost > __instance.Stars)
            reason |= UnplayableReason.StarCostTooHigh;
        if (tempoCost > MelodicFlowTracker.GetTempo(____player))
            reason |= UnplayableReason.StarCostTooHigh;
        __result = reason == UnplayableReason.None;

        // Skip everything else if we have a reason to cost Tempo
        return tempoCost == 0;
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendEnergy))]
    [HarmonyPrefix]
    public static void SpendEnergyPrefix(ref int amount, CardModel __instance, out int __state)
    {
        // Tempo cost
        __state = 0;
        
        int energyToSpend = __instance.EnergyCost.GetAmountToSpend();
        
        if (__instance is ITempoCostCard { Tempo: >0 } beatCard)
        {
            __state = Math.Max(0, beatCard.GetTempoCostWithModifiers());
        }
        
        if (amount > 0 && !__instance.EnergyCost.CostsX
                       && __instance.Owner.PlayerCombatState?.MelodicFlow().MelodicState == MelodicState.Silence)
        {
            amount -= 1;
            __state += 1;
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendEnergy))]
    [HarmonyPostfix]
    public static async Task<(int, int)> SpendEnergyPostfix(Task<(int, int)> results, CardModel __instance, int __state)
    {
        var ret = await results;

        if (__state > 0)
        {
            // TODO this should be the card's SpendTempo function call
            __instance.Owner.PlayerCombatState?.MelodicFlow().LoseTempo(__state);
            // MelodicFlowTracker.LoseTempo(__instance.Owner, __state);
        }
        return ret;
    }
}