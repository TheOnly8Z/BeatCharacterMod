using BaseLib.Utils;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Nodes;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace BeatCharacterMod.BeatCharacterModCode.Fields;

public static class MelodicFlowFields
{
    public static readonly SpireField<PlayerCombatState, PlayerCombatStateExtensions.MelodicFlowCombatState> CombatState = new(() => null);
    
    public static readonly AddedNode<NCombatUi, NTempoCounter> TempoCounterNode =
        new((ui) =>
        {
            var tempoCounter = PreloadManager.Cache.GetScene("res://BeatCharacterMod/scenes/combat/energy_counters/tempo_counter.tscn")
                .Instantiate<NTempoCounter>();
            ui.AddChildSafely(tempoCounter);
            MainFile.Logger.Info("Loaded tempo counter " + tempoCounter);
            return tempoCounter;
        });
}