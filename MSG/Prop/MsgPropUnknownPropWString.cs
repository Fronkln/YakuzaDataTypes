using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    //we dont know what it does but i included here because
    //to properly write msgs. we need to read all strings
    public class MsgPropUnknownPropWString : MsgProp
    {
        public ushort Unk1;
        public ushort Unk2;
        public int Unk3;
        public string UnknownString;
        
        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadUInt16();
            Unk2 = reader.ReadUInt16();
            Unk3 = reader.ReadInt32();
            UnknownString = stringTable[reader.ReadInt16()];
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
            m_writeStringIndices[0] = table.Write(UnknownString);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { UnknownString };
        }
    }
}
