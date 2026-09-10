using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventSoundCuesheet : ScenarioEventOOE
    {
        public string Cuesheet;

        internal override void ReadData(DataReader reader)
        {
            Cuesheet = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(Cuesheet));
        }

        public override ushort GetCategory()
        {
            return 2;
        }
    }
}
