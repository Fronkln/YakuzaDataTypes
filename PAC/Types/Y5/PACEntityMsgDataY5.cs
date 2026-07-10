using System;
using System.Collections.Generic;

namespace YakuzaDataTypes.PAC
{
    [Serializable]
    public struct PACEntityMsgDataY5
    {
        public byte[] Identifier;
        public List<MSG.MsgPositionData> Positions;
        public string[] Strings;

        public List<PACMsgGroup> Groups;
    }
}
