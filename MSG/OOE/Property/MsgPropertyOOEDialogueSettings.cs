using System;
using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropertyOOEDialogueSettings : MSGPropertyOOESpeechPause
    {
        public int Unknown1;
        public short TextLength;
        public short Length;

        internal override void Read(DataReader reader, string[] stringTable)
        {
            Unknown1 = reader.ReadInt32();
            TextLength = reader.ReadInt16();
            Length = reader.ReadInt16();
        }

        internal override void Write(DataWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(TextLength);
            writer.Write(Length);
        }
    }
}
