using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropPlayDialogueSound : MsgProp
    {
        public byte Unk1;
        public string Sound;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadByte();
            Sound = stringTable[reader.ReadByte()];
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write((byte)m_writeStringIndices[0]);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            m_writeStringIndices[0] = table.Write(Sound);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { Sound };
        }
    }
}
