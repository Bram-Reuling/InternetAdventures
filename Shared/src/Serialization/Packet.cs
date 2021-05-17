// Starter class from the handouts of the Networking course from CMGT Y2T3

using System;
using System.IO;

namespace Shared.Serialization
{
    /**
	 * The Packet class provides a simple wrapper around an array of bytes (in the form of a MemoryStream), 
	 * that allows us to write/read values to/from the Packet easily. 
	 */
    public class Packet
    {
        #region Variables

        private BinaryWriter writer; // Used to write bytes into a byte array.
        private BinaryReader reader; // Used to read bytes from a byte array.

        #endregion

        #region Constructors

        // Create packet for writing
        public Packet()
        {
            writer = new BinaryWriter(new MemoryStream());
        }

        // Create packet from existing byte array for reading
        public Packet(byte[] pSource)
        {
            reader = new BinaryReader(new MemoryStream(pSource));
        }

        #endregion

        #region Write Methods

        public void Write (int pInt)							{		writer.Write(pInt);			}
        public void Write (string pString)						{		writer.Write(pString);		}
        public void Write (bool pBool)							{		writer.Write(pBool);		}
		
        public void Write (ASerializable pSerializable)			{
            //write the full classname into the stream first
            Write(pSerializable.GetType().FullName);
            //then ask the serializable object to serialize itself
            pSerializable.Serialize(this); 
        }

        #endregion

        #region Read Methods

        public int ReadInt() { return reader.ReadInt32(); }
        public string ReadString() { return reader.ReadString(); }
        public bool ReadBool() { return reader.ReadBoolean(); }

        public ASerializable ReadObject() 
        {
            //get the classname from the stream first
            Type type = Type.GetType(ReadString());
            //create an instance of it through reflection (requires default constructor)
            ASerializable obj = (ASerializable)Activator.CreateInstance(type);
            obj.Deserialize(this);
            return obj;
        }

        /**
		 * Convenience method to read AND cast an object in one go.
		 */
        public T Read<T>() where T:ASerializable
        {
            return (T)ReadObject();
        }

        #endregion

        #region Other

        /**
		 * Return the bytes that have been written into this Packet.
		 * Only works in Write mode.
		 */
        public byte[] GetBytes()
        {
            //If we opened the Packet in writing mode, we'll probably need to send it at some point.
            //MemoryStream can either return the whole buffer or simply the part of the buffer that has been filled,
            //which is what we do here using ToArray()
            return ((MemoryStream)writer.BaseStream).ToArray();
        }

        /**
		 * Helper method to find out if the Packet has more data to read.
		 */
        public bool HasMoreData()
        {
            if (reader == null) return false;

            MemoryStream memoryStream = (MemoryStream)reader.BaseStream;
            return memoryStream.Position < memoryStream.Length;
        }

        #endregion
    }
}