using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropScenarioBranching : MsgProp
    {
        public byte Unk1;
        public byte Unk2;
        private byte m_branchEventID; //only used while reading

        public MsgEvent BranchingEvent;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadByte();
            Unk2 = reader.ReadByte();
            m_branchEventID = reader.ReadByte();
        }

        internal override void PostRead(MsgGroup parentGroup, MsgEvent parentEvent)
        {
            base.PostRead(parentGroup, parentEvent);

            try
            {
                BranchingEvent = parentGroup.Events[m_branchEventID];
            }
            catch
            {
                BranchingEvent = null;
            }
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(Unk2);
            byte parentIdx = (byte)parentGroup.Events.IndexOf(BranchingEvent);
            writer.Write(parentIdx);
        }
    }
}
