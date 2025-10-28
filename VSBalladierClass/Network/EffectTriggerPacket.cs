using ProtoBuf;
using Vintagestory.API.MathTools;

namespace VSBalladeerClass.Network
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class EffectTriggerPacket
    {
        public required Vec3d SourcePos;
    }
}