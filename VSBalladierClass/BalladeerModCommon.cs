using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using VSBalladeerClass.Model;
using VSBalladeerClass.Network;

namespace VSBalladeerClass
{
    public abstract class BalladeerModCommon : ModSystem
    {
        private const string CONFIGURATION_FILE_NAME = nameof(VSBalladeerClass) + ".json";
        private const string NET_CHANNEL_NAME = nameof(VSBalladeerClass) + "ClientNetworkChannel";

        protected Configuration Configuration => _configuration ?? new Configuration();
        protected INetworkChannel? NetworkChannel { get; set; }

        private Configuration? _configuration;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return false;
        }

        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api)
        {
            Mod.Logger.Notification($"Starting Balladeer on {api.Side} side.");
            if (_configuration == null)
            {
                LoadConfiguration(api);
            }

            var netChannel = api.Network.GetChannel(NET_CHANNEL_NAME);
            if (netChannel != null)
            {
                Mod.Logger.Warning($"Channel {NET_CHANNEL_NAME} was already registered at startup!");
                NetworkChannel ??= netChannel;
            }
            else
                NetworkChannel ??= api.Network.RegisterChannel(NET_CHANNEL_NAME) ??
                                   throw new Exception(
                                       $"Failed to register channel {NET_CHANNEL_NAME} on side {api.Side}!");

            NetworkChannel
                .RegisterMessageType(typeof(EffectTriggerPacket))
                .RegisterMessageType(typeof(ConfigurationSyncPacket));
        }

        private void LoadConfiguration(ICoreAPI api)
        {
            var loadSuccessful = false;
            try
            {
                _configuration = api.LoadModConfig<Configuration>(CONFIGURATION_FILE_NAME);
                loadSuccessful = true;
            }
            catch (Exception e)
            {
                Mod.Logger.Error($"Failed to load mod configuration for Balladeer: {e.Message}!");
                Mod.Logger.Warning("Default configuration will be used. Please correct your config file!");
            }

            _configuration ??= new Configuration();

            if (!loadSuccessful) return;

            Mod.Logger.Notification("Loaded configuration for Balladeer.");

            try
            {
                api.StoreModConfig(_configuration, CONFIGURATION_FILE_NAME);
            }
            catch (Exception e)
            {
                Mod.Logger.Error($"Failed to save mod configuration for Balladeer: {e.Message}!");
            }
        }
    }
}