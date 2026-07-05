using BaseLib.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Cards;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

public class FindYourVoice() : BeatCharacterModCard(1,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6M),
        new ExtraDamageVar(2M),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier(((Func<CardModel, Creature, Decimal>) ((card, _)
                => MelodicFlowTracker.GetTempo(card.Owner)))!)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [MelodicFlowHoverTip.TempoStatic()];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        FindYourVoice card = this;
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(card.DynamicVars.CalculatedDamage)
            .FromCard((CardModel) card, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => this.DynamicVars.CalculationBase.UpgradeValueBy(3M);
}