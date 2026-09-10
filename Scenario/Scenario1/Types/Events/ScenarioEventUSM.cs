using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventUSM : ScenarioEventOOE
    {
        public string USM;

        internal override void ReadData(DataReader reader)
        {
            USM = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(USM));
        }

        public override ushort GetCategory()
        {
            return 27;
        }
    }
}
