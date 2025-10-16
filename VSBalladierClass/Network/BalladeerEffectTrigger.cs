using ProtoBuf;
using Vintagestory.API.MathTools;

namespace VSBalladeerClass.Network
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    internal class BalladeerEffectTrigger
    {
        public required Vec3d SourcePos;
    }
}