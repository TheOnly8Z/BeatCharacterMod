using BeatCharacterMod.BeatCharacterModCode.Cards;
using BeatCharacterMod.BeatCharacterModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

public class DanceDelightful() : BeatCharacterModCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DanceDelightfulPower>(2M)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<DanceDelightfulPower>(choiceContext, Owner.Creature, DynamicVars["DanceDelightfulPower"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["DanceDelightfulPower"].UpgradeValueBy(1M);
    }
}