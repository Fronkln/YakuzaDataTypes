using System;
using System.Collections.Generic;

namespace YakuzaDataTypes.MSG
{
    public class MsgEventOOE
    {
        public string Text = "";
        public byte Unknown;
        public List<MsgPropertyOOE> Properties = new List<MsgPropertyOOE>();
    }
}
