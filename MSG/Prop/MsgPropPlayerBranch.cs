using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropPlayerBranch : MsgPropBranching
    {
        public byte PlayerID;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadByte();
            PlayerID = reader.ReadByte();
            m_branchEventID = reader.ReadByte();
        }

        internal override void PostRead(MsgGroup parentGroup, MsgEvent parentEvent)
        {
            base.PostRead(parentGroup, parentEvent);

            BranchingEvent = parentGroup.Events[m_branchEventID];
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            byte parentIdx = (byte)parentGroup.Events.IndexOf(BranchingEvent);
            writer.Write(parentIdx);
        }
    }
}
