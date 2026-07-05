using BeatCharacterMod.BeatCharacterModCode.Fields;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace BeatCharacterMod.BeatCharacterModCode.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
public class PatchNCombatUi
{
    [HarmonyPostfix]
    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        var counter = MelodicFlowFields.TempoCounterNode[__instance];
        if (counter == null) return;
        counter.Initialize(LocalContext.GetMe(state)!);
        counter.Reparent(__instance._energyCounter);
        counter.Position = new Vector2(0, -150);
        //counter.Size = new Vector2(128, 128);
    }
}