using System;
using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropertyOOESetSpeaker : MsgPropertyOOE
    {
        public int Unknown1;
        public short Unknown2;
        public string Speaker;
        public short Unknown3;
        public uint EntityUID;

        internal override void Read(DataReader reader, string[] stringTable)
        {
            Unknown1 = reader.ReadInt32();
            Unknown2 = reader.ReadInt16();

            short speakerIdx = reader.ReadInt16();

            if (speakerIdx < 0)
                Speaker = "(NULL)";
            else
                Speaker = stringTable[speakerIdx];

            Unknown3 = reader.ReadInt16();
            EntityUID = reader.ReadUInt32();
        }

        internal override void Write(DataWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(m_writeStringIndices[0]);
            writer.Write(Unknown3);
            writer.Write(EntityUID);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            if (Speaker != "(NULL)")
                m_writeStringIndices[0] = table.Write(Speaker);
            else
                m_writeStringIndices[0] = -1;
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { Speaker };
        }
    }
}
