using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEvent15 : ScenarioEventOOE
    {
        public string Unknown;
        public int Unknown2;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadStringPointer();
            Unknown2 = reader.ReadInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
            writer.Write(Unknown2);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(Unknown));
        }

        public override ushort GetCategory()
        {
            return 15;
        }
    }
}
