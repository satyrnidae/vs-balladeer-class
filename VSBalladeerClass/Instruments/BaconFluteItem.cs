using instruments;
using Vintagestory.API.Common;

namespace VSBalladeerClass.Instruments;

public class BaconFluteItem : InstrumentItem
{
    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        instrument = "baconflute";
        animation = "flutecallloop";
        Definitions.GetInstance().AddInstrumentType(instrument, animation);
    }
}
