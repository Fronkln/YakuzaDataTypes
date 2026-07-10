using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropFaceExpression : MsgProp
    {
        public int Unk1;
        public short IfaID;
        public short IfaID2;
        public short Unk2;
        public int EntityUID;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadInt32();
            IfaID = reader.ReadInt16();
            IfaID2 = reader.ReadInt16();
            Unk2 = reader.ReadInt16();
            EntityUID = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(IfaID);
            writer.Write(IfaID2);
            writer.Write(Unk2);
            writer.Write(EntityUID);
        }
    }
}
