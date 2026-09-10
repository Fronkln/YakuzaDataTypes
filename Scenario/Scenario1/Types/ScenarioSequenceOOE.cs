using System;
using System.Collections.Generic;
using System.Linq;
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
            ScenarioEventOOE sEvent = null;

            switch (category)
            {
                default:
                    throw new Exception($"Unknown event type: {category.ToString()} at {Name} on position: {reader.Stream.Position}");
                case 0:
                    sEvent = new ScenarioEvent0();
                    break;
                case 1:
                    sEvent = new ScenarioEventBGM();
                    break;
                case 2:
                    sEvent = new ScenarioEventSoundCuesheet();
                    break;
                case 3:
                    sEvent = new ScenarioEventBattle();
                    break;
                case 4:
                    sEvent = new ScenarioEventPreBattleCam();
                    break;
                case 5:
                    sEvent = new ScenarioEvent5();
                    break;
                case 6:
                    sEvent = new ScenarioEvent6();
                    break;
                case 7:
                    sEvent = new ScenarioEventTitleCard();
                    break;
                case 9:
                    sEvent = new ScenarioEvent9();
                    break;
                case 10:
                    sEvent = new ScenarioEvent10();
                    break;
                case 11:
                    sEvent = new ScenarioEventStage();
                    break;
                case 12:
                    sEvent = new ScenarioEvent12();
                    break;
                case 13:
                    sEvent = new ScenarioEvent13();
                    break;
                case 14:
                    sEvent = new ScenarioEvent13();
                    break;
                case 15:
                    sEvent = new ScenarioEvent15();
                    break;
                case 17:
                    sEvent = new ScenarioEvent17();
                    break;
                case 18:
                    sEvent = new ScenarioEventNext();
                    break;
                case 19:
                    sEvent = new ScenarioEvent19();
                    break;
                case 20:
                    sEvent = new ScenarioEventPIBArchive();
                    break;
                case 23:
                    sEvent = new ScenarioEvent23();
                    break;
                case 26:
                    sEvent = new ScenarioEvent26();
                    break;
                case 27:
                    sEvent = new ScenarioEventUSM();
                    break;
                case 28:
                    sEvent = new ScenarioEvent28();
                    break;
                case 30:
                    sEvent = new ScenarioEvent30();
                    break;
                case 31:
                    sEvent = new ScenarioEvent31();
                    break;
                case 32:
                    sEvent = new ScenarioEvent32();
                    break;
                case 33:
                    sEvent = new ScenarioEvent33();
                    break;
                case 34:
                    sEvent = new ScenarioEvent34();
                    break;
                case 35:
                    sEvent = new ScenarioEvent35();
                    break;
                case 36:
                    sEvent = new ScenarioEvent36();
                    break;
                case 37:
                    sEvent = new ScenarioEventHAct();
                    break;
                case 38:
                    sEvent = new ScenarioEvent38();
                    break;
                case 39:
                    sEvent = new ScenarioEvent39();
                    break;
                case 40:
                    sEvent = new ScenarioEvent40();
                    break;
                case 41:
                    sEvent = new ScenarioEvent41();
                    break;
                case 42:
                    sEvent = new ScenarioEvent42();
                    break;
                case 43:
                    sEvent = new ScenarioEvent43();
                    break;
                case 45:
                    sEvent = new ScenarioEvent45();
                    break;
                case 46:
                    sEvent = new ScenarioEvent46();
                    break;
                case 47:
                    sEvent = new ScenarioEvent47();
                    break;
                case 48:
                    sEvent = new ScenarioEvent48();
                    break;
                case 49:
                    sEvent = new ScenarioEvent49();
                    break;

                case 50:
                    sEvent = new ScenarioEvent50();
                    break;
            }

            sEvent.ReadData(reader);
            return sEvent;
        }

        private ScenarioEventOOE ReadY4Event(DataReader reader, ushort category)
        {
            ScenarioEventOOE sEvent = null;

            switch (category)
            {
                default:
                    throw new Exception($"Unknown event type: {category.ToString()} at {Name} on position: {reader.Stream.Position}");
                case 0:
                    sEvent = new ScenarioEventAuthUSM();
                    break;
                case 1:
                    sEvent = new ScenarioEventBGM();
                    break;
                case 3:
                    sEvent = new ScenarioEventBattle();
                    break;
                case 4:
                    sEvent = new ScenarioEventPreBattleCam();
                    break;
                case 5:
                    sEvent = new ScenarioEvent5();
                    break;
                case 6:
                    sEvent = new ScenarioEvent6();
                    break;
                case 7:
                    sEvent = new ScenarioEventTitleCard();
                    break;
                case 9:
                    sEvent = new ScenarioEvent9();
                    break;
                case 10:
                    sEvent = new ScenarioEvent10();
                    break;
                case 11:
                    sEvent = new ScenarioEventStage();
                    break;
                case 12:
                    sEvent = new ScenarioEvent12();
                    break;
                case 13:
                    sEvent = new ScenarioEvent13();
                    break;
                case 14:
                    sEvent = new ScenarioEvent14();
                    break;
                case 15:
                    sEvent = new ScenarioEvent15Y4();
                    break;
                case 17:
                    sEvent = new ScenarioEvent17();
                    break;
                case 18:
                    sEvent = new ScenarioEventNext();
                    break;
                case 19:
                    sEvent = new ScenarioEvent19();
                    break;
                case 20:
                    sEvent = new ScenarioEventPIBArchive();
                    break;
                case 23:
                    sEvent = new ScenarioEvent23();
                    break;
                case 24:
                    sEvent = new ScenarioEvent24();
                    break;
                case 25:
                    sEvent = new ScenarioEvent25();
                    break;
                case 26:
                    sEvent = new ScenarioEvent26();
                    break;
                case 27:
                    sEvent = new ScenarioEventUSM();
                    break;
                case 28:
                    sEvent = new ScenarioEvent28();
                    break;
                case 30:
                    sEvent = new ScenarioEvent30();
                    break;
                case 32:
                    sEvent = new ScenarioEvent32();
                    break;
                case 34:
                    sEvent = new ScenarioEvent34();
                    break;
                case 35:
                    sEvent = new ScenarioEvent35();
                    break;
                case 37:
                    sEvent = new ScenarioEventHActY4();
                    break;
                case 38:
                    sEvent = new ScenarioEvent38();
                    break;
                case 39:
                    sEvent = new ScenarioEvent39();
                    break;
                case 40:
                    sEvent = new ScenarioEvent40();
                    break;
                case 41:
                    sEvent = new ScenarioEvent41();
                    break;
                case 42:
                    sEvent = new ScenarioEvent42Y4();
                    break;
                case 43:
                    sEvent = new ScenarioEvent43Y4();
                    break;
                case 45:
                    sEvent = new ScenarioEvent45();
                    break;
                case 46:
                    sEvent = new ScenarioEvent46();
                    break;
                case 47:
                    sEvent = new ScenarioEvent47();
                    break;
                case 48:
                    sEvent = new ScenarioEvent48();
                    break;
                case 49:
                    sEvent = new ScenarioEvent49();
                    break;
                case 50:
                    sEvent = new ScenarioEvent50();
                    break;
                case 51:
                    sEvent = new ScenarioEvent51();
                    break;
                case 52:
                    sEvent = new ScenarioEvent52();
                    break;
                case 53:
                    sEvent = new ScenarioEvent53();
                    break;
                case 54:
                    sEvent = new ScenarioEvent54();
                    break;
                case 55:
                    sEvent = new ScenarioEvent55();
                    break;
                case 56:
                    sEvent = new ScenarioEvent56();
                    break;
                case 59:
                    sEvent = new ScenarioEvent59();
                    break;
            }

            sEvent.ReadData(reader);
            return sEvent;
        }
    }
}
