using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace BeatCharacterMod.BeatCharacterModCode.Extensions;
using static PlayerCombatStateExtensions;

public static class CombatStateTrackerExtensions
{
    private static void OnTempoChanged(this CombatStateTracker tracker, int _, int __)
    {
        tracker.NotifyCombatStateChanged("OnPlayerCombatStateValueChanged");
    }
    
    private static void OnMelodicStateChanged(this CombatStateTracker tracker, MelodicState state_old, MelodicState state_new)
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