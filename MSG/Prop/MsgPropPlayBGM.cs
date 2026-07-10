using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    //uid00xxxxxx\uid000301ca.msg y0  right after Bye! does it stop the soundtrack
    public class MsgPropPlayBGM : MsgProp
    {
        public int Unk1;
        public int Unk2;
        public string BGM;
        public float Unk3;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            BGM = stringTable[reader.ReadUInt16()];
            Unk3 = reader.ReadSingle();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(m_writeStringIndices[0]);
            writer.Write(Unk3);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            m_writeStringIndices[0] = table.Write(BGM);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { BGM };
        }
    }
}
