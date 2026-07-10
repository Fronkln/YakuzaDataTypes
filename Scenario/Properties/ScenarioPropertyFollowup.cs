using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyFollowup : ScenarioProperty
    {
        public short ChapterStateID;
        public short ScenarioStateID;
        public int FollowType;
        public int NextNode;

        internal override int GetSize()
        {
            return 12;
        }

        internal override void Read(DataReader reader)
        {
            ChapterStateID = reader.ReadInt16();
            ScenarioStateID = reader.ReadInt16();
            FollowType = reader.ReadInt32();
            NextNode = reader.ReadInt32();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);

            writer.Write(ChapterStateID);
            writer.Write(ScenarioStateID);
            writer.Write(FollowType);
            writer.Write(NextNode);
        }
    }
}
