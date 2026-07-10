using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MSGPropertyOOESpeechPause : MsgPropertyOOE
    {
        public ushort PauseLength;
        public ushort PausePosition;

        internal override void Read(DataReader reader, string[] stringTable)
        {
            PauseLength = reader.ReadUInt16();
            reader.Stream.Position += 2;
            PausePosition = reader.ReadUInt16();
            reader.Stream.Position += 8;
        }

        internal override void Write(DataWriter writer)
        {
            writer.Write(PausePosition);
            writer.WriteTimes(0, 2);
            writer.Write(PauseLength);
            writer.WriteTimes(0, 8);
        }
    }
}
