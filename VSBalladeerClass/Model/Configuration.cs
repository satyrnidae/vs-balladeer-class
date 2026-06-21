using ProtoBuf;

namespace VSBalladeerClass.Model;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class Configuration
{

    public string __ActivationPerSeconds_Comment = BalladeerModCommon.ACTIVATION_PER_SECONDS_COMMENT;
    public int ActivationPerSeconds = 3;

    public BalladeerEffectSettings EffectSettings = new();

    public BalladeerEffectRadius EffectRadius = new();

    public BalladeerTraitsConfig Traits = new();

    public BalladeerInstrumentsConfig Instruments = new();
}

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class BalladeerInstrumentsConfig
{
    public InstrumentFlags BoneFlute = new();
    public InstrumentFlags BaconFlute = new();
    public InstrumentFlags FrogGuiro = new();
}

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class InstrumentFlags
{
    public bool Enabled = true;
    public bool RequireBardTraitToCraft = true;
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

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class BalladeerTraitsConfig
{
    public string __Claustrophobic_Comment = BalladeerModCommon.CLAUSTROPHOBIC_COMMENT;
    public bool Claustrophobic = true;

    public string __Overconfident_Comment = BalladeerModCommon.OVERCONFIDENT_COMMENT;
    public bool Overconfident = true;

    public string __Brash_Comment = BalladeerModCommon.BRASH_COMMENT;
    public bool Brash = true;

    public string __Frail_Comment = BalladeerModCommon.FRAIL_COMMENT;
    public bool Frail = true;
}
