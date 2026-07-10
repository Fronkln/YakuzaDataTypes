using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropSetVisibility : MsgProp
    {
        public ushort Invisible;
        public byte[] Unk1;
        public int EntityUID;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Invisible = reader.ReadUInt16();
            Unk1 = reader.ReadBytes(8);
            EntityUID = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Invisible);
            writer.Write(Unk1);
            writer.Write(EntityUID);
        }
    }
}
