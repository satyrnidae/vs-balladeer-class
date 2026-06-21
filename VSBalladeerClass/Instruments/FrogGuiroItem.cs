using instruments;
using Vintagestory.API.Common;

namespace VSBalladeerClass.Instruments;

public class FrogGuiroItem : InstrumentItem
{
    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        instrument = "frogguiro";
        Definitions.GetInstance().AddInstrumentType(instrument, animation);
    }
}
