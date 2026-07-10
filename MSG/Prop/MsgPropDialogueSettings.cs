using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropDialogueSettings : MsgProp
    {
        public short Unk1;
        public short Unk2;
        public short TextLength;
        public short Duration;
        public short Unk3;
        public int EntityUID;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadInt16();
            Unk2 = reader.ReadInt16();
            TextLength = reader.ReadInt16();
            Duration = reader.ReadInt16();
            Unk3 = reader.ReadInt16();
            EntityUID = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(TextLength);
            writer.Write(Duration);
            writer.Write(Unk3);
            writer.Write(EntityUID);
        }
    }
}
