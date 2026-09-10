using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventTitleCard : ScenarioEventOOE
    {
        public int Unknown;
        public int Unknown2;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadInt32();
            Unknown2 = reader.ReadInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(Unknown2);
        }
        public override ushort GetCategory()
        {
            return 7;
        }

    }
}
