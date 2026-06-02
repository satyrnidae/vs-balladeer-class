using instruments;
using Vintagestory.API.Common;

namespace VSBalladeerClass.Instruments;

public class BoneFluteItem : InstrumentItem
{
    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        instrument = "boneflute";
        animation = "flutecallloop";
        Definitions.GetInstance().AddInstrumentType(instrument, animation);
    }
}
