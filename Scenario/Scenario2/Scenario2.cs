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
        public int Version;

        public List<BranchSequence> GetAllBranches()
        {
            List<BranchSequence> branches = new List<BranchSequence>();
            foreach (var chapter in Chapters)
            {
                foreach (var seq in chapter.Sequences)
                {
                    if (seq.GetSequenceType() != 4)
                        continue;

                    var branch = seq as BranchSequence;
                    
                    if (branch != null)
                        branches.Add(branch);
                }
            }
            return branches;
        }

        public static Scenario2 Read(string path)
        {
            Dictionary<long, BaseSequence> sequenceLocations = new Dictionary<long, BaseSequence>();
            Dictionary<BaseSequence, int> sequenceFollowupsPtr = new Dictionary<BaseSequence, int>();
            Dictionary<BranchSequence, int> sequenceBranchContinuePtr = new Dictionary<BranchSequence, int>();

            //Chapter: Next chapter pointer
            //Mission: Chapter we belong to
            Dictionary<BaseSequence, int> sequenceChaptersPtr = new Dictionary<BaseSequence, int>();

            //Just a basic sanity check to ensure all node IDs are unique
            HashSet<int> nodeIds = new HashSet<int>();

            var scenario = new Scenario2();

            byte[] fileBuffer = File.ReadAllBytes(path);

            var readStream = DataStreamFactory.FromArray(fileBuffer, 0, fileBuffer.Length);
            var reader = new DataReader(readStream) { DefaultEncoding = Encoding.UTF8, Endianness = EndiannessMode.BigEndian };
            bool isY5 = false;

            reader.Stream.RunInPosition(delegate
            {
                if (reader.ReadInt64() != reader.ReadInt64())
                    isY5 = true;
            }, 56);

            scenario.Version = isY5 ? 0 : 1;

            reader.Stream.Position += 4;
            long unkibunki = reader.ReadInt64();
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

                        if (!isY5)
                        {
                            cSeq.Unk1 = reader.ReadInt32();
                            reader.Stream.Position += 12; //unused
                        }

                        sequence = cSeq;
                        break;

                    case 4:
                        BranchSequence bSeq = new BranchSequence();

                        int branchEventsPtr = reader.ReadInt32();
                        followupPtr = reader.ReadInt32();

                        int continuePtr = reader.ReadInt32();
                        chapterPtr = reader.ReadInt32();

                        reader.Stream.Position += 12;

                        sequenceBranchContinuePtr[bSeq] = continuePtr;
                        sequence = bSeq;

                        reader.Stream.RunInPosition(delegate
                        {
                            bSeq.LogicalOperation = reader.ReadStringPointer();
                            int branchIDsPtr = reader.ReadInt32();
                            int branchCount = reader.ReadInt32();
                            
                            reader.Stream.RunInPosition(delegate
                            {
                                for (int j = 0; j < branchCount; j++)
                                    bSeq.SequenceIDs.Add(reader.ReadInt32());
                            }, branchIDsPtr);

                        }, branchEventsPtr);
                        

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

                        if(!isY5)
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


                if (sequence.ID != -1 && nodeIds.Contains(sequence.ID))
                {
                    nodeIds.Add(sequence.ID);

                    if(nodeIds.Contains(sequence.ID))
                        throw new Exception("Node ID already exists: " + sequence.ID);
                }

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

            foreach(var kv in sequenceBranchContinuePtr)
            {
                if (kv.Value <= 0)
                {
                    kv.Key.EndOfBranchSequenceID = -1;
                    continue;
                }

                kv.Key.EndOfBranchSequenceID = sequenceLocations[kv.Value].ID;
            }

            foreach (var kv in sequenceChaptersPtr)
            {
                if (kv.Value <= 0)
                    continue;

                if (!(kv.Key is ChapterSequence))
                {
                    var seq = sequenceLocations[kv.Value];
                    var chapter = seq as ChapterSequence;

                    if (chapter != null)
                        chapter.Sequences.Add(kv.Key);
                    else
                        ((seq as BranchSequence)).BranchSequences.Add(kv.Key as MissionSequence);
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

            Dictionary<BaseSequence, long> scenarioSettingsLocations = new Dictionary<BaseSequence, long>();          
            Dictionary<MissionSequence, int> missionSettingsLocations = new Dictionary<MissionSequence, int>();



            //Forming the structure of the file first.
            long headerPointersArea = writer.Stream.Position;

            writer.WriteTimes(0, scenario2.Version == 0 ? 376 : 368);

            long missionSettingsPointer = writer.Stream.Position;
            int totalMissionSettingsCount = 0;

            var scenarioBranches = scenario2.GetAllBranches();


            void WriteScenarioSettingStubForMission(MissionSequence mission)
            {
                scenarioSettingsLocations[mission] = writer.Stream.Position;
                writer.WriteTimes(0, 80);
            }


            void WriteMissionSettingStubForMission(MissionSequence mission)
            {
                missionSettingsLocations[mission] = (int)writer.Stream.Position;
                totalMissionSettingsCount += mission.Settings.Count;
                writer.WriteTimes(0, 24 * mission.Settings.Count);
            }


            //Adjust followup IDs accordingly
            foreach (var cSeq in scenario2.Chapters)
            {
                if (cSeq.Sequences.Count > 0)
                    cSeq.FollowupID = cSeq.Sequences[0].ID;
                else
                    cSeq.FollowupID = -1;

                for(int i = 0; i < cSeq.Sequences.Count; i++)
                {
                    var mSeq = cSeq.Sequences[i];

                    if(i < cSeq.Sequences.Count - 1)
                        mSeq.FollowupID = cSeq.Sequences[(i + 1)].ID;
                    else
                        mSeq.FollowupID = -1;
                }
            }

            //Write space for mission settings, we will fill this in later
            foreach (var cSeq in scenario2.Chapters)
            {
                foreach (var sequence in cSeq.Sequences)
                {
                    if (sequence.GetSequenceType() == 5)
                    {
                        var mission = sequence as MissionSequence;
                        WriteMissionSettingStubForMission(mission);
                    }
                    else
                    {
                        var branch = sequence as BranchSequence;

                       // foreach (var branchMission in branch.BranchSequences)
                         //   WriteScenarioSettingStubForMission(branchMission);
                    }
                }
            }


            //Write space for scenario settings, we will fill this in later
            long scenarioSettingsPointer = writer.Stream.Position;

            foreach (var cSeq in scenario2.Chapters)
            {
                foreach (var seq in cSeq.Sequences)
                {
                    if(seq.GetSequenceType() == 5)
                    {
                        var mission = seq as MissionSequence;
                        WriteScenarioSettingStubForMission(mission);
                    }
                    else
                    {
                        var branch = seq as BranchSequence;

                      //  foreach(var branchMission in branch.BranchSequences)
                            //    WriteScenarioSettingStubForMission(branchMission);
                    }
                }
            }

            long propertiesPointer = writer.Stream.Position;

            Dictionary<MissionSequence, int[]> statePropertiesLocations = new Dictionary<MissionSequence, int[]>();
            Dictionary<MissionSequence, int[]> statePropertiesSizes = new Dictionary<MissionSequence, int[]>();
            Dictionary<ScenarioSetting, int> scenarioSettingPropertiesLocations = new Dictionary<ScenarioSetting, int>();
            Dictionary<ScenarioSetting, int> scenarioSettingPropertiesSizes = new Dictionary<ScenarioSetting, int>();
            Dictionary<BranchSequence, int> branchSequenceOpStrLocations = new Dictionary<BranchSequence, int>();

            foreach (var cSeq in scenario2.Chapters)
            {
                foreach (var seq in cSeq.Sequences)
                {
                    var mission = seq as MissionSequence;

                    if (mission == null)
                        continue;

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
                writer.WriteTimes(0, scenario2.Version == 0 ? 32 : 48);

                foreach (var mission in seq.Sequences)
                {
                    totalSequencesCount++;
                    sequencesLocations[mission] = writer.Stream.Position;
                    writer.WriteTimes(0, scenario2.Version == 0 ? 32 : 48);
                }
            }

            long scenarioAccessPtr = writer.Stream.Position;

            int totalAccesses = 0;
            Dictionary<MissionSequence, long> accessPtrs = new Dictionary<MissionSequence, long>();

            foreach (var cSeq in scenario2.Chapters)
            {
                foreach (var seq in cSeq.Sequences)
                {
                    var mission = seq as MissionSequence;

                    //Means its a branch, it will be written afte rmissions are written
                    if (mission == null)
                        continue;

                    accessPtrs[mission] = mission.AccessingNodes.Count > 0 ? writer.Stream.Position : 0;

                    totalAccesses += mission.AccessingNodes.Count;

                    foreach (int access in mission.AccessingNodes)
                        writer.Write(access);
                }
            }

            long stringTablePtr = writer.Stream.Position;
            StringTable stringTable = new StringTable(writer, stringTablePtr);

            //Write the strings of the scenario
            foreach (var cSeq in scenario2.Chapters)
            {
                foreach (var seq in cSeq.Sequences)
                {
                    var mission = seq as MissionSequence;

                    if(mission != null)
                    {
                        writer.Stream.Position = scenarioSettingsLocations[mission];
                        writer.Write(missionSettingsLocations[mission]);
                        writer.Write(mission.Settings.Count);
                        writer.Write(mission.ID);
                        writer.Write(0);

                        foreach (int val in statePropertiesSizes[mission])
                            writer.Write(val);

                        writer.Write(0);

                        foreach (int val in statePropertiesLocations[mission])
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
                    else
                    {
                        var branch = seq as BranchSequence;
                        branchSequenceOpStrLocations[branch] = (int)stringTable.Write(branch.LogicalOperation);
                        throw new NotImplementedException("Branch write not implemented yet");
                    }
                }
            }

            writer.Stream.Position = sequencesPtr;

            foreach (var cSeq in scenario2.Chapters)
            {
                writer.Write(cSeq.GetSequenceType());
                writer.Write((int)stringTable.Write(cSeq.Name));

                if (cSeq.FollowupID < 0)
                    writer.Write(0);
                else
                    writer.Write((int)sequencesLocations.Where(x => x.Key.ID == cSeq.FollowupID).First().Value);

                int nextChapterPtr = (int)sequencesLocations.Where(x => x.Key.ID == cSeq.NextChapterID).First().Value;
                writer.Write(nextChapterPtr);
                writer.Write(0);
                writer.Write(cSeq.ID);
                writer.WriteTimes(0, 8);

                if (scenario2.Version == 1)
                {
                    writer.Write(cSeq.Unk1);
                    writer.WriteTimes(0, 12);
                }

                foreach (var seq in cSeq.Sequences)
                {
                    writer.Write(seq.GetSequenceType());
                    writer.Write((int)scenarioSettingsLocations[seq]);
            
                    var mission = seq as MissionSequence;

                    if(mission != null)
                    {
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

                        int chapterPtr = (int)sequencesLocations.Where(x => x.Key.ID == cSeq.ID).First().Value;

                        writer.Write(followUpPtr);
                        writer.Write(chapterPtr);
                        writer.Write(mission.ID);
                        writer.Write((int)accessPtrs[mission]);
                        writer.Write(mission.AccessingNodes.Count);

                        if(scenario2.Version == 1)
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
                                foreach (var property in state)
                                    property.Write(writer, stringTable);
                            }, statePropertiesLocations[mission][i]);
                        }
                    }
                    else
                    {
#warning TODO: Write branch sequence data
                        writer.WriteTimes(0, 20);
                    }

                  
                }
            }

            long scenarioBranchPtr = writer.Stream.Length;

            //Write branch settings, the branch sequence was already written, we just need to write the settings for it.
            if (scenario2.Version == 0)
            {
                writer.Stream.Position = scenarioBranchPtr;


                foreach(var branch in scenarioBranches)
                {

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

            if (scenario2.Version == 0)
            {
#warning TODO: Y5 Scenario branch
                writer.Write(0);
                writer.Write(0);
            }
            else 
            {
                writer.Write((int)stringTablePtr);
            }

            writer.Stream.WriteTo(path);
        }
    }
}
