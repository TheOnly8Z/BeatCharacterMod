using BaseLib.Abstracts;
using BaseLib.Utils;
using BeatCharacterMod.BeatCharacterModCode.Character;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BeatCharacterMod.BeatCharacterModCode.Singletons;

public class MelodicFlowTracker() : CustomSingletonModel(true, false)
{
    public static readonly SpireField<PlayerCombatState, decimal> Tempo = new(() => 0);

    public static readonly SpireField<PlayerCombatState, MelodicFlowState> MelodicFlow = new(() => MelodicFlowState.None);
    
    public static Decimal GetTempo(Player player)
    {
        return player.PlayerCombatState == null ? 0 : Tempo[player.PlayerCombatState];
    }

    public static MelodicFlowState GetMelodicFlowState(PlayerCombatState state)
    {
        return MelodicFlow[state];
    }

    public static MelodicFlowState GetMelodicFlowState(Player player)
    {
        return player.PlayerCombatState == null ? MelodicFlowState.None : MelodicFlow[player.PlayerCombatState];
    }
    
    public static Task SetMelodicFlowState(Player player, MelodicFlowState state)
    {
        if (CombatManager.Instance.IsEnding || player.PlayerCombatState == null)
            return Task.CompletedTask;
        
        // Maybe trigger "On enter state" hooks here
        
        MainFile.Logger.Info( player.Character + " Melodic Flow state changing from " + MelodicFlow[player.PlayerCombatState] + " to " + state);
        
        MelodicFlow[player.PlayerCombatState] = state;
        
        return Task.CompletedTask;
    }

    public static Task GainTempo(Player player, Decimal amount = 1M)
    {
        if (amount <= 0M || CombatManager.Instance.IsEnding || player.PlayerCombatState == null)
            return Task.CompletedTask;

        // Maybe trigger "On tempo gain" hooks here
        
        Tempo[player.PlayerCombatState] += amount;
        
        MainFile.Logger.Info( player.Character + " gained " + amount + " tempo, now has " + Tempo[player.PlayerCombatState]);
        
        return Task.CompletedTask;
    }
    
    public static Task LoseTempo(Player player, Decimal amount = 1M)
    {
        if (amount <= 0M || CombatManager.Instance.IsEnding || player.PlayerCombatState == null)
            return Task.CompletedTask;

        // Maybe trigger "On tempo loss" hooks here
        
        Tempo[player.PlayerCombatState] -= amount;
        
        MainFile.Logger.Info( player.Character + " lost " + amount + " tempo, now has " + Tempo[player.PlayerCombatState]);

        return Task.CompletedTask;
    }

    public static CardType GetLastPlayedCardType(Player player, int skipLast = 1)
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
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player player = cardPlay.Card.Owner;
        
        CardType lastPlayedCardType = GetLastPlayedCardType(player);
        CardType cardType = cardPlay.Card.Type;

        MainFile.Logger.Info( player.Character + " last played card type " + lastPlayedCardType + ", current played card type " + cardType);
        
        // If this is the first card played this combat (last card type was None), nothing happens
        if (lastPlayedCardType == CardType.None)
        {
            return Task.CompletedTask;
        }
        
        // If player isn't in Melodic Flow, isn't Beat, and did not play one of Beat's cards, nothing happens 
        if (GetMelodicFlowState(player) == MelodicFlowState.None
            && player.Character is not Character.BeatCharacterMod
            && cardPlay.Card.Pool is not BeatCharacterModCardPool)
        {
            return Task.CompletedTask;
        }
        
        MelodicFlowState melodicFlowState = GetMelodicFlowState(player);
        Decimal tempo = GetTempo(player);
        
        // If in Silence stance and ran out of Tempo, exit stance into what would be the proper stance
        // Making the assumption that entering Silence stance requires at least 1 card played this combat
        bool shouldExitSilence = melodicFlowState is MelodicFlowState.Silence && tempo <= 0M;
        
        if (lastPlayedCardType != cardType)
        {
            if (melodicFlowState == MelodicFlowState.Rhythm)
            {
                GainTempo(player);
            } else if (shouldExitSilence || melodicFlowState is MelodicFlowState.None or MelodicFlowState.Resonance)
            {
                SetMelodicFlowState(player, MelodicFlowState.Rhythm);
            }
        }
        else
        {
            if (shouldExitSilence || melodicFlowState is MelodicFlowState.None or MelodicFlowState.Rhythm)
            {
                SetMelodicFlowState(player, MelodicFlowState.Resonance);
            }
        }
        
        return Task.CompletedTask;
    }
}