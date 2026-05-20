using BeatCharacterMod.BeatCharacterModCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

public class LoseYourWay() : BeatCharacterModCard(2,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // TODO draw 1 card
        // TODO enter Silence
    }

    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}