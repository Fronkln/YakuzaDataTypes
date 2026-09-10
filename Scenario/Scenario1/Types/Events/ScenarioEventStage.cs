using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventStage : ScenarioEventOOE
    {
        public int StageID;

        internal override void ReadData(DataReader reader)
        {
            StageID = reader.ReadInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(StageID);
        }

        public override ushort GetCategory()
        {
            return 11;
        }
    }
}
