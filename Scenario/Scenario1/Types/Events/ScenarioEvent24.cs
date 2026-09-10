using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    internal class ScenarioEvent24 : ScenarioEventOOE
    {
        public int Unknown;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(Unknown);
        }

        public override ushort GetCategory()
        {
            return 24;
        }
    }
}
