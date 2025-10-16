using System;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using VSBalladeerClass.Effects;
using VSBalladeerClass.Network;

namespace VSBalladeerClass
{
    public class BalladeerModServer : BalladeerModCommon
    {
        private ICoreServerAPI? ServerApi { get; set; }

        private IServerNetworkChannel ServerNetworkChannel => NetworkChannel as IServerNetworkChannel ??
                                                              throw new Exception(
                                                                  "Expected the server network channel, but it wasn't registered.");

        public override void Dispose()
        {
            if (ServerApi == null) return;

            ServerApi.Event.PlayerJoin -= Event_PlayerJoin;
        }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);

            ServerApi = api;
            if (api.ModLoader.GetModSystem<effectshud.src.effectshud>() == null)
            {
                throw new Exception(
                    "Failed to load the effectshud ModSystem. Please ensure CAN Effects is installed and enabled.");
            }

            ServerNetworkChannel
                .SetMessageHandler<EffectTriggerPacket>(ReceivedEffectTriggerPacket);
            api.Event.PlayerJoin += Event_PlayerJoin;
        }

        private void Event_PlayerJoin(IServerPlayer byPlayer)
        {
            Mod.Logger.Notification($"Player {byPlayer.PlayerName} joined, syncing config.");
            ServerNetworkChannel.SendPacket(new ConfigurationSyncPacket()
            {
                EffectRadiusHorizontal = Configuration.EffectRadius.Horizontal,
                EffectRadiusVertical = Configuration.EffectRadius.Vertical
            }, byPlayer);
        }

        private void ReceivedEffectTriggerPacket(IServerPlayer fromPlayer, EffectTriggerPacket packet)
        {
            if (ServerApi == null) return;

            Mod.Logger.Debug($"Received {nameof(EffectTriggerPacket)} from {fromPlayer.PlayerName}. Applying effect to all players in a {Configuration.EffectRadius.Horizontal * 2.0f}x{Configuration.EffectRadius.Vertical * 2.0f} ellipsoid from point ({packet.SourcePos.X}, {packet.SourcePos.Y}, {packet.SourcePos.Z}).");
            var players = ServerApi.World.GetPlayersAround(packet.SourcePos, Configuration.EffectRadius.Horizontal,
                Configuration.EffectRadius.Vertical, player => player.Entity.Alive);
            if (players == null) return;
            Mod.Logger.Debug($"Found {players.Length} player{(players.Length == 1 ? "" : "s")}.");

            foreach (var player in players)
            {
                Mod.Logger.Debug($"Applying effect to {player.PlayerName} ({player.Entity.Pos.X}, {player.Entity.Pos.Y}, {player.Entity.Pos.Z})");
                effectshud.src.effectshud.ApplyEffectOnEntity(player.Entity,
                    new BalladeerEffect(seconds: 5, tier: 1, infinite: false));
            }
        }
    }
}