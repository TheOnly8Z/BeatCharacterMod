using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using BeatCharacterMod.BeatCharacterModCode.Character;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Formatters;
using BeatCharacterMod.BeatCharacterModCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

[Pool(typeof(BeatCharacterModCardPool))]
public abstract class BeatCharacterModCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target), ITempoCostCard
{
    protected int _tempo = 0;

    public virtual int Tempo
    {
        get => _tempo;
        set
        {
            if (_tempo == value)
            {
                return;
            }
            int oldValue = _tempo;
            _tempo = value;
            // TODO figure out if I want to log Tempo modification like Stars
            // CombatManager.Instance.History.StarsModified(Owner.Creature.CombatState, this._tempo - oldValue, Owner);
        }
    }

    public int GetTempoCostWithModifiers()
    {
        // TODO worry about Tempo modifiers
        return Tempo;
    }

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}