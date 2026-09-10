using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    internal class ScenarioEvent56 : ScenarioEventOOE
    {
        public int Unknown;
        public int Unknown1;
        public int Unknown2;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadInt32();
            Unknown1 = reader.ReadInt32();
            Unknown2 = reader.ReadInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
        }

        public override ushort GetCategory()
        {
            return 56;
        }
    }
}
