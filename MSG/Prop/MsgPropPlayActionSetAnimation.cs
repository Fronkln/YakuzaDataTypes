using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropPlayActionSetAnimation : MsgProp
    {
        public byte Unk1;
        public short TalkSound;
        public byte Unk3;
        public short Unk4;
        public string Animation1;
        public ushort Unk2;
        public int EntityUID;


        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadByte();
            TalkSound = reader.ReadInt16();
            Unk3 = reader.ReadByte();
            Unk4 = reader.ReadInt16();
            int animationID1 = reader.ReadInt16();

            if (animationID1 >= 0)
                Animation1 = stringTable[animationID1];
            else
                Animation1 = "(NULL)";

            Unk2 = reader.ReadUInt16();
            EntityUID = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(TalkSound);
            writer.Write(Unk3);
            writer.Write(Unk4);
            writer.Write(m_writeStringIndices[0]);
            writer.Write(Unk2);
            writer.Write(EntityUID);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            if (Animation1 == "(NULL)")
                m_writeStringIndices[0] = -1;
            else
                m_writeStringIndices[0] = table.Write(Animation1);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { Animation1 };
        }
    }
}
