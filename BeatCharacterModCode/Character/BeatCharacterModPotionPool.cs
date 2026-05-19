using BaseLib.Abstracts;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using Godot;

namespace BeatCharacterMod.BeatCharacterModCode.Character;

public class BeatCharacterModPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => BeatCharacterMod.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}