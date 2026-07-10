using System;
using System.Collections.Generic;

namespace YakuzaDataTypes.MSG
{
    public class MsgGroupOOE
    {
        public short Unknown1;
        public byte Unknown2;
        public byte Unknown3;
        public short Unknown4;
        public byte Unknown5;

        public List<MsgSubGroupOOE> Subgroups = new List<MsgSubGroupOOE>();
    }
}
