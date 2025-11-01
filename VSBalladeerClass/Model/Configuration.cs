using ProtoBuf;

namespace VSBalladeerClass.Model
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class Configuration
    {

        public string __ActivationPerSeconds_Comment = BalladeerModCommon.ACTIVATION_PER_SECONDS_COMMENT;
        public int ActivationPerSeconds = 3;

        public BalladeerEffectSettings EffectSettings = new();

        public BalladeerEffectRadius EffectRadius = new();
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class BalladeerEffectSettings
    {
        public string __EffectDurationSeconds_Comment = BalladeerModCommon.EFFECT_DURATION_SECONDS_COMMENT;
        public int EffectDurationSeconds = 7;

        public string __EffectTier_Comment = BalladeerModCommon.EFFECT_TIER_COMMENT;
        public int EffectTier = 1;
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class BalladeerEffectRadius
    {
        public string __Vertical_Comment = BalladeerModCommon.VERTICAL_EFFECT_RADIUS_COMMENT;
        public float Vertical = 5f;

        public string __Horizontal_Comment = BalladeerModCommon.HORIZONTAL_EFFECT_RADIUS_COMMENT;
        public float Horizontal = 20.5f;
    }
}