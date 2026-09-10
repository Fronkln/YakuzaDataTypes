using System;
using System.Collections.Generic;
using System.Linq;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioSequenceGroupOOE
    {
        public string Name = "GROUP";

        public List<ScenarioSequenceOOE> Sequences = new List<ScenarioSequenceOOE>();

        public int Unknown1;
        public int Unknown2;
        public int Unknown3;
        public int Unknown4;
        public int Unknown5;

        public override string ToString()
        {
            return Name;
        }

        public ScenarioSequenceOOE GetScenarioSequenceByName(string name)
        {
            return Sequences.FirstOrDefault(x => x.Name == name);
        }

        internal void ReadSequences(DataReader reader, SizedPointer ptr, ScenarioVersion version)
        {
            reader.Stream.Seek(ptr.Pointer);

            Sequences = new List<ScenarioSequenceOOE>();

            for(int i = 0; i < ptr.Size; i++)
            {
                ScenarioSequenceOOE sequence = new ScenarioSequenceOOE();
                sequence.Type = reader.ReadInt32();
                sequence.Name = reader.ReadStringPointer();
                sequence.MissionID = reader.ReadInt32();
                sequence.ChapterCode = reader.ReadStringPointer();
                sequence.Unknown1 = reader.ReadInt32();

                SizedPointer eventsPtr = reader.Read<SizedPointer>();
                
                sequence.Unknown2 = reader.ReadInt32();
                sequence.PlayerPosID = reader.ReadInt32();
                sequence.Unknown3 = reader.ReadInt32();

                SizedPointer unkDat = reader.Read<SizedPointer>();

                reader.Stream.RunInPosition(delegate { sequence.ReadUnkDat(reader, unkDat, version); }, 0);
                reader.Stream.RunInPosition(delegate { sequence.ReadEvents(reader, eventsPtr, version); }, 0);

                Sequences.Add(sequence);
            }
        }
    }
}
