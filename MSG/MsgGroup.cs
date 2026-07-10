using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YakuzaDataTypes.MSG
{
    public class MsgGroup
    {
        public short Flag1;
        public int Flag2;

        public List<MsgEvent> Events = new List<MsgEvent>();
    }
}
