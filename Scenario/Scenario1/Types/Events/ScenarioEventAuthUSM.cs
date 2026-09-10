using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    internal class ScenarioEventAuthUSM : ScenarioEventOOE
    {
        public int Unknown;
        public string USM;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadInt32();
            USM = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            ooeWriter.WriteToStringTable(USM);
        }

        public override ushort GetCategory()
        {
            return 0;
        }
    }
}
