using BeatCharacterMod.BeatCharacterModCode.Cards;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Interfaces;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;



public class TonePolice() : BeatCharacterModCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public override int Tempo => 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5M, ValueProp.Move), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        MelodicFlowHoverTip.FromMelodicFlow(MelodicState.Rhythm, this),
        MelodicFlowHoverTip.FromMelodicFlow(MelodicState.Resonance, this),
        HoverTipFactory.FromCard<Flat>(),
        HoverTipFactory.FromCard<Sharp>(),];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (Owner.PlayerCombatState.MelodicFlow().IsInRhythmState())
        {
            await Flat.CreateInHand(Owner, DynamicVars.Cards.IntValue, CombatState);
            await Cmd.Wait(0.1f);
        }
        if (Owner.PlayerCombatState.MelodicFlow().IsInResonanceState())
        {
            await Sharp.CreateInHand(Owner, DynamicVars.Cards.IntValue, CombatState);
            await Cmd.Wait(0.1f);
        }
    }

    protected override void OnUpgrade() => this.DynamicVars.Damage.UpgradeValueBy(3M);
}