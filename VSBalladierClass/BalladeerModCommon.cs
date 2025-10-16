using Vintagestory.API.Common;

namespace VSBalladeerClass
{
    public class BalladeerModCommon : ModSystem
    {
        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api)
        {
            Mod.Logger.Notification("Loading Balladeer class for side " + api.Side);
        }
    }
}