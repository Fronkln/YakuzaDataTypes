using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventPreBattleCam : ScenarioEventOOE
    {
        public string Camera;

        internal override void ReadData(DataReader reader)
        {
            Camera = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(Camera));
        }
        public override ushort GetCategory()
        {
            return 4;
        }
    }
}
