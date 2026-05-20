using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace BeatCharacterMod.BeatCharacterModCode.Formatters;

public class TempoIconsFormatter : IFormatter
{
    private const string tempoIconSprite = "[img]res://images/packed/sprite_fonts/tempo_icon.png[/img]";

    public string Name
    {
        get => "tempoIcons";
        set => throw new NotImplementedException();
    }
    
    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        int count;
        switch (formattingInfo.CurrentValue)
        {
            case DynamicVar dynamicVar:
                count = (int) dynamicVar.PreviewValue;
                break;
            case Decimal num1:
                count = (int) num1;
                break;
            case int num2:
                count = num2;
                break;
            default:
                throw new LocException($"Unknown value='{formattingInfo.CurrentValue}' type={formattingInfo.CurrentValue?.GetType()}");
        }
        string text = string.Concat(Enumerable.Repeat<string>(tempoIconSprite, count));
        formattingInfo.Write(text);
        return true;
    }
}