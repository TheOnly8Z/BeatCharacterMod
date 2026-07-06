using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

public class CharredMemoir() : BeatCharacterModCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Burn>(), MelodicFlowHoverTip.FromMelodicFlow(MelodicState.Silence, this)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MelodicFlowTracker.SetMelodicFlowState(Owner, MelodicState.Silence);
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Burn>(Owner), PileType.Hand, Owner);
    }

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}