using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropChoice : MsgProp
    {
        public ushort Unk1;
        public int Unk2;
        public string Choice;
        public short Unk3;
        public int Unk4;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadUInt16();
            Unk2 = reader.ReadInt32();
            Choice = stringTable[reader.ReadInt16()];
            Unk3 = reader.ReadInt16();
            Unk4 = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(m_writeStringIndices[0]);
            writer.Write(Unk3);
            writer.Write(Unk4);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            m_writeStringIndices[0] = table.Write(Choice);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { Choice };
        }
    }
}
