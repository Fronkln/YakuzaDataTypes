using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropPlayHAct : MsgProp
    {
        public short Unk1;
        public int Unk2;
        public short Unk3;
        public string HAct;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadInt16();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadInt16();
            HAct = stringTable[reader.ReadInt16()];
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(Unk3);
            writer.Write(m_writeStringIndices[0]);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            m_writeStringIndices[0] = table.Write(HAct);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { HAct };
        }
    }
}
