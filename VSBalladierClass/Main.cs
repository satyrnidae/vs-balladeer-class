using effectshud.src;
using effectshud.src.DefaultEffects;
using instruments;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using VSBalladeerClass.Effects;

namespace VSBalladierClass
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class BalladeerEffectPulse
    {
        public Vec3d pos;
    }

    public class VSBalladeerModCommon : ModSystem
    {
        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api)
        {
            Mod.Logger.Notification("Loading Balladeer class for side " + api.Side);
        }
    }

    public class VSBalladeerModClient : VSBalladeerModCommon
    {
        internal BalladeerClientGameTickHandler? ClientTickHander { get; private set; }
        public IClientNetworkChannel? BalladeerEffectPulseChannel { get; private set; }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            BalladeerEffectPulseChannel = api.Network.RegisterChannel("balladeerEffectPulse")
                .RegisterMessageType(typeof(BalladeerEffectPulse));
            ClientTickHander = new BalladeerClientGameTickHandler(Mod.Logger, api, BalladeerEffectPulseChannel);
            // We are letting CAN Effects handle the effect stuff :3
        }

        public override void Dispose()
        {
            base.Dispose();
            ClientTickHander?.Dispose();
        }
    }

    public class VSBalladeerModServer : VSBalladeerModCommon
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
                throw new Exception("oough i am missing a dependency argh");
            }
            BalladeerEffectPulseChannel = api.Network.RegisterChannel("balladeerEffectPulse")
                .RegisterMessageType(typeof(BalladeerEffectPulse))
                .SetMessageHandler<BalladeerEffectPulse>(DoEffectPulse);
        }

        private void DoEffectPulse(IServerPlayer fromPlayer, BalladeerEffectPulse packet)
        {
            var players = ServerApi?.World.GetPlayersAround(packet.pos, 20, 5);
            if (players == null) return;

            foreach (var player in players)
            {
                effectshud.src.effectshud.ApplyEffectOnEntity(player.Entity, new BalladeerEffect(seconds: 5, tier: 1, infinite: false));
            }
        }
    }

    internal class BalladeerClientGameTickHandler : IDisposable
    {
        public ILogger Logger { get; }
        public ICoreClientAPI ClientApi { get; }
        public IClientNetworkChannel Channel { get; }
        public InstrumentModClient InstrumentMod { get; }
        public FieldInfo ThisClientPlayingField { get; } = typeof(InstrumentModClient).GetField("thisClientPlaying", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Failed to find thisClientPlaying field!");
        public bool IsClientPlayerPlaying
        {
            get
            {
                return (bool?)ThisClientPlayingField.GetValue(InstrumentMod) ?? false;
            }
        }
        public long TickListenerId { get; set; }

        private bool wasPlayingLastTick;
        private short effectPulseTimer;

        public BalladeerClientGameTickHandler(
            ILogger logger,
            ICoreClientAPI api,
            IClientNetworkChannel channel)
        {
            Logger = logger;
            ClientApi = api;
            Channel = channel;
            InstrumentMod = api.ModLoader.GetModSystem<InstrumentModClient>() ?? throw new Exception("Failed to get the Instrument client-side mod!");
            TickListenerId = api.Event.RegisterGameTickListener(OnClientGameTick, 50, 0);
        }

        public void Dispose()
        {
            ClientApi.Event.UnregisterGameTickListener(TickListenerId);
        }

        public void OnClientGameTick(float dt)
        {
            var charClass = ClientApi.World.Player.Entity.WatchedAttributes.GetString("characterClass");
            if (charClass == "balladeer")
            {
                if (!IsClientPlayerPlaying)
                {
                    wasPlayingLastTick = false;
                    effectPulseTimer = 0;
                }
                else if (!wasPlayingLastTick)
                {
                    wasPlayingLastTick = true;
                }

                if (wasPlayingLastTick)
                {
                    if (effectPulseTimer == 0)
                    {
                        Logger.Notification($"{dt} Local balladeer {ClientApi.World.Player.PlayerName} is triggering an effect phase.");
                        Channel.SendPacket(new BalladeerEffectPulse()
                        {
                            pos = new Vec3d(ClientApi.World.Player.Entity.Pos.X, ClientApi.World.Player.Entity.Pos.Y, ClientApi.World.Player.Entity.Pos.Z)
                        });
                    }
                    effectPulseTimer = (short)(++effectPulseTimer % 60);
                }
            }
        }
    }
}
