using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventPIBArchive : ScenarioEventOOE
    {
        public string PIBArchive;

        internal override void ReadData(DataReader reader)
        {
            PIBArchive = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(PIBArchive));
        }

        public override ushort GetCategory()
        {
            return 20;
        }
    }
}
