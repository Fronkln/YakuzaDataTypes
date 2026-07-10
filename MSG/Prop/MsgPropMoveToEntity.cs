using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropMoveToEntity : MsgProp
    {
        public byte Unk1;
        public byte Unk2;
        public short Unk3;
        public short Unk4;
        public short Unk5;
        public int EntityToWalkTo;
        public int EntityUID;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadByte();
            Unk2 = reader.ReadByte();
            Unk3 = reader.ReadInt16();
            Unk4 = reader.ReadInt16();
            EntityToWalkTo = reader.ReadInt32();
            EntityUID = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(Unk3);
            writer.Write(Unk4);
            writer.Write(EntityToWalkTo);
            writer.Write(EntityUID);
        }
    }
}
