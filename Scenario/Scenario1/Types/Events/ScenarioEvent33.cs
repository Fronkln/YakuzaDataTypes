using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEvent33 : ScenarioEventOOE
    {
        public string Unknown;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadStringPointer();
        }
        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(Unknown));
        }

        public override ushort GetCategory()
        {
            return 33;
        }
    }
}
