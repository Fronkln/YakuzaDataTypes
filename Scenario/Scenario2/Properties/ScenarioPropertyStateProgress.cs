using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyStateProgress : ScenarioProperty
    {
        public short StateGroupID;
        public short StateID;
        public int Complete;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);

            StateGroupID = reader.ReadInt16();
            StateID = reader.ReadInt16();
            Complete = reader.ReadInt32();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);

            writer.Write(StateGroupID);
            writer.Write(StateID);
            writer.Write(Complete);
        }

        internal override int GetSize()
        {
            return 8;
        }
    }
}
