using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace YakuzaDataTypes.MSG
{
    public class MsgSubGroupOOE
    {
        public List<MsgEventOOE> Events = new List<MsgEventOOE>();

        public int Unknown1;
        public short Unknown2;
        public short Unknown3;
        public short Unknown4;
    }
}
