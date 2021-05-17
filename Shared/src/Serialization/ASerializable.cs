// Starter class from the handouts of the Networking course from CMGT Y2T3

namespace Shared.Serialization
{
    /**
     * Classes that extend ASerializable can (de)serialize themselves into/out of a Packet instance. 
     * See the classes in the protocol package for an example. 
     * This base class provides a ToString method for simple (and slow) debugging.
     */
    public abstract class ASerializable
    {
        public abstract void Serialize(Packet pPacket);
        public abstract void Deserialize(Packet pPacket);
    }
}