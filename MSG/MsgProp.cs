using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgProp
    {
        public ushort PropType;

        public byte[] UnreadData = null;

        protected short[] m_writeStringIndices = new short[6];

        internal virtual void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
        }

        internal virtual void PostRead(MsgGroup parentGroup, MsgEvent parentEvent)
        {

        }

        internal virtual void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            if (UnreadData.Length != 14)
                throw new NotImplementedException();
        }

        internal virtual void WriteStrings(DataWriter writer, StringTableIndexed table)
        {

        }

        /// <summary>
        /// only used to determine string table size
        /// </summary>
        /// <returns></returns>
        internal virtual List<string> GetStrings()
        {
            return new List<string>();
        }
    }
}
