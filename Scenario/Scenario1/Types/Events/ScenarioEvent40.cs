using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    internal class ScenarioEvent40 : ScenarioEventOOE
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
            return 40;
        }
    }
}
