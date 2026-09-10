using System;
using System.Collections.Generic;
using System.Linq;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioCollection
    {
        public List<ScenarioSequenceGroup> SequenceGroups = new List<ScenarioSequenceGroup>();

        public int Unknown1;
        public int Unknown2;

        public ScenarioSequenceGroup GetSequenceGroupByName(string name)
        {
            return SequenceGroups.FirstOrDefault(x => x.Name == name);
        }

        internal void ReadScenarioSequenceGroups(DataReader reader, SizedPointer ptr, ScenarioVersion version)
        {
            reader.Stream.Seek(ptr.Pointer);

            for(int i = 0; i < ptr.Size; i++)
            {
                ScenarioSequenceGroup group = new ScenarioSequenceGroup();
                group.Name = reader.ReadStringPointer();

                SizedPointer sequencesPtr = reader.Read<SizedPointer>();

                group.Unknown1 = reader.ReadInt32();
                group.Unknown2 = reader.ReadInt32();
                group.Unknown3 = reader.ReadInt32();
                group.Unknown4 = reader.ReadInt32();
                group.Unknown5 = reader.ReadInt32();

                reader.Stream.RunInPosition(delegate { group.ReadSequences(reader, sequencesPtr, version); }, 0);

                SequenceGroups.Add(group);
            }
        }
    }
}
