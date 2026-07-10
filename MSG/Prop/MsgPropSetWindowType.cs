using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropSetWindowType : MsgProp
    {
        public ushort WindowType;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            WindowType = reader.ReadUInt16();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(WindowType);
        }
    }
}
