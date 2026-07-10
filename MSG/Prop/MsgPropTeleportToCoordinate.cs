using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropTeleportToCoordinate : MsgProp
    {
        public int Unk1;
        public int CoordinateID;
        public short Unk2;
        public int EntityUID;

        public byte OEUnk;
        public int OEUnk2;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            if (!isOE)
            {
                Unk1 = reader.ReadInt32();
                CoordinateID = reader.ReadInt32();
                Unk2 = reader.ReadInt16();
                EntityUID = reader.ReadInt32();
            }
            else
            {
                Unk1 = reader.ReadInt16();
                CoordinateID = reader.ReadByte();
                OEUnk = reader.ReadByte();
                Unk2 = reader.ReadInt16();
                OEUnk2 = reader.ReadInt32();
                EntityUID = reader.ReadInt32();
            }
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            if (!isOE)
            {
                writer.Write(Unk1);
                writer.Write(CoordinateID);
                writer.Write(Unk2);
                writer.Write(EntityUID);
            }
            else
            {
                writer.Write((short)Unk1);
                writer.Write((byte)CoordinateID);
                writer.Write(OEUnk);
                writer.Write(Unk2);
                writer.Write(OEUnk2);
                writer.Write(EntityUID);
            }
        }
    }
}
