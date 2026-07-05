using BeatCharacterMod.BeatCharacterModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

public class PlayTheBlues() : BeatCharacterModCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5M, ValueProp.Move), new CardsVar(1)];
    
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        for (int i = 0; i < DynamicVars.Cards.IntValue; ++i)
        {
            await Flat.CreateInHand(Owner, CombatState);
            await Cmd.Wait(0.25f);
        }
    }
    
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}