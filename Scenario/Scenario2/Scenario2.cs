using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YakuzaDataTypes.Scenario;
using Yarhl.IO;
using System.CodeDom;
using System.Diagnostics;

namespace YakuzaDataTypes.Scenario
{
    public class Scenario2
    {
        public List<ChapterSequence> Chapters = new List<ChapterSequence>();

        public static Scenario2 Read(string path)
        {
            Dictionary<long, BaseSequence> sequenceLocations = new Dictionary<long, BaseSequence>();
            Dictionary<BaseSequence, int> sequenceFollowupsPtr = new Dictionary<BaseSequence, int>();

            //Chapter: Next chapter pointer
            //Mission: Chapter we belong to
            Dictionary<BaseSequence, int> sequenceChaptersPtr = new Dictionary<BaseSequence, int>();

            //Just a basic sanity check to ensure all node IDs are unique
            HashSet<int> nodeIds = new HashSet<int>();

            var scenario = new Scenario2();

            byte[] fileBuffer = File.ReadAllBytes(path);

            var readStream = DataStreamFactory.FromArray(fileBuffer, 0, fileBuffer.Length);
            var reader = new DataReader(readStream) { DefaultEncoding = Encoding.UTF8, Endianness = EndiannessMode.BigEndian };

            reader.Stream.Position += 4;
            long version = reader.ReadInt64();
            reader.Stream.Position += 4;

            int sequenceSettingsCount = reader.ReadInt32();
            int sequenceSettingsPtr = reader.ReadInt32();

            int scenarioSettingsCount = reader.ReadInt32();
            int scenarioSettingsPtr = reader.ReadInt32();

            int settingsSize = reader.ReadInt32();
            int settingsPtr = reader.ReadInt32();

            int scenarioCount = reader.ReadInt32();
            int scenarioPtr = reader.ReadInt32();

            int scenarioAccessPtr = reader.ReadInt32();
            int scenarioAccessCount = reader.ReadInt32();

            int stringTablePointer = reader.ReadInt32();
            reader.Stream.Position += 4;
            int stringTablePointer2 = reader.ReadInt32();

            reader.Stream.Position = scenarioPtr;

            List<ChapterSequence> chapters = new List<ChapterSequence>();
            List<BaseSequence> sequences = new List<BaseSequence>();

            for (int i = 0; i < scenarioCount; i++)
            {
                long sequencePos = reader.Stream.Position;

                BaseSequence sequence = null;

                int type = reader.ReadInt32();

                int followupPtr = 0;
                int chapterPtr = 0;

                switch (type)
                {
                    default:
                        throw new Exception("Unrecognized sequence type: " + type);
                    case 0:
                        ChapterSequence cSeq = new ChapterSequence();
                        cSeq.Name = reader.ReadStringPointer();
                        followupPtr = reader.ReadInt32();
                        chapterPtr = reader.ReadInt32();
                        reader.Stream.Position += 4; //unused
                        cSeq.ID = reader.ReadInt32();
                        reader.Stream.Position += 8; //unused
                        cSeq.Unk1 = reader.ReadInt32();
                        reader.Stream.Position += 12; //unused

                        sequence = cSeq;
                        break;
                    case 5:
                        MissionSequence mSeq = new MissionSequence();
                        int seqSettingsPtr = reader.ReadInt32();
                        reader.Stream.Position += 4; //unused
                        followupPtr = reader.ReadInt32();
                        chapterPtr = reader.ReadInt32();
                        mSeq.ID = reader.ReadInt32();

                        int nodeAccessPtr = reader.ReadInt32();
                        int nodeAccessCount = reader.ReadInt32();
                        reader.Stream.Position += 16;

                        reader.Stream.RunInPosition(delegate
                        {
                            int settingsPointer = reader.ReadInt32();
                            int settingsCount = reader.ReadInt32();

                            reader.Stream.RunInPosition(delegate
                            {
                                for (int k = 0; k < settingsCount; k++)
                                {
                                    ScenarioSetting setting = new ScenarioSetting();
                                    setting.MissionID = reader.ReadInt32();
                                    setting.Name = reader.ReadStringPointer();
                                    setting.PlayerPosID = reader.ReadInt32();
                                    setting.StageID = reader.ReadInt32();
                                    setting.Properties = new List<ScenarioProperty>();

                                    int propertiesSize = reader.ReadInt32() * 4;
                                    int propertiesPtr = reader.ReadInt32();

                                    if (propertiesSize > 0)
                                    {
                                        reader.Stream.RunInPosition(delegate
                                        {
                                            long propEnd = propertiesPtr + propertiesSize;

                                            while (reader.Stream.Position < propEnd)
                                            {
                                                setting.Properties.Add(ScenarioProperty.Read(reader, reader.Stream.Position, propEnd, stringTablePointer));
                                            }

                                        }, propertiesPtr);
                                    }

                                    mSeq.Settings.Add(setting);
                                }
                            }, settingsPointer);

                            //Same as the ID, don't know why they are storing this in scenario settings as well
                            //Its of no use to us.
                            int id = reader.ReadInt32();

                            reader.Stream.Position += 4;

                            mSeq.Properties = new List<ScenarioProperty>[7];
                            int[] seqPropertiesSize = new int[7];
                            int[] seqPropertiesPtr = new int[7];

                            for (int j = 0; j < 7; j++)
                                seqPropertiesSize[j] = reader.ReadInt32() * 4;

                            reader.Stream.Position += 4;

                            for (int j = 0; j < 7; j++)
                                seqPropertiesPtr[j] = reader.ReadInt32();

                            reader.Stream.Position += 4;

                            for (int j = 0; j < 7; j++)
                            {
                                mSeq.Properties[j] = new List<ScenarioProperty>();

                                if (seqPropertiesSize[j] <= 0)
                                    continue;

                                long seqPropEnd = seqPropertiesPtr[j] + seqPropertiesSize[j];

                                reader.Stream.RunInPosition(delegate
                                {
                                    while (reader.Stream.Position < seqPropEnd)
                                    {
                                        mSeq.Properties[j].Add(ScenarioProperty.Read(reader, reader.Stream.Position, seqPropEnd, stringTablePointer));
                                    }
                                }, seqPropertiesPtr[j]);
                            }

                        }, seqSettingsPtr);

                        reader.Stream.RunInPosition(
                            delegate
                            {
                                for (int j = 0; j < nodeAccessCount; j++)
                                    mSeq.AccessingNodes.Add(reader.ReadInt32());

                            }, nodeAccessPtr);

                        sequence = mSeq;
                        break;
                }


                if (nodeIds.Contains(sequence.ID))
                    throw new Exception("Node ID already exists: " + sequence.ID);

                nodeIds.Add(sequence.ID);

                sequenceLocations[sequencePos] = sequence;
                sequenceFollowupsPtr[sequence] = followupPtr;
                sequenceChaptersPtr[sequence] = chapterPtr;

                if (type == 0)
                    chapters.Add(sequence as ChapterSequence);
            }

            foreach (var kv in sequenceFollowupsPtr)
            {
                if (kv.Value <= 0)
                {
                    kv.Key.FollowupID = -1;
                    continue;
                }

                kv.Key.FollowupID = sequenceLocations[kv.Value].ID;
            }

            foreach (var kv in sequenceChaptersPtr)
            {
                if (kv.Value <= 0)
                    continue;

                if (kv.Key is MissionSequence)
                {
                    var mSeq = (MissionSequence)kv.Key;
                    var chapter = sequenceLocations[kv.Value] as ChapterSequence;
                    chapter.Missions.Add(mSeq);
                    //mSeq.ChapterID = sequenceLocations[kv.Value].ID;
                }
                else
                {
                    var cSeq = (ChapterSequence)kv.Key;
                    cSeq.NextChapterID = sequenceLocations[kv.Value].ID;
                }
            }

            scenario.Chapters = chapters;
            return scenario;
        }

        //Order:
        //State Properties
        //Sequence Properties

        public static void Write(Scenario2 scenario2, string path)
        {
            DataWriter writer = new DataWriter(new DataStream()) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };
            writer.Write(0);
            writer.Write(81604378704);
            writer.Write(0);

            //Forming the structure of the file first.
            long headerPointersArea = writer.Stream.Position;

            writer.WriteTimes(0, 368);

            long missionSettingsPointer = writer.Stream.Position;
            Dictionary<MissionSequence, int> missionSettingsLocations = new Dictionary<MissionSequence, int>();

            int totalMissionSettingsCount = 0;


            foreach(var cSeq in scenario2.Chapters)
            {
                if (cSeq.Missions.Count > 0)
                    cSeq.FollowupID = cSeq.Missions[0].ID;
                else
                    cSeq.FollowupID = -1;

                for(int i = 0; i < cSeq.Missions.Count; i++)
                {
                    var mSeq = cSeq.Missions[i];

                    if(i < cSeq.Missions.Count - 1)
                        mSeq.FollowupID = cSeq.Missions[(i + 1)].ID;
                    else
                        mSeq.FollowupID = -1;
                }
            }

            foreach (var seq in scenario2.Chapters)
            {
                foreach (var mission in seq.Missions)
                {
                    missionSettingsLocations[mission] = (int)writer.Stream.Position;
                    totalMissionSettingsCount += mission.Settings.Count;
                    writer.WriteTimes(0, 24 * mission.Settings.Count);
                }
            }

            long scenarioSettingsPointer = writer.Stream.Position;
            Dictionary<MissionSequence, long> scenarioSettingsLocations = new Dictionary<MissionSequence, long>();

            foreach (var seq in scenario2.Chapters)
            {
                foreach (var mission in seq.Missions)
                {
                    scenarioSettingsLocations[mission] = writer.Stream.Position;
                    writer.WriteTimes(0, 80);
                }
            }

            long propertiesPointer = writer.Stream.Position;

            Dictionary<MissionSequence, int[]> statePropertiesLocations = new Dictionary<MissionSequence, int[]>();
            Dictionary<MissionSequence, int[]> statePropertiesSizes = new Dictionary<MissionSequence, int[]>();
            Dictionary<ScenarioSetting, int> scenarioSettingPropertiesLocations = new Dictionary<ScenarioSetting, int>();
            Dictionary<ScenarioSetting, int> scenarioSettingPropertiesSizes = new Dictionary<ScenarioSetting, int>();

            foreach (var seq in scenario2.Chapters)
            {
                foreach (var mission in seq.Missions)
                {
                    statePropertiesLocations[mission] = new int[mission.Properties.Length];
                    statePropertiesSizes[mission] = new int[mission.Properties.Length];

                    for (int i = 0; i < mission.Properties.Length; i++)
                    {
                        var state = mission.Properties[i];

                        if (state.Count > 0)
                        {
                            long start = writer.Stream.Position;

                            statePropertiesLocations[mission][i] = (int)start;

                            foreach (var prop in state)
                                writer.WriteTimes(0, 4 + prop.GetSize());

                            statePropertiesSizes[mission][i] = (int)(writer.Stream.Position - start) / 4;
                        }
                        else
                        {
                            statePropertiesLocations[mission][i] = (int)writer.Stream.Position;
                            statePropertiesSizes[mission][i] = 0;
                        }
                    }

                    bool anyProps = mission.Settings.Any(x => x.Properties.Count > 0);
                    scenarioSettingPropertiesLocations[mission.Settings[0]] = anyProps ? (int)writer.Stream.Position : 0;

                    foreach (var setting in mission.Settings)
                    {
                        long start = writer.Stream.Position;

                        if(setting.Properties.Count <= 0)
                        {
                            scenarioSettingPropertiesLocations[setting] = 0;
                            scenarioSettingPropertiesSizes[setting] = 0;

                            continue;
                        }

                        scenarioSettingPropertiesLocations[setting] = (int)start;

                        foreach (var prop in setting.Properties)
                            writer.WriteTimes(0, 4 + prop.GetSize());

                        scenarioSettingPropertiesSizes[setting] = (int)(writer.Stream.Position - start) / 4;
                    }
                }
            }

            int propertySize = (int)(writer.Stream.Position - propertiesPointer);

            long sequencesPtr = writer.Stream.Position;
            Dictionary<BaseSequence, long> sequencesLocations = new Dictionary<BaseSequence, long>();
            int totalSequencesCount = 0;

            foreach (var seq in scenario2.Chapters)
            {
                totalSequencesCount++;
                sequencesLocations[seq] = writer.Stream.Position;
                writer.WriteTimes(0, 48);

                foreach (var mission in seq.Missions)
                {
                    totalSequencesCount++;
                    sequencesLocations[mission] = writer.Stream.Position;
                    writer.WriteTimes(0, 48);
                }
            }

            long scenarioAccessPtr = writer.Stream.Position;

            int totalAccesses = 0;
            Dictionary<MissionSequence, long> accessPtrs = new Dictionary<MissionSequence, long>();

            foreach (var seq in scenario2.Chapters)
            {
                foreach (var mission in seq.Missions)
                {
                    accessPtrs[mission] = mission.AccessingNodes.Count > 0 ? writer.Stream.Position : 0;

                    totalAccesses += mission.AccessingNodes.Count;

                    foreach (int access in mission.AccessingNodes)
                        writer.Write(access);
                }
            }

            long stringTablePtr = writer.Stream.Position;
            StringTable stringTable = new StringTable(writer, stringTablePtr);

            foreach (var seq in scenario2.Chapters)
            {
                foreach (var mission in seq.Missions)
                {
                    writer.Stream.Position = scenarioSettingsLocations[mission];
                    writer.Write(missionSettingsLocations[mission]);
                    writer.Write(mission.Settings.Count);
                    writer.Write(mission.ID);
                    writer.Write(0);

                    foreach (int val in statePropertiesSizes[mission])
                        writer.Write(val);

                    writer.Write(0);

                    foreach(int val in statePropertiesLocations[mission])
                        writer.Write(val);

                    writer.Write(0);

                    foreach (var scenarioSetting in mission.Settings)
                    {
                        writer.Stream.RunInPosition(delegate
                        {
                            foreach (var prop in scenarioSetting.Properties)
                                prop.Write(writer, stringTable);
                        }, scenarioSettingPropertiesLocations[scenarioSetting]);
                    }
                }
            }

            writer.Stream.Position = sequencesPtr;

            foreach (var seq in scenario2.Chapters)
            {
                writer.Write(0);
                writer.Write((int)stringTable.Write(seq.Name));

                if (seq.FollowupID < 0)
                    writer.Write(0);
                else
                    writer.Write((int)sequencesLocations.Where(x => x.Key.ID == seq.FollowupID).First().Value);

                int nextChapterPtr = (int)sequencesLocations.Where(x => x.Key.ID == seq.NextChapterID).First().Value;
                writer.Write(nextChapterPtr);
                writer.Write(0);
                writer.Write(seq.ID);
                writer.WriteTimes(0, 8);
                writer.Write(seq.Unk1);
                writer.WriteTimes(0, 12);

                foreach (var mission in seq.Missions)
                {
#warning This hardcoded type will break Yakuza 5
                    writer.Write(5);
                    writer.Write((int)scenarioSettingsLocations[mission]);
                    writer.Write(0);

                    int followUpPtr;

                    if (mission.FollowupID < 0)
                        followUpPtr = 0;
                    else
                    {
                        var followUpObj = sequencesLocations.Where(x => x.Key.ID == mission.FollowupID).FirstOrDefault();

                        if (followUpObj.Key != null)
                            followUpPtr = (int)followUpObj.Value;
                        else
                            followUpPtr = 0;
                    }

                    int chapterPtr = (int)sequencesLocations.Where(x => x.Key.ID == seq.ID).First().Value;

                    writer.Write(followUpPtr);
                    writer.Write(chapterPtr);
                    writer.Write(mission.ID);
                    writer.Write((int)accessPtrs[mission]);
                    writer.Write(mission.AccessingNodes.Count);
                    writer.WriteTimes(0, 16);

                    writer.Stream.RunInPosition(delegate
                    {
                        foreach (var missionSetting in mission.Settings)
                        {
                            writer.Write(missionSetting.MissionID);
                            writer.Write((int)stringTable.Write(missionSetting.Name));
                            writer.Write(missionSetting.PlayerPosID);
                            writer.Write(missionSetting.StageID);
                            writer.Write(scenarioSettingPropertiesSizes[missionSetting]);
                            writer.Write(scenarioSettingPropertiesLocations[missionSetting]);
                        }
                    }, missionSettingsLocations[mission]);


                    for (int i = 0; i < mission.Properties.Length; i++)
                    {
                        var state = mission.Properties[i];

                        if (state.Count <= 0)
                            continue;

                        writer.Stream.RunInPosition(delegate
                        {
                            foreach(var property in state)
                                property.Write(writer, stringTable);
                        }, statePropertiesLocations[mission][i]);
                    }
                }
            }

            writer.Stream.Position = headerPointersArea;
            writer.Write(totalMissionSettingsCount);
            writer.Write((int)missionSettingsPointer);
            writer.Write(scenarioSettingsLocations.Count);
            writer.Write((int)scenarioSettingsPointer);
            writer.Write(propertySize / 4);
            writer.Write((int)propertiesPointer);
            writer.Write(totalSequencesCount);
            writer.Write((int)sequencesPtr);
            writer.Write((int)scenarioAccessPtr);
            writer.Write(totalAccesses);
            writer.Write((int)stringTablePtr);
            writer.Write(0);
            writer.Write((int)stringTablePtr);

            writer.Stream.WriteTo(path);
        }
    }
}
