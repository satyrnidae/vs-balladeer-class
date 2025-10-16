using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using VSBalladeerClass.Effects;
using VSBalladeerClass.Network;

namespace VSBalladeerClass
{
    public class BalladeerModServer : BalladeerModCommon
    {
        public ICoreServerAPI? ServerApi { get; private set; }
        public IServerNetworkChannel? BalladeerEffectPulseChannel { get; private set; }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            ServerApi = api;
            if (api.ModLoader.GetModSystem<effectshud.src.effectshud>() == null)
            {
                throw new Exception(
                    "Failed to load the effectshud ModSystem. Please ensure CAN Effects is installed and enabled.");
            }

            BalladeerEffectPulseChannel = api.Network.RegisterChannel("balladeerEffectPulse")
                .RegisterMessageType(typeof(BalladeerEffectTrigger))
                .SetMessageHandler<BalladeerEffectTrigger>(DoEffectPulse);
        }

        private void DoEffectPulse(IServerPlayer fromPlayer, BalladeerEffectTrigger packet)
        {
            var players = ServerApi?.World.GetPlayersAround(packet.SourcePos, 20, 5);
            if (players == null) return;

            foreach (var player in players)
            {
                effectshud.src.effectshud.ApplyEffectOnEntity(player.Entity,
                    new BalladeerEffect(seconds: 5, tier: 1, infinite: false));
            }
        }
    }
}