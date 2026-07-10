using System;
using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropertyOOEPlayActionsetAnim : MsgPropertyOOE
    {
        public short Unknown1;
        public short Unknown2;
        public short Unknown3;
        public uint EntityUID;
        public string Animation;

        internal override void Read(DataReader reader, string[] stringTable)
        {
            Unknown1 = reader.ReadInt16();
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadInt16();
            Animation = stringTable[reader.ReadUInt16()];
            reader.Stream.Position += 2;
            EntityUID = reader.ReadUInt32();
        }

        internal override void Write(DataWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(m_writeStringIndices[0]);
            writer.WriteTimes(0, 2);
            writer.Write(EntityUID);
        }

        internal override void WriteStrings(DataWriter writer, StringTableIndexed table)
        {
            m_writeStringIndices[0] = table.Write(Animation);
        }

        internal override List<string> GetStrings()
        {
            return new List<string>() { Animation };
        }
    }
}
