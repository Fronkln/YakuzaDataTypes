using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropSpeechPause : MsgProp
    {
        public ushort PauseLength;
        public short Unk1;
        public ushort PausePosition;

        public byte[] Unk2 = new byte[8];

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            PauseLength = reader.ReadUInt16();
            Unk1 = reader.ReadInt16();
            PausePosition = reader.ReadUInt16();
            Unk2 = reader.ReadBytes(8);
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(PauseLength);
            writer.Write(Unk1);
            writer.Write(PausePosition);
            writer.Write(Unk2);
        }
    }
}
