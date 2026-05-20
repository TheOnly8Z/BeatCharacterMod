using BeatCharacterMod.BeatCharacterModCode.Relics;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BeatCharacterMod.BeatCharacterModCode.Relics;


public class BeatWalkmanRelic() : BeatCharacterModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Tempo", 3)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        MelodicFlowHoverTip.GetStaticTip("MELODIC_FLOW"), MelodicFlowHoverTip.TempoStatic()];
    
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber > 1)
            return;
        
        await MelodicFlowTracker.GainTempo(Owner, 3M);
    }
}