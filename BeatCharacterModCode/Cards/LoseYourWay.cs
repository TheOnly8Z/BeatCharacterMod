using BaseLib.Patches.Localization;
using BeatCharacterMod.BeatCharacterModCode.Cards;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using StaticHoverTip = MegaCrit.Sts2.Core.HoverTips.StaticHoverTip;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

public class LoseYourWay() : BeatCharacterModCard(2,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [MelodicFlowHoverTip.FromMelodicFlow(MelodicState.Silence, Owner.Character)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        LoseYourWay cardSource = this;
        IEnumerable<CardModel> cardModels = await CardPileCmd.Draw(choiceContext, cardSource.DynamicVars.Cards.BaseValue, cardSource.Owner);

        await MelodicFlowTracker.SetMelodicFlowState(Owner, MelodicState.Silence);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1M);
}