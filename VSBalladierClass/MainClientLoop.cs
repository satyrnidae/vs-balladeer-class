using instruments;
using System;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using VSBalladeerClass.Network;

namespace VSBalladeerClass
{
    internal class MainClientLoop : IDisposable
    {
        // Retrieve the field with reflection ONCE
        private static FieldInfo ThisClientPlayingField { get; } =
            typeof(InstrumentModClient).GetField("thisClientPlaying", BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new Exception("Failed to find thisClientPlaying field!");

        public ILogger Logger { get; }

        public ICoreClientAPI ClientApi { get; }

        public IClientNetworkChannel Channel { get; }

        public InstrumentModClient InstrumentMod { get; }

        public bool IsClientPlayerPlaying => (bool?)ThisClientPlayingField.GetValue(InstrumentMod) ?? false;

        public long TickListenerId { get; set; }

        private bool _wasPlayingLastTick;
        private short _effectTriggerTimer;

        public MainClientLoop(
            ILogger logger,
            ICoreClientAPI api,
            IClientNetworkChannel channel)
        {
            Logger = logger;
            ClientApi = api;
            Channel = channel;
            InstrumentMod = api.ModLoader.GetModSystem<InstrumentModClient>() ?? throw new Exception(
                $"Failed to locate the {nameof(InstrumentModClient)} mod system.  Please ensure Instruments is installed and configured.");
            TickListenerId = api.Event.RegisterGameTickListener(Update, 50); // 20 Hz update rate
        }

        public void Dispose()
        {
            ClientApi.Event.UnregisterGameTickListener(TickListenerId);
        }

        public void Update(float dt)
        {
            var charClass = ClientApi.World.Player.Entity.WatchedAttributes.GetString("characterClass");
            if (charClass != "balladeer") return; // Don't bother doing anything unless the player is a balladeer.

            if (!IsClientPlayerPlaying)
            {
                _wasPlayingLastTick = false;
                _effectTriggerTimer = 0;
            }
            else if (!_wasPlayingLastTick)
            {
                _wasPlayingLastTick = true;
            }

            if (!_wasPlayingLastTick) return;

            if (_effectTriggerTimer == 0)
            {
                Logger.Debug($"Local balladeer {ClientApi.World.Player.PlayerName} is triggering an effect phase.");
                Channel.SendPacket(new BalladeerEffectTrigger()
                {
                    SourcePos = new Vec3d(ClientApi.World.Player.Entity.Pos.X, ClientApi.World.Player.Entity.Pos.Y,
                        ClientApi.World.Player.Entity.Pos.Z)
                });
            }

            _effectTriggerTimer = (short)(++_effectTriggerTimer % 60); // Trigger effect every three seconds
        }
    }
}