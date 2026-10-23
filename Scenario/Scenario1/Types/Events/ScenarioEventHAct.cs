using System;
using System.IO;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventHAct : ScenarioEventOOE
    {
        public uint HActID;

        internal override void ReadData(DataReader reader)
        {
            reader.ReadStringPointer();
            HActID = reader.ReadUInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(0);
            writer.Write(HActID);
        }

        internal override void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {
            writer.Write(ooeWriter.WriteToStringTable(HActID.ToString()));
        }

        public override ushort GetCategory()
        {
            return 37;
        }
    }
}
