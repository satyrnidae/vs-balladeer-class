using ProtoBuf;
using Vintagestory.API.MathTools;

namespace VSBalladeerClass.Network
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class EffectTriggerPacket
    {
        public required Vec3d SourcePos;
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ConfigurationSyncPacket
    {
        public required float EffectRadiusVertical;
        public required float EffectRadiusHorizontal;
    }
}