using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace BeatCharacterMod.BeatCharacterModCode.Cards;

[Pool(typeof(CurseCardPool))]
public class Offbeat() : CustomCardModel(1, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;
    public override bool HasTurnEndInHandEffect => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Tempo", 2)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal, CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [MelodicFlowHoverTip.TempoStatic()];
    
    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        Owner.PlayerCombatState.MelodicFlow().LoseTempo(DynamicVars["Tempo"].BaseValue);
        await Cmd.Wait(0.25f);
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