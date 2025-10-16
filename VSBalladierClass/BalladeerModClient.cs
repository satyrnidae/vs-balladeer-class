using Vintagestory.API.Client;
using Vintagestory.API.Common;
using VSBalladeerClass.Network;

namespace VSBalladeerClass
{
    public class BalladeerModClient : BalladeerModCommon
    {
        internal MainClientLoop? ClientTickHandler { get; private set; }
        public IClientNetworkChannel? BalladeerEffectPulseChannel { get; private set; }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            BalladeerEffectPulseChannel = api.Network.RegisterChannel("balladeerEffectPulse")
                .RegisterMessageType(typeof(BalladeerEffectTrigger));
            ClientTickHandler = new MainClientLoop(Mod.Logger, api, BalladeerEffectPulseChannel);
            // We are letting CAN Effects handle the effect stuff :3
        }

        public override void Dispose()
        {
            base.Dispose();
            ClientTickHandler?.Dispose();
        }
    }
}