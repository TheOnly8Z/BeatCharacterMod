using BaseLib.Utils.Patching;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Interfaces;
using BeatCharacterMod.BeatCharacterModCode.Nodes;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace BeatCharacterMod.BeatCharacterModCode.Patches;


[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
internal class PatchNCardUpdateVisuals
{
    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldarg_0()
            .ldarg_1()
            .call(typeof(NCard), nameof(NCard.UpdateStarCostVisuals), [typeof(PileType)])
        ).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadArgument(1),
            CodeInstruction.Call(typeof(PatchNCardUpdateVisuals), nameof(UpdateTempoVisuals))
        ]);
    }

    private static void UpdateTempoVisuals(NCard card, PileType pileType)
    {
        if (card.Model is ITempoCostCard beatCard)
        {
            var tempoCost = beatCard.GetTempoCostWithModifiers();
            
            // TODO HACK HACK HACK
            if (MelodicFlowTracker.GetMelodicFlowState(card.Model.Owner) == MelodicState.Silence)
            {
                tempoCost += 1;
            }
            
            var costDisplay = CardTempoCostDisplay.Node.Get(card);
            if (costDisplay != null)
            {
                Label costLabel = (Label)(costDisplay.GetNode("CostLabel"));
                if (costLabel != null)
                {
                    costLabel.Text = tempoCost.ToString();
                }
                costDisplay.Visible = tempoCost > 0;
            }
        }
    }
}