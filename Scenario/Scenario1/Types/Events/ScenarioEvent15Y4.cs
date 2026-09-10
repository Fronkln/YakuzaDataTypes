using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEvent15Y4 : ScenarioEventOOE
    {
        public int Unknown;
        public int Unknown2;
        public int Unknown3;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadInt32();
            Unknown2 = reader.ReadInt32();
            Unknown3 = reader.ReadInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
        }

        public override ushort GetCategory()
        {
            return 15;
        }
    }
}
