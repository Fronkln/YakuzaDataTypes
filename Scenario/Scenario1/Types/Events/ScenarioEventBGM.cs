using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventBGM: ScenarioEventOOE
    {
        public string BGM;

        internal override void ReadData(DataReader reader)
        {
            BGM = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(BGM));
        }

        public override ushort GetCategory()
        {
            return 1;
        }
    }
}
