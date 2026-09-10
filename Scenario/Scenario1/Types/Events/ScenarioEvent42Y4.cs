using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    internal class ScenarioEvent42Y4 : ScenarioEventOOE
    {
        public int Unknown;
        public string UnknownString;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadInt32();
            UnknownString = reader.ReadStringPointer();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(0);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(UnknownString));
        }

        public override ushort GetCategory()
        {
            return 42;
        }
    }
}
