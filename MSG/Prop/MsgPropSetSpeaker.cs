using System;
using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropSetSpeaker : MsgProp
    {
        public byte[] Unk1 = new byte[6];
        public string SpeakerName = "";
        public ushort Unk2;
        public int EntityUID;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadBytes(6);
            int speakerID = reader.ReadInt16();

            if (speakerID != -1)
                SpeakerName = stringTable[speakerID];
            else
                SpeakerName = "(NULL)";

            Unk2 = reader.ReadUInt16();
            EntityUID = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(m_writeStringIndices[0]);
            writer.Write(Unk2);
            writer.Write(EntityUID);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { SpeakerName };
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            if (SpeakerName == "(NULL)")
                m_writeStringIndices[0] = -1;
            else
                m_writeStringIndices[0] = table.Write(SpeakerName);             
        }
    }
}
