using instruments;
using System;
using System.Reflection;
using effectshud.src;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using VSBalladeerClass.Network;

namespace VSBalladeerClass
{
    public class BalladeerModClient : BalladeerModCommon
    {
        // Retrieve the field with reflection ONCE
        private static FieldInfo ThisClientPlayingField { get; } =
            typeof(InstrumentModClient).GetField("thisClientPlaying", BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new Exception("Failed to find thisClientPlaying field!");

        private InstrumentModClient? InstrumentMod { get; set; }

        private ICoreClientAPI? ClientApi { get; set; }

        private long TickListenerId { get; set; }

        private bool IsClientPlayerPlaying =>
            InstrumentMod != null && ((bool?)ThisClientPlayingField.GetValue(InstrumentMod) ?? false);

        private IClientNetworkChannel ClientNetworkChannel => NetworkChannel as IClientNetworkChannel ??
                                                              throw new Exception(
                                                                  "Expected the client network channel, but it wasn't registered.");

        private bool _wasPlayingLastTick;
        private short _effectTriggerTimer;

        public override void Dispose()
        {
            base.Dispose();
            if (TickListenerId != 0)
            {
                ClientApi?.Event.UnregisterGameTickListener(TickListenerId);
            }
        }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);

            ClientApi = api;
            InstrumentMod = api.ModLoader.GetModSystem<InstrumentModClient>() ?? throw new Exception(
                $"Failed to locate the {nameof(InstrumentModClient)} mod system.  Please ensure Instruments is installed and configured.");

            ClientNetworkChannel
                .SetMessageHandler<ConfigurationSyncPacket>(ReceiveConfigurationSyncPacket);

            TickListenerId = api.Event.RegisterGameTickListener(OnGameTick, 50); // 20 Hz update rate
            // We are letting CAN Effects handle the effect stuff :3
        }

        private void ReceiveConfigurationSyncPacket(ConfigurationSyncPacket packet)
        {
            Mod.Logger.Notification("Overriding client configuration with server configuration.");
            // Does the effect radius really *need* to be synced? No. Will I anyway? I guess!
            Mod.Logger.Debug(
                $"{nameof(Configuration)}.{nameof(Configuration.EffectRadius)}.{nameof(Configuration.EffectRadius.Horizontal)}: {Configuration.EffectRadius.Horizontal} => {packet.EffectRadiusHorizontal}");
            Configuration.EffectRadius.Horizontal = packet.EffectRadiusHorizontal;
            Mod.Logger.Debug(
                $"{nameof(Configuration)}.{nameof(Configuration.EffectRadius)}.{nameof(Configuration.EffectRadius.Vertical)}: {Configuration.EffectRadius.Vertical} => {packet.EffectRadiusVertical}");
            Configuration.EffectRadius.Vertical = packet.EffectRadiusVertical;
        }

        private void OnGameTick(float dt)
        {
            if (ClientApi == null) return;

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
                Mod.Logger.Debug($"Local balladeer {ClientApi.World.Player.PlayerName} is triggering an effect phase.");
                ClientNetworkChannel.SendPacket(new EffectTriggerPacket()
                {
                    SourcePos = new Vec3d(ClientApi.World.Player.Entity.Pos.X, ClientApi.World.Player.Entity.Pos.Y,
                        ClientApi.World.Player.Entity.Pos.Z)
                });
            }

            _effectTriggerTimer = (short)(++_effectTriggerTimer % 60); // Trigger effect every three seconds
        }
    }
}