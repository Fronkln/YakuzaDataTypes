using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropertyOOE
    {
        public MsgPropTypeOOE Type;

        public byte[] UnreadData = new byte[0];

        protected short[] m_writeStringIndices = new short[6];

        internal virtual void Read(DataReader reader, string[] stringTable)
        {

        }

        internal virtual void Write(DataWriter writer)
        {

        }

        internal virtual void WriteStrings(DataWriter writer, StringTableIndexed table)
        {

        }

        internal virtual List<string> GetStrings()
        {
            return new List<string>();
        }
    }
}
