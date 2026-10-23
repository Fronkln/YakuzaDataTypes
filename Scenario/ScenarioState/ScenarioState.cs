using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Yarhl.IO;
using System.Linq;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioState
    {
        public List<ScenarioStateGroup> Groups = new List<ScenarioStateGroup>();
        public List<ScenarioStateChunk2> Chunk2 = new List<ScenarioStateChunk2>();
        public List<ScenarioStateTimer> Timers = new List<ScenarioStateTimer>();
        public List<ScenarioStateCounter> Counters = new List<ScenarioStateCounter>();
        public List<ScenarioStateEnum> Enums = new List<ScenarioStateEnum>();
        public List<int> TimerEnums = new List<int>();
        public List<int> CounterEnums = new List<int>();

        public static ScenarioState Read(string path)
        {
            var scenarioState = new ScenarioState();

            byte[] fileBuffer = File.ReadAllBytes(path);

            var readStream = DataStreamFactory.FromArray(fileBuffer, 0, fileBuffer.Length);
            var reader = new DataReader(readStream) { DefaultEncoding = Encoding.GetEncoding(932), Endianness = EndiannessMode.BigEndian };

            #region Read header
            int stateCategoryCount = reader.ReadInt32();
            int stateCategoryPtr = reader.ReadInt32();

            int part2Count = reader.ReadInt32();
            int part2Ptr = reader.ReadInt32();

            int timersCount = reader.ReadInt32();
            int timersPtr = reader.ReadInt32();

            int countersCount = reader.ReadInt32();
            int countersPtr = reader.ReadInt32();

            int enumsCount = reader.ReadInt32();
            int enumsPtr = reader.ReadInt32();

            int timerControllersCount = reader.ReadInt32();
            int timerControllersPtr = reader.ReadInt32();

            int counterEnumsCount = reader.ReadInt32();
            int counterEnumsPtr = reader.ReadInt32();

            int stateGroupsCount = reader.ReadInt32();
            int stateGroupsPtr = reader.ReadInt32();
            #endregion

            #region Read chunk 2
            //Read chunk 2
            reader.Stream.Position = part2Ptr;

            for (int i = 0; i < part2Count; i++)
            {
                ScenarioStateChunk2 chunk = new ScenarioStateChunk2();
                int pointer = reader.ReadInt32();
                short count = reader.ReadInt16();
                reader.Stream.Position += 2;

                reader.Stream.RunInPosition(delegate
                {
                    for (int k = 0; k < count; k++)
                    {
                        ScenarioStateChunk2Entry entry = new ScenarioStateChunk2Entry();
                        entry.Chapter = reader.ReadInt16();
                        entry.Unk1 = reader.ReadByte();
                        entry.Unk2 = reader.ReadByte();

                        chunk.Entries.Add(entry);
                    }
                }, pointer);

                scenarioState.Chunk2.Add(chunk);
            }
            #endregion

            #region Read state groups
            //Read state groups and states
            reader.Stream.Position = stateGroupsPtr;

            for (int i = 0; i < stateGroupsCount; i++)
            {
                ScenarioStateGroup group = new ScenarioStateGroup();
                group.Name = reader.ReadStringPointer();

                short stateStartIndex = reader.ReadInt16();
                short stateCategoriesCount = reader.ReadInt16();

                for (int k = 0; k < stateCategoriesCount; k++)
                {
                    reader.Stream.RunInPosition(delegate
                    {
                        ScenarioStateCategory category = new ScenarioStateCategory();
                        category.Name = reader.ReadStringPointer();

                        int stateCount = reader.ReadInt32();
                        int statesNameStringTablePtr = reader.ReadInt32();

                        category.Unk1 = reader.ReadInt32();
                        category.Unk2 = reader.ReadBytes(28);

                        if (statesNameStringTablePtr > 0)
                        {
                            for (int j = 0; j < stateCount; j++)
                            {
                                string stateName = reader.ReadStringPointer(statesNameStringTablePtr + (j * 4));
                                category.States.Add(stateName);
                            }
                        }

                        group.Categories.Add(category);

                    }, stateCategoryPtr + ((stateStartIndex + k) * 44));
                }

                scenarioState.Groups.Add(group);
            }
            #endregion

            #region Read timers
            //Read timers
            reader.Stream.Position = timersPtr;

            for (int i = 0; i < timersCount; i++)
            {
                ScenarioStateTimer timer = new ScenarioStateTimer();
                timer.Name = reader.ReadStringPointer();
                timer.Unk1 = reader.ReadInt16();
                timer.Unk2 = reader.ReadInt16();

                scenarioState.Timers.Add(timer);
            }
            #endregion

            #region Read counters
            //Read counters
            reader.Stream.Position = countersPtr;

            for (int i = 0; i < countersCount; i++)
            {
                ScenarioStateCounter counter = new ScenarioStateCounter();
                counter.Name = reader.ReadStringPointer();
                counter.MaximumCount = reader.ReadInt32();
                counter.Unk2 = reader.ReadInt32();

                scenarioState.Counters.Add(counter);
            }
            #endregion

            #region Read enums
            //Read enums
            reader.Stream.Position = enumsPtr;

            for (int i = 0; i < enumsCount; i++)
            {
                ScenarioStateEnum sEnum = new ScenarioStateEnum();
                sEnum.Chapter = reader.ReadInt16();
                sEnum.Scenario = reader.ReadInt16();

                scenarioState.Enums.Add(sEnum);
            }
            #endregion

            #region Read timer enums
            //Read timer enums
            reader.Stream.Position = timerControllersPtr;

            for (int i = 0; i < timerControllersCount; i++)
                scenarioState.TimerEnums.Add(reader.ReadInt32());
            #endregion

            #region Read counter enums
            //Read counter enums
            reader.Stream.Position = counterEnumsPtr;

            for (int i = 0; i < counterEnumsCount; i++)
                scenarioState.CounterEnums.Add(reader.ReadInt32());
            #endregion

            return scenarioState;
        }

        public static void Write(string path, ScenarioState scenarioState)
        {
            DataWriter writer = new DataWriter(new DataStream()) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.GetEncoding(932) };

            //Write the space first

            //Write the header
            writer.WriteTimes(0, 64);

            #region Write space for state categories
            int stateCategoriesPtr = (int)writer.Stream.Position;
            int categoryCount = 0;

            Dictionary<ScenarioStateCategory, int> stateCategoryPtrs = new Dictionary<ScenarioStateCategory, int>();

            //Write empty space for state group
            foreach (var group in scenarioState.Groups)
                foreach (var category in group.Categories)
                {
                    stateCategoryPtrs.Add(category, (int)writer.Stream.Position);
                    writer.WriteTimes(0, 44);
                    categoryCount++;
                }

            #endregion

            #region Write chunk 2
            //Write chunk 2
            int chunk2Ptr = (int)writer.Stream.Position;
            Dictionary<ScenarioStateChunk2, int> chunk2EntriesPtr = new Dictionary<ScenarioStateChunk2, int>();

            writer.WriteTimes(0, scenarioState.Chunk2.Count * 8);

            foreach (var chunk2 in scenarioState.Chunk2)
            {
                chunk2EntriesPtr[chunk2] = (int)writer.Stream.Position;

                foreach (var chunk2Entry in chunk2.Entries)
                {
                    writer.Write(chunk2Entry.Chapter);
                    writer.Write(chunk2Entry.Unk1);
                    writer.Write(chunk2Entry.Unk2);
                }
            }

            writer.Stream.RunInPosition(delegate
            {
                foreach (var chunk2 in scenarioState.Chunk2)
                {
                    writer.Write(chunk2EntriesPtr[chunk2]);
                    writer.Write((short)chunk2.Entries.Count);
                    writer.WriteTimes(0, 2);
                }
            }, chunk2Ptr);
            #endregion

            #region Write space for timers
            int timersPtr = (int)writer.Stream.Position;

            foreach(var timer in scenarioState.Timers)
            {
                writer.Write(0);
                writer.Write(timer.Unk1);
                writer.Write(timer.Unk2);
            }
            #endregion

            #region Write space for counters
            int countersPtr = (int)writer.Stream.Position;

            foreach (var counter in scenarioState.Counters)
            {
                writer.Write(0);
                writer.Write(counter.MaximumCount);
                writer.Write(counter.Unk2);
            }
            #endregion

            #region Write scenario enums
            int scenarioEnumPtr = (int)writer.Stream.Position;

            foreach (var scenarioEnum in scenarioState.Enums)
            {
                writer.Write(scenarioEnum.Chapter);
                writer.Write(scenarioEnum.Scenario);
            }
            #endregion

            #region Write timer enums
            int timerEnumsPtr = (int)writer.Stream.Position;

            foreach (int timerEnum in scenarioState.TimerEnums)
                writer.Write(timerEnum);
            #endregion

            #region Write counter enums
            int counterEnumsPtr = (int)writer.Stream.Position;

            foreach (int counterEnum in scenarioState.CounterEnums)
                writer.Write(counterEnum);
            #endregion

            #region Write space for state groups
            int stateGroupsPtr = (int)writer.Stream.Position;
            Dictionary<ScenarioStateGroup, int> stateGroupPtrs = new Dictionary<ScenarioStateGroup, int>();

            short currentStateCategoryIdx = 0;

            foreach (var stateGroup in scenarioState.Groups)
            {
                stateGroupPtrs[stateGroup] = (int)writer.Stream.Position;

                writer.Write(0xBEED);
                writer.Write(currentStateCategoryIdx);
                writer.Write((short)stateGroup.Categories.Count);

                currentStateCategoryIdx += (short)stateGroup.Categories.Count;
            }

            #endregion

            #region Write pointer space for state names

            //Strings are still written in reverse order of appearance to the table
            int statesPtr = (int)writer.Stream.Position;
            Dictionary<ScenarioStateCategory, int> stateCategoryStateStringTablePtrs = new Dictionary<ScenarioStateCategory, int>();

            foreach (var group in scenarioState.Groups)
            {
                foreach (var category in group.Categories)
                {
                    stateCategoryStateStringTablePtrs.Add(category, (int)writer.Stream.Position);
                    writer.WriteTimes(0, 4 * category.States.Count);
                }
            }
            #endregion

            #region Write the string table for scenario category, states
            StringTable statesStringTable = new StringTable(writer, writer.Stream.Position);

            Dictionary<ScenarioStateCategory, int> stateCategoryNamePtrs = new Dictionary<ScenarioStateCategory, int>();
            List<int> stateNamePtrs = new List<int>();

            foreach (var stateGroup in scenarioState.Groups)
            {
                foreach (var category in stateGroup.Categories)
                {
                    stateCategoryNamePtrs.Add(category, (int)statesStringTable.Write(category.Name, true));

                    foreach (var state in category.States)
                        stateNamePtrs.Add((int)statesStringTable.Write(state, true));
                }
            }

            #endregion

            #region Write the string table for timers and counters
            writer.Stream.Position = writer.Stream.Length;
            List<int> timerNamePtrs = new List<int>();
            StringTable timerStringTable = new StringTable(writer, writer.Stream.Position);

            foreach (var timer in scenarioState.Timers)
                timerNamePtrs.Add((int)timerStringTable.Write(timer.Name));

            writer.Stream.Position = writer.Stream.Length;
            List<int> counterNamePtrs = new List<int>();
            StringTable counterStringTable = new StringTable(writer, writer.Stream.Position);

            foreach (var counter in scenarioState.Counters)
                counterNamePtrs.Add((int)counterStringTable.Write(counter.Name));
            #endregion

            #region Write the string table for state groups
            writer.Stream.Position = writer.Stream.Length;
            Dictionary<ScenarioStateGroup, int> stateGroupNamePtrs = new Dictionary<ScenarioStateGroup, int>();
            StringTable stateGroupNameTable = new StringTable(writer, writer.Stream.Position);

            foreach (var stateGroup in scenarioState.Groups)
                stateGroupNamePtrs.Add(stateGroup, (int)stateGroupNameTable.Write(stateGroup.Name, true));
            #endregion

            #region Write name pointers for state groups, categories and states

            foreach (var stateGroup in stateGroupPtrs)
            {
                writer.Stream.RunInPosition(delegate { writer.Write(stateGroupNamePtrs[stateGroup.Key]); }, stateGroupPtrs[stateGroup.Key]);

                foreach (var stateCategory in stateGroup.Key.Categories)
                    writer.Stream.RunInPosition(
                        delegate
                        {
                            writer.Write(stateCategoryNamePtrs[stateCategory]);

                            if (stateCategory.States.Count > 0)
                            {
                                writer.Write(stateCategory.States.Count); 
                                writer.Write(stateCategoryStateStringTablePtrs[stateCategory]);
                            }
                            else
                            {
                                writer.Write(256);
                                writer.Write(0);
                            }

                            writer.Write(stateCategory.Unk1);
                            writer.Write(stateCategory.Unk2);
                        }, stateCategoryPtrs[stateCategory]);
            }

            writer.Stream.RunInPosition(delegate
            {
                foreach (int i in stateNamePtrs)
                    writer.Write(i);
            }, statesPtr);

            #endregion

            #region Write name pointers for timers and counters
            writer.Stream.RunInPosition(delegate
            {
                for (int i = 0; i < scenarioState.Timers.Count; i++)
                {
                    writer.Write(timerNamePtrs[i]);
                    writer.Stream.Position += 4;
                }
            }, timersPtr);

            writer.Stream.RunInPosition(delegate
            {
                for (int i = 0; i < scenarioState.Counters.Count; i++)
                {
                    writer.Write(counterNamePtrs[i]);
                    writer.Stream.Position += 8;
                }
            }, countersPtr);
            #endregion

            writer.Stream.Position = 0;
            writer.Write(categoryCount);
            writer.Write(stateCategoriesPtr);
            writer.Write(scenarioState.Chunk2.Count);
            writer.Write(chunk2Ptr);
            writer.Write(scenarioState.Timers.Count);
            writer.Write(timersPtr);
            writer.Write(scenarioState.Counters.Count);
            writer.Write(countersPtr);
            writer.Write(scenarioState.Enums.Count);
            writer.Write(scenarioEnumPtr);
            writer.Write(scenarioState.TimerEnums.Count);
            writer.Write(timerEnumsPtr);
            writer.Write(scenarioState.CounterEnums.Count);
            writer.Write(counterEnumsPtr);
            writer.Write(scenarioState.Groups.Count);
            writer.Write(stateGroupsPtr);

            File.WriteAllBytes(path, writer.Stream.ToArray());
        }
    }
}
