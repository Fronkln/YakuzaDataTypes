using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropFaceEntity : MsgProp
    {
        public ushort Unk1;
        public float Unk2;
        public int EntityToFaceUID;
        public int Unk3;

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Unk1 = reader.ReadUInt16();
            Unk2 = reader.ReadSingle();
            EntityToFaceUID = reader.ReadInt32();
            Unk3 = reader.ReadInt32();
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write(Unk1);
            writer.Write(Unk2);
            writer.Write(EntityToFaceUID);
            writer.Write(Unk3);
        }
    }
}
