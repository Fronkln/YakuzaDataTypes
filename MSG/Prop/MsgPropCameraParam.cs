using System;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgPropCameraParam : MsgProp
    {
        public Flag1 Flags1;
        public ushort Flags2;
        public ushort Unk1;

        public int IntVal1;
        public int IntVal2;

        public float FloatVal1;

        public enum Flag1
        {
            MoveCamera = 1,
            MoveCameraInterpolated = 4
        }

        internal override void ReadData(DataReader reader, string[] stringTable, bool isOE)
        {
            Flags1 = (Flag1)reader.ReadUInt16();
            Flags2 = reader.ReadUInt16();
            Unk1 = reader.ReadUInt16();

            if (((ushort)Flags1 & 49152) != 0)
            {
                IntVal1 = reader.ReadInt32();
                IntVal2 = reader.ReadInt32();
            }
            else
            {
                IntVal1 = reader.ReadInt16();
                IntVal2 = reader.ReadInt16();
            }
            //Interpolated camera:
            if (Flags1.HasFlag(Flag1.MoveCameraInterpolated))
            {
               // System.Diagnostics.Debug.WriteLine("find the beat and express yourself " + (int)Flags1 + " " + (int)Flags2 + " " + reader.Stream.Position);

                //Duration
                if(Flags2 <= 0)
                    FloatVal1 = reader.ReadSingle();
            }
        }

        internal override void WriteData(MsgGroup parentGroup, MsgEvent parentEvent, DataWriter writer, bool isOE)
        {
            writer.Write((ushort)Flags1);
            writer.Write(Flags2);
            writer.Write(Unk1);

            if (((ushort)Flags1 & 49152) != 0)
            {
                writer.Write(IntVal1);
                writer.Write(IntVal2);
            }
            else
            {
                writer.Write((ushort)IntVal1);
                writer.Write((ushort)IntVal2);
            }

            if (Flags1.HasFlag(Flag1.MoveCameraInterpolated))
            {
                writer.Write(FloatVal1);
            }
        }
    }
}
