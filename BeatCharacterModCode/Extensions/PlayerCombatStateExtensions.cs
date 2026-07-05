using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Fields;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace BeatCharacterMod.BeatCharacterModCode.Extensions;

public static class PlayerCombatStateExtensions
{
    public class MelodicFlowCombatState(PlayerCombatState state)
    {
        private int _tempo;
        private MelodicState _melodicState;
        private CardType _lastPlayedCardType;
        
        public int Tempo
        {
            get => _tempo;
            set
            {
                if (_tempo == value)
                    return;
                var oldValue = _tempo;
                _tempo = value;
                // TODO: Add entry in CombatManager.Instance.History
                TempoChanged?.Invoke(oldValue, value);
                
                MainFile.Logger.Info( state._player + " Tempo changed from " + oldValue + " to " + value);
            }
        }

        public MelodicState MelodicState
        {
            get => _melodicState;
            set
            {
                if (_melodicState == value)
                    return;
                var oldValue = _melodicState;
                _melodicState = value;
                // TODO: Add entry in CombatManager.Instance.History
                MelodicStateChanged?.Invoke(oldValue, value);
                
                MainFile.Logger.Info( state._player + " Melodic Flow changed from " + oldValue + " to " + value);
            }
        }

        public CardType LastPlayedCardType
        {
            get => _lastPlayedCardType;
            set
            {
                if (_lastPlayedCardType == value)
                    return;
                var oldValue = _lastPlayedCardType;
                _lastPlayedCardType = value;
                LastPlayedCardTypeChanged?.Invoke(oldValue, value);
                
                MainFile.Logger.Info( state._player + " Last Played Card Type changed from " + oldValue + " to " + value);
            }
        }
        
        public event Action<MelodicState, MelodicState>? MelodicStateChanged;

        public event Action<int, int>? TempoChanged;
        
        public event Action<CardType, CardType>? LastPlayedCardTypeChanged;
        
        public void GainTempo(Decimal amount)
        {
            Tempo = !(amount < 0M) ? (int) Math.Clamp(Tempo + amount, 0, 999999999M) : throw new ArgumentException("Must not be negative.", nameof (amount));
        }

        public void LoseTempo(Decimal amount)
        {
            Tempo = !(amount < 0M) ? (int) Math.Clamp(Tempo - amount, 0, 999999999M) : throw new ArgumentException("Must not be negative.", nameof (amount));
        }
        
        /// <summary>
        /// Returns whether Rhythm keywords should trigger for a player (Rhythm or Silence state).
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if the keyword is active.</returns>
        public bool IsInRhythmState(Player player)
        {
            return MelodicState is MelodicState.Rhythm or MelodicState.Silence;
        }
    
        /// <summary>
        /// Returns whether Resonance keywords should trigger for a player (Resonance or Silence state).
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>True if the keyword is active.</returns>
        public bool IsInResonanceState(Player player)
        {
            return MelodicState is MelodicState.Resonance or MelodicState.Silence;
        }
    }
    
    public static MelodicFlowCombatState MelodicFlow(this PlayerCombatState state)
    {
        return MelodicFlowFields.CombatState[state];
    }
}