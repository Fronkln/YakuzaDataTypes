using System;
using System.IO;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventHActY4 : ScenarioEventOOE
    {
        public string HAct;

        internal override void ReadData(DataReader reader)
        {
            HAct = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(HAct));
        }

        public override ushort GetCategory()
        {
            return 37;
        }
    }
}
