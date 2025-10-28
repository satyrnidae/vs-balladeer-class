using ProtoBuf;

namespace VSBalladeerClass.Model
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class Configuration
    {
        public BalladeerEffectRadius EffectRadius = new();
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class BalladeerEffectRadius
    {
        public float Vertical = 5f;

        public float Horizontal = 20.5f;
    }
}