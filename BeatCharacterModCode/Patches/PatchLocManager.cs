using BeatCharacterMod.BeatCharacterModCode.Formatters;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using SmartFormat;
using SmartFormat.Core.Extensions;

namespace BeatCharacterMod.BeatCharacterModCode.Patches;

[HarmonyPatch(typeof(LocManager))]
public class PatchLocManager
{
    [HarmonyPostfix]
    [HarmonyPatch("LoadLocFormatters")]
    static void PostfixLoadLocFormatters(SmartFormatter ____smartFormatter)
    {
        ____smartFormatter.AddExtensions(new TempoIconsFormatter());
    }
}