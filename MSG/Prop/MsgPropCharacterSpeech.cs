using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropCharacterSpeech : MsgProp
    {
        public string Speech;

        public int Unknown;
        public short Unknown2;
        public int EntityUID;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unknown = reader.ReadInt32();
            Speech = stringTable[reader.ReadInt32()];
            Unknown2 = reader.ReadInt16();
            EntityUID = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unknown);
            writer.Write((int)m_writeStringIndices[0]);
            writer.Write(Unknown2);
            writer.Write(EntityUID);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            m_writeStringIndices[0] = table.Write(Speech);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { Speech };
        }
    }
}
