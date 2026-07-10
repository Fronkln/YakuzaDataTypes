using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YakuzaDataTypes.MSG
{
    public class MsgEvent
    {
        public string Value = "";
        public List<MsgProp> Properties = new List<MsgProp>();
    }
}
