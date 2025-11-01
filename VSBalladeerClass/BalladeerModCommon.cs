using System;
using effectshud.src;
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

        public const string ACTIVATION_PER_SECONDS_COMMENT =
            "The number of seconds between effect activations while a Bard is playing. Ideally less than EffectDurationSeconds. (Min: 1)";

        public const string EFFECT_TIER_COMMENT =
            "Multiplies the effect of the Bard trait. Note that the description text will still read at the 1x value. (Min: 1, Max: 3)";

        public const string EFFECT_DURATION_SECONDS_COMMENT =
            "How long an applied effect will last. This continues past the point where the Bard stops playing. Ideally this should be longer than ActivationPerSeconds. (Min: 1)";

        public const string VERTICAL_EFFECT_RADIUS_COMMENT =
            "The vertical radius in which players will be affected by the Bard trait. (Min: 0.0, Max: 64.0)";

        public const string HORIZONTAL_EFFECT_RADIUS_COMMENT =
            "The horizontal radius in which players will be affected by the Bard trait. (Min: 0.0, Max: 128.0)";

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
                .RegisterMessageType(typeof(Configuration));
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

            ValidateConfig();

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

        private void ValidateConfig()
        {
            Mod.Logger.Notification("Validating configuration for Balladeer...");

            if (Configuration.ActivationPerSeconds < 1)
            {
                Mod.Logger.Warning($"Warning: {nameof(Configuration.ActivationPerSeconds)} out of range (min 1). Fixing.");
                Configuration.ActivationPerSeconds = 1;
            }

            if (Configuration.EffectSettings.EffectDurationSeconds < 1)
            {
                Mod.Logger.Warning($"Warning: {nameof(Configuration.EffectSettings)}.{nameof(Configuration.EffectSettings.EffectDurationSeconds)} out of range (min 1). Fixing.");
                Configuration.EffectSettings.EffectDurationSeconds = 1;
            }

            if (Configuration.EffectSettings.EffectTier is < 1 or > 3)
            {
                Mod.Logger.Warning($"Warning: {nameof(Configuration.EffectSettings)}.{nameof(Configuration.EffectSettings.EffectTier)} out of range (min 1, max 3). Clamping.");
                Configuration.EffectSettings.EffectTier =
                    Math.Max(1, Math.Min(3, Configuration.EffectSettings.EffectTier));
            }

            if (Configuration.EffectRadius.Horizontal is < 0f or > 128f)
            {
                Mod.Logger.Warning($"Warning: {nameof(Configuration.EffectRadius)}.{nameof(Configuration.EffectRadius.Horizontal)} is out of range (min 0.0, max 128.0). Clamping.");
                Configuration.EffectRadius.Horizontal =
                    Math.Max(0f, Math.Min(128f, Configuration.EffectRadius.Horizontal));
            }

            if (Configuration.EffectRadius.Vertical is < 0f or > 64f)
            {
                Mod.Logger.Warning($"Warning: {nameof(Configuration.EffectRadius)}.{nameof(Configuration.EffectRadius.Vertical)} is out of range (min 0.0, max 64.0). Clamping.");
                Configuration.EffectRadius.Vertical =
                    Math.Max(0f, Math.Min(64f, Configuration.EffectRadius.Vertical));
            }

            // Reset comments
            Configuration.__ActivationPerSeconds_Comment = ACTIVATION_PER_SECONDS_COMMENT;
            Configuration.EffectSettings.__EffectDurationSeconds_Comment = EFFECT_DURATION_SECONDS_COMMENT;
            Configuration.EffectSettings.__EffectTier_Comment = EFFECT_TIER_COMMENT;
            Configuration.EffectRadius.__Horizontal_Comment = HORIZONTAL_EFFECT_RADIUS_COMMENT;
            Configuration.EffectRadius.__Vertical_Comment = VERTICAL_EFFECT_RADIUS_COMMENT;
        }
    }
}