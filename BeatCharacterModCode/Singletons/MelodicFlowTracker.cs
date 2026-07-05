using BaseLib.Abstracts;
using BeatCharacterMod.BeatCharacterModCode.Character;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Fields;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace BeatCharacterMod.BeatCharacterModCode.Singletons;

public class MelodicFlowTracker() : CustomSingletonModel(true, false)
{
    // public static readonly SpireField<PlayerCombatState, decimal> Tempo = new(() => 0);
    // public static readonly SpireField<PlayerCombatState, MelodicFlowState> MelodicFlow = new(() => MelodicFlowState.None);
    
    public static int GetTempo(Player player)
    {
        if (player.PlayerCombatState == null)
        {
            return 0;
        }
        return MelodicFlowFields.CombatState[player.PlayerCombatState]?.Tempo ?? 0;
        //return player.PlayerCombatState == null ? 0 : Tempo[player.PlayerCombatState];
    }

    public static MelodicState GetMelodicFlowState(Player player)
    {
        if (player.PlayerCombatState == null)
        {
            return MelodicState.None;
        }
        return MelodicFlowFields.CombatState[player.PlayerCombatState]?.MelodicState ?? MelodicState.None;
        // return player.PlayerCombatState == null ? MelodicFlowState.None : MelodicFlow[player.PlayerCombatState];
    }
    
    /// <summary>
    /// Sets a player's Melodic Flow state.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="state">The state.</param>
    /// <returns>The task.</returns>
    public static Task SetMelodicFlowState(Player player, MelodicState state)
    {
        if (CombatManager.Instance.IsEnding || player.PlayerCombatState == null || MelodicFlowFields.CombatState[player.PlayerCombatState] == null)
            return Task.CompletedTask;
        
        // Maybe trigger "On enter state" hooks here
        
        MelodicFlowFields.CombatState[player.PlayerCombatState].MelodicState = state;
        // MelodicFlow[player.PlayerCombatState] = state;
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gives some Tempo to the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="amount">Amount to give.</param>
    /// <returns>The task.</returns>
    public static Task GainTempo(Player player, Decimal amount = 1M)
    {
        if (amount <= 0M || CombatManager.Instance.IsEnding || player.PlayerCombatState == null)
            return Task.CompletedTask;

        // Maybe trigger "On tempo gain" hooks here
        
        MelodicFlowFields.CombatState[player.PlayerCombatState].GainTempo(amount);
        // Tempo[player.PlayerCombatState] = Math.Min(999M, Tempo[player.PlayerCombatState] + amount);
        // MainFile.Logger.Info( player.Character + " gained " + amount + " tempo, now has " + Tempo[player.PlayerCombatState]);
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Deduct some Tempo from the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="amount">Amount to deduct.</param>
    /// <returns>The task.</returns>
    public static Task LoseTempo(Player player, Decimal amount = 1M)
    {
        if (amount <= 0M || CombatManager.Instance.IsEnding || player.PlayerCombatState == null)
            return Task.CompletedTask;

        // Maybe trigger "On tempo loss" hooks here
        
        MelodicFlowFields.CombatState[player.PlayerCombatState].LoseTempo(amount);
        // Tempo[player.PlayerCombatState] = Math.Max(0M, Tempo[player.PlayerCombatState] - amount);
        // MainFile.Logger.Info( player.Character + " lost " + amount + " tempo, now has " + Tempo[player.PlayerCombatState]);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Get the CardType of the most recently played card by a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skipLast">Skip X amount of most recent entries from the card history.</param>
    /// <returns>CardType of the last played card.</returns>
    public static CardType GetLastPlayedCardType(Player player, int skipLast = 0)
    {
        if (CombatManager.Instance.History.Entries
            .OfType<CardPlayFinishedEntry>()
            .SkipLast(skipLast)
            .Any(entry => entry.CardPlay != null && entry.CardPlay.Card.Owner == player))
        {
            var lastPlayedCard = CombatManager.Instance.History.Entries
                .OfType<CardPlayFinishedEntry>()
                .SkipLast(skipLast)
                .Last(entry => entry.CardPlay != null && entry.CardPlay.Card.Owner == player);
            return lastPlayedCard.CardPlay.Card.Type;
        }
        else
        {
            return CardType.None;
        }
    }

    /// <summary>
    /// Returns whether Rhythm keywords should trigger for a player (Rhythm or Silence state).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>True if the keyword is active.</returns>
    public static bool IsInRhythmState(Player player)
    {
        return GetMelodicFlowState(player) is MelodicState.Rhythm or MelodicState.Silence;
    }
    
    /// <summary>
    /// Returns whether Resonance keywords should trigger for a player (Resonance or Silence state).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>True if the keyword is active.</returns>
    public static bool IsInResonanceState(Player player)
    {
        return GetMelodicFlowState(player) is MelodicState.Resonance or MelodicState.Silence;
    }
    
    // Redundant; Tempo cost and Silence state discount is now handled via patch
    /*
    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        Player player = card.Owner;
        modifiedCost = originalCost;

        if (GetMelodicFlowState(player) is not MelodicFlowState.Silence || originalCost < 1M)
            return false;
        
        modifiedCost = originalCost - 1M;
        return true;
    }
    */
    
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        Player player = cardPlay.Card.Owner;
        
        CardType lastPlayedCardType = GetLastPlayedCardType(player, 0);
        CardType cardType = cardPlay.Card.Type;

        MainFile.Logger.Info( player.Character + " last played card type " + lastPlayedCardType + ", current played card type " + cardType);
        
        // If this is the first card played this combat (last card type was None), nothing happens
        if (lastPlayedCardType == CardType.None)
        {
            return Task.CompletedTask;
        }
        
        MelodicState melodicState = GetMelodicFlowState(player);

        // If player isn't in Melodic Flow, isn't Beat, and did not play one of Beat's cards, nothing happens 
        if (melodicState is MelodicState.None
            && player.Character is not Character.BeatCharacterMod
            && cardPlay.Card.Pool is not BeatCharacterModCardPool)
        {
            return Task.CompletedTask;
        }
        
        if (lastPlayedCardType != cardType)
        {
            if (melodicState == MelodicState.Rhythm)
            {
                GainTempo(player);
            } else if (melodicState is MelodicState.None or MelodicState.Resonance)
            {
                SetMelodicFlowState(player, MelodicState.Rhythm);
            }
        }
        else
        {
            if (melodicState is MelodicState.None or MelodicState.Rhythm)
            {
                SetMelodicFlowState(player, MelodicState.Resonance);
            }
        }
        
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player player = cardPlay.Card.Owner;

        MelodicState melodicState = GetMelodicFlowState(player);
        Decimal tempo = GetTempo(player);
        
        CardType lastPlayedCardType = GetLastPlayedCardType(player, 1);
        CardType cardType = cardPlay.Card.Type;
        
        if (melodicState is MelodicState.Silence && tempo <= 0M)
        {
            if (lastPlayedCardType != cardType)
            {
                SetMelodicFlowState(player, MelodicState.Rhythm);
            }
            else
            {
                SetMelodicFlowState(player, MelodicState.Resonance);
            }
        }

        if (player.PlayerCombatState != null)
        {
            player.PlayerCombatState.MelodicFlow().LastPlayedCardType = cardType;
        }
        
        return Task.CompletedTask;
    }
}