using BaseLib.Abstracts;
using BaseLib.Utils;
using BeatCharacterMod.BeatCharacterModCode.Character;

namespace BeatCharacterMod.BeatCharacterModCode.Potions;

[Pool(typeof(BeatCharacterModPotionPool))]
public abstract class BeatCharacterModPotion : CustomPotionModel;