using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace BeatCharacterMod.BeatCharacterModCode.Nodes;

public partial class CardTempoCostDisplay : Control
{
    // TODO create a scene instead of this
    /*
    public static AddedNode<NCard, CardTempoCostDisplay> Node = new((card) =>
    {
        var control = new CardTempoCostDisplay();
        
        // TODO replace this with the proper background
        var tex = ResourceLoader.Load<Texture2D>("res://BeatCharacterMod/images/ui/combat/energy_tempo.png");
        
        var size = tex.GetSize();
        var texRect = new TextureRect();
        texRect.Name = tex.ResourcePath;
        texRect.Size = new(50, 50);
        texRect.Texture = tex;
        texRect.PivotOffset = size / 2f;
        texRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        texRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        texRect.MouseFilter = MouseFilterEnum.Ignore;
        
        control.Size = new(50, 50);
        control.Position = new(-126, -231);
        control.AddChild(texRect);
        
        var label = new Label { Text = "1" };
        label.SetAnchorsAndOffsetsPreset(LayoutPreset.Center);
        control.AddChild(label);
        
        return control;
    });
    */
}