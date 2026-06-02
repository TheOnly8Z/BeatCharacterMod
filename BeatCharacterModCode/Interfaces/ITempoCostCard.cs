namespace BeatCharacterMod.BeatCharacterModCode.Interfaces;

public interface ITempoCostCard
{
    public int Tempo { get; set; }
    
    public int GetTempoCostWithModifiers();
}