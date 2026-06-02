using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Fields;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;

namespace BeatCharacterMod.BeatCharacterModCode.Patches;

[HarmonyPatch]
public class PatchPlayerCombatState
{
    [HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
    [HarmonyPatch([typeof(Player)])]
    [HarmonyPostfix]
    static void ConstructorPostfix(Player player, PlayerCombatState __instance)
    {
        var combatState = new PlayerCombatStateExtensions.MelodicFlowCombatState(__instance);
        MelodicFlowFields.CombatState[__instance] = combatState;
        CombatManager.Instance.StateTracker.SubscribeMelodicFlow(combatState);
        MainFile.Logger.Info("Initialized MelodicFlowCombatState for " + player);
    }

    [HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.AfterCombatEnd))]
    [HarmonyPostfix]
    static void AfterCombatEndPostfix(PlayerCombatState __instance)
    {
        var combatState = __instance.MelodicFlow();
        if (combatState != null)
        {
            CombatManager.Instance.StateTracker.UnsubscribeMelodicFlow(combatState);
        }
    }

}