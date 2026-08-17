using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Interfaces;
using BeatCharacterMod.BeatCharacterModCode.Powers;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BeatCharacterMod.BeatCharacterModCode.Powers;

public class DanceDelightfulPower() : BeatCharacterModPower, IMelodicStatePower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public async Task AfterMelodicStateChanged(Player player, MelodicState state_old, MelodicState state_new)
    {
        if (player != Owner.Player)
        {
            return;
        }
        bool old_strength = state_old is MelodicState.Rhythm or MelodicState.Silence;
        bool new_strength = state_new is MelodicState.Rhythm or MelodicState.Silence;
        if (old_strength && !new_strength)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, -Amount, Owner, null);
        }
        else if (new_strength && !old_strength)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
        }
        
        bool old_dexterity = state_old is MelodicState.Resonance or MelodicState.Silence;
        bool new_dexterity = state_new is  MelodicState.Resonance or MelodicState.Silence;
        if (old_dexterity && !new_dexterity)
        {
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner, -Amount, Owner, null);
        }
        else if (new_dexterity && !old_dexterity)
        {
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (!Owner.IsPlayer || power != this)
        {
            return;
        }
        if (MelodicFlowTracker.IsInRhythmState(Owner.Player))
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, amount, Owner, null);
        }
        if (MelodicFlowTracker.IsInResonanceState(Owner.Player))
        {
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner, amount, Owner, null);
        }
    }
}