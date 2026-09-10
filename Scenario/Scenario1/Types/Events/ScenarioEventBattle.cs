using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventBattle : ScenarioEventOOE
    {
        public string DisposeBin;

        internal override void ReadData(DataReader reader)
        {
            DisposeBin = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(DisposeBin));
        }

        public override ushort GetCategory()
        {
            return 3;
        }
    }
}
