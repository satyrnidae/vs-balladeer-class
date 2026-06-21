using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Server;
using VSBalladeerClass.Effects;
using VSBalladeerClass.Network;

namespace VSBalladeerClass;

public class BalladeerModServer : BalladeerModCommon
{
    private ICoreServerAPI? ServerApi { get; set; }

    private IServerNetworkChannel ServerNetworkChannel => NetworkChannel as IServerNetworkChannel ??
                                                          throw new Exception(
                                                              "Expected the server network channel, but it wasn't registered.");

    private bool SynergyEnabled
    {
        get
        {
            if (_synergyEnabledInternal == null && ServerApi != null) {
                _synergyEnabledInternal = ServerApi.ModLoader.IsModEnabled("synergy");
            }
            return _synergyEnabledInternal ?? false;
        }
    }

    private bool? _synergyEnabledInternal;

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
                $"Failed to load the {nameof(effectshud.src.effectshud)} Mod System. Please ensure CAN Effects is installed and enabled.");
        }


        ServerNetworkChannel
            .SetMessageHandler<EffectTriggerPacket>(ReceivedEffectTriggerPacket);
        api.Event.PlayerJoin += Event_PlayerJoin;
    }

    private void Event_PlayerJoin(IServerPlayer byPlayer)
    {
        Mod.Logger.Notification($"Player {byPlayer.PlayerName} joined, syncing config.");
        ServerNetworkChannel.SendPacket(Configuration, byPlayer);
    }

    private void ReceivedEffectTriggerPacket(IServerPlayer fromPlayer, EffectTriggerPacket packet)
    {
        if (ServerApi == null) return;

        var sourcePos = fromPlayer.Entity.Pos;

        Mod.Logger.Debug($"Received {nameof(EffectTriggerPacket)} from {fromPlayer.PlayerName}. "
            + $"Applying effect to all players in a {Configuration.EffectRadius.Horizontal * 2.0f}×{Configuration.EffectRadius.Vertical * 2.0f} ellipsoid "
            + $"from point ({sourcePos}).");

        EntityPlayer[] players;
        // Use slower GetEntitiesAround to bypass weird issue with Synergy DPE leaving player world positions null
        if (SynergyEnabled) {
            players = [.. ServerApi.World.GetEntitiesAround(sourcePos.XYZFast.ToVec3d(), Configuration.EffectRadius.Horizontal,
                        Configuration.EffectRadius.Vertical, entity => entity is EntityPlayer && entity.Alive).OfType<EntityPlayer>()];
        }
        else
        {
            players = [.. ServerApi.World.GetPlayersAround(sourcePos.XYZFast.ToVec3d(), Configuration.EffectRadius.Horizontal,
                                Configuration.EffectRadius.Vertical, player => player.Entity.Alive).Select(player => player.Entity)];
        }
        if (players == null) return;
        Mod.Logger.Debug($"Found {players.Length} player{(players.Length == 1 ? "" : "s")}.");

        var effectDurationInSeconds = Math.Max(1, Configuration.EffectSettings.EffectDurationSeconds);
        var effectTier = Math.Max(1, Math.Min(5, Configuration.EffectSettings.EffectTier));

        foreach (var player in players)
        {
            Mod.Logger.Debug($"Applying effect to {player.GetName()} ({player.Pos})");
            effectshud.src.effectshud.ApplyEffectOnEntity(player,
                new BalladeerEffect(effectDurationInSeconds, effectTier));
        }
    }
}
