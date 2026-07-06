using BaseLib.Abstracts;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BeatCharacterMod.BeatCharacterModCode.Singletons;

public class MelodicFlowHoverTip() : CustomSingletonModel(false, false)
{
    private static LocString L10NStatic(string entry) => new LocString("static_hover_tips", entry);

    public static IHoverTip GetStaticTip(string str, params DynamicVar[] vars)
    {
        LocString title = L10NStatic(str + ".title");
        LocString description = L10NStatic(str + ".description");
        foreach (DynamicVar var in vars)
        {
            title.Add(var);
            description.Add(var);
        }
        return new HoverTip(title, description);
    }
    
    public static IHoverTip FromMelodicFlow(MelodicState state, AbstractModel model, params DynamicVar[] vars)
    {
        string str = StringHelper.Slugify(state.ToString());
        LocString title = L10NStatic("MELODIC_FLOW_" + str + ".title");
        LocString description = L10NStatic("MELODIC_FLOW_" + str + ".description");
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(model));
        description.Add("tempoIcon", "[img]res://BeatCharacterMod/images/packed/sprite_fonts/tempo_icon.png[/img]");
        
        foreach (DynamicVar var in vars)
        {
            title.Add(var);
            description.Add(var);
        }
        return new HoverTip(title, description);
    }

    public static IHoverTip TempoStatic()
    {
        return new HoverTip(L10NStatic("TEMPO_STATIC.title")
            , L10NStatic("TEMPO_STATIC.description")
            , PreloadManager.Cache.GetTexture2D(Path.Join(MainFile.ResPath, "images", "packed", "sprite_fonts", "tempo_icon.png")));
    }

}