using instruments;
using System;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using VSBalladeerClass.Model;
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
        private CharacterSystem? CharacterSystem { get; set; }

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
            CharacterSystem = api.ModLoader.GetModSystem<CharacterSystem>() ?? throw new Exception(
                $"Failed to locate the {nameof(CharacterSystem)} mod system. Please ensure Survival is enabled.");

            ClientNetworkChannel
                .SetMessageHandler<Configuration>(ReceiveConfigurationSyncPacket);

            TickListenerId = api.Event.RegisterGameTickListener(OnGameTick, 50); // 20 Hz update rate
            // We are letting CAN Effects handle the effect stuff :3
        }

        private void ReceiveConfigurationSyncPacket(Configuration packet)
        {
            Mod.Logger.Notification("Overriding client configuration with server configuration.");
            Mod.Logger.Debug(
                $"{nameof(Configuration)}.{nameof(Configuration.ActivationPerSeconds)}: {Configuration.ActivationPerSeconds} => {packet.ActivationPerSeconds}");
            Configuration.ActivationPerSeconds = packet.ActivationPerSeconds;
        }

        private void OnGameTick(float dt)
        {
            // Don't bother doing anything unless the player is a balladeer.
            if (ClientApi == null || CharacterSystem == null || !CharacterSystem.HasTrait(ClientApi.World.Player, "bard")) return;

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

            _effectTriggerTimer = (short)(++_effectTriggerTimer % Math.Max(1, Configuration.ActivationPerSeconds * 20)); // Trigger effect every <ActivationPerSeconds> seconds (min 1)
        }
    }
}