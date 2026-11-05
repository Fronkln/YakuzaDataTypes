using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioSequenceOOE
    {
        public string Name = "SEQUENCE";
        public int Type = 3;
        public int MissionID = 300;
        public string ChapterCode = null;
        public int Unknown1;

        public List<ScenarioEventOOE> Events = new List<ScenarioEventOOE>();

        public int Unknown2;
        public int PlayerPosID = -1;
        public int Unknown3 = -1;

        public int[] UnkDat;

        public override string ToString()
        {
            return Name;
        }

        public ScenarioEventOOE[] GetScenarioEventsByCategory(ushort category)
        {
            return Events.Where(x => x.GetCategory() == category).ToArray();
        }

        internal void ReadUnkDat(DataReader reader, SizedPointer ptr, ScenarioVersion version)
        {
            reader.Stream.Seek(ptr.Pointer);
            UnkDat = new int[ptr.Size];

            for (int i = 0; i < UnkDat.Length; i++)
                UnkDat[i] = reader.ReadInt32();
        }

        internal void ReadEvents(DataReader reader, SizedPointer ptr, ScenarioVersion version)
        {
            reader.Stream.Seek(ptr.Pointer);

            Events = new List<ScenarioEventOOE>();

            long limit = reader.Stream.Position + (ptr.Size * 4);

            for (int i = 0; i < ptr.Size && reader.Stream.Position < limit; i++)
            {
                ushort start = reader.ReadUInt16();
                ushort category = reader.ReadUInt16();

                if (version == ScenarioVersion.Y3)
                    Events.Add(ReadY3Event(reader, category));
                else
                    Events.Add(ReadY4Event(reader, category));
            }
        }

        private ScenarioEventOOE ReadY3Event(DataReader reader, ushort category)
        {
            ScenarioEventOOE sEvent = category switch {
                0 => new ScenarioEvent0(),
                1 => new ScenarioEventBGM(),
                2 => new ScenarioEventSoundCuesheet(),
                3 => new ScenarioEventBattle(),
                4 => new ScenarioEventPreBattleCam(),
                5 => new ScenarioEvent5(),
                6 => new ScenarioEvent6(),
                7 => new ScenarioEventTitleCard(),
                9 => new ScenarioEvent9(),
                10 => new ScenarioEvent10(),
                11 => new ScenarioEventStage(),
                12 => new ScenarioEvent12(),
                13 => new ScenarioEvent13(),
                14 => new ScenarioEvent13(),
                15 => new ScenarioEvent15(),
                17 => new ScenarioEvent17(),
                18 => new ScenarioEventNext(),
                19 => new ScenarioEvent19(),
                20 => new ScenarioEventPIBArchive(),
                23 => new ScenarioEvent23(),
                26 => new ScenarioEvent26(),
                27 => new ScenarioEventUSM(),
                28 => new ScenarioEvent28(),
                30 => new ScenarioEvent30(),
                31 => new ScenarioEvent31(),
                32 => new ScenarioEvent32(),
                33 => new ScenarioEvent33(),
                34 => new ScenarioEvent34(),
                35 => new ScenarioEvent35(),
                36 => new ScenarioEvent36(),
                37 => new ScenarioEventHAct(),
                38 => new ScenarioEvent38(),
                39 => new ScenarioEvent39(),
                40 => new ScenarioEvent40(),
                41 => new ScenarioEvent41(),
                42 => new ScenarioEvent42(),
                43 => new ScenarioEvent43(),
                45 => new ScenarioEvent45(),
                46 => new ScenarioEvent46(),
                47 => new ScenarioEvent47(),
                48 => new ScenarioEvent48(),
                49 => new ScenarioEvent49(),
                50 => new ScenarioEvent50(),
                _ => throw new Exception($"Unknown event type: {category.ToString()} at {Name} on position: {reader.Stream.Position}")
            };

            sEvent.ReadData(reader);
            return sEvent;
        }

        private ScenarioEventOOE ReadY4Event(DataReader reader, ushort category)
        {
            ScenarioEventOOE sEvent = category switch {
                0 => new ScenarioEventAuthUSM(),
                1 => new ScenarioEventBGM(),
                3 => new ScenarioEventBattle(),
                4 => new ScenarioEventPreBattleCam(),
                5 => new ScenarioEvent5(),
                6 => new ScenarioEvent6(),
                7 => new ScenarioEventTitleCard(),
                9 => new ScenarioEvent9(),
                10 => new ScenarioEvent10(),
                11 => new ScenarioEventStage(),
                12 => new ScenarioEvent12(),
                13 => new ScenarioEvent13(),
                14 => new ScenarioEvent14(),
                15 => new ScenarioEvent15Y4(),
                17 => new ScenarioEvent17(),
                18 => new ScenarioEventNext(),
                19 => new ScenarioEvent19(),
                20 => new ScenarioEventPIBArchive(),
                23 => new ScenarioEvent23(),
                24 => new ScenarioEvent24(),
                25 => new ScenarioEvent25(),
                26 => new ScenarioEvent26(),
                27 => new ScenarioEventUSM(),
                28 => new ScenarioEvent28(),
                30 => new ScenarioEvent30(),
                32 => new ScenarioEvent32(),
                34 => new ScenarioEvent34(),
                35 => new ScenarioEvent35(),
                37 => new ScenarioEventHActY4(),
                38 => new ScenarioEvent38(),
                39 => new ScenarioEvent39(),
                40 => new ScenarioEvent40(),
                41 => new ScenarioEvent41(),
                42 => new ScenarioEvent42Y4(),
                43 => new ScenarioEvent43Y4(),
                45 => new ScenarioEvent45(),
                46 => new ScenarioEvent46(),
                47 => new ScenarioEvent47(),
                48 => new ScenarioEvent48(),
                49 => new ScenarioEvent49(),
                50 => new ScenarioEvent50(),
                51 => new ScenarioEvent51(),
                52 => new ScenarioEvent52(),
                53 => new ScenarioEvent53(),
                54 => new ScenarioEvent54(),
                55 => new ScenarioEvent55(),
                56 => new ScenarioEvent56(),
                59 => new ScenarioEvent59(),
                _ => throw new Exception($"Unknown event type: {category.ToString()} at {Name} on position: {reader.Stream.Position}")
            };

            sEvent.ReadData(reader);
            return sEvent;
        }
    }
}
