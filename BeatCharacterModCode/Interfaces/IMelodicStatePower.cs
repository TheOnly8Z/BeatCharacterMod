using BeatCharacterMod.BeatCharacterModCode.Enums;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace BeatCharacterMod.BeatCharacterModCode.Interfaces;

public interface IMelodicStatePower
{
    public Task AfterMelodicStateChanged(Player player, MelodicState state_old, MelodicState state_new);
}