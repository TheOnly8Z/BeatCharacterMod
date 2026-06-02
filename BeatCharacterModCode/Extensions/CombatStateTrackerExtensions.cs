using BeatCharacterMod.BeatCharacterModCode.Enums;
using MegaCrit.Sts2.Core.Combat;

namespace BeatCharacterMod.BeatCharacterModCode.Extensions;
using static PlayerCombatStateExtensions;

public static class CombatStateTrackerExtensions
{
    private static void OnTempoChanged(this CombatStateTracker tracker, int _, int __)
    {
        tracker.NotifyCombatStateChanged("OnPlayerCombatStateValueChanged");
    }
    
    private static void OnMelodicStateChanged(this CombatStateTracker tracker, MelodicState _, MelodicState __)
    {
        tracker.NotifyCombatStateChanged("OnPlayerCombatStateValueChanged");
    }

    public static void SubscribeMelodicFlow(this CombatStateTracker tracker, MelodicFlowCombatState combatState)
    {
        combatState.TempoChanged += tracker.OnTempoChanged;
        combatState.MelodicStateChanged += tracker.OnMelodicStateChanged;
    }

    public static void UnsubscribeMelodicFlow(this CombatStateTracker tracker, MelodicFlowCombatState combatState)
    {
        combatState.TempoChanged -= tracker.OnTempoChanged;
        combatState.MelodicStateChanged -= tracker.OnMelodicStateChanged;
    }
}