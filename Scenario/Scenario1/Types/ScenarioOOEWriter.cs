using System;
using Yarhl.IO;
using Yarhl.FileFormat;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioOOEWriter : IConverter<ScenarioOOE, BinaryFormat>
    {
        private Dictionary<ScenarioCollectionOOE, long> m_scenarioCollectionPtrMap = new Dictionary<ScenarioCollectionOOE, long>();
        private Dictionary<ScenarioSequenceGroupOOE, long> m_scenarioSequenceGroupPtrMap = new Dictionary<ScenarioSequenceGroupOOE, long>();
        private Dictionary<ScenarioSequenceOOE, long> m_scenarioSequencePtrMap = new Dictionary<ScenarioSequenceOOE, long>();
        private Dictionary<ScenarioEventOOE, long> m_scenarioEventsPtrMap = new Dictionary<ScenarioEventOOE, long>();

        private Dictionary<ScenarioEventOOE, int> m_scenarioSequencesEventSize= new Dictionary<ScenarioEventOOE, int>();


        private DataWriter m_writer;

        private long m_y4DataPosition;
        private long m_stringTablePosition;
        private Dictionary<string, int> m_stringTable = new Dictionary<string, int>();

        public BinaryFormat Convert(ScenarioOOE scenario)
        {
            var binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);
            writer.Endianness = EndiannessMode.BigEndian;
            writer.DefaultEncoding = Encoding.UTF8;

            m_writer = writer;

            writer.Write(0);

            //Collections
            if (scenario.Version >= ScenarioVersion.Y4)
                writer.Write(0x30);
            else
                writer.Write(0x20);

            writer.Write(scenario.Collections.Count);

            //Events
            writer.Write(0xDEADC0DE);
            writer.Write(0);

            //Sequence Groups
            writer.Write(0xDEADC0DE);
            writer.Write(0);

            writer.Write(0);

            if (scenario.Version >= ScenarioVersion.Y4)
            {
                writer.Write(150);
                writer.WriteTimes(0, 12);
            }


            foreach(ScenarioCollectionOOE collection in scenario.Collections)
            {
                m_scenarioCollectionPtrMap.Add(collection, writer.Stream.Position);

                //Sequence Group SizedPtr
                writer.Write(0);
                writer.Write(collection.SequenceGroups.Count);

                writer.Write(collection.Unknown1);
                writer.Write(collection.Unknown2);
            }

            int sequenceGroupsStart = (int)writer.Stream.Position;


            foreach(var kv in m_scenarioCollectionPtrMap)
            {
                ScenarioCollectionOOE collection = kv.Key;

                int sequenceGroupStart = (int)writer.Stream.Position;
                writer.Stream.RunInPosition(delegate { writer.Write(sequenceGroupStart); }, kv.Value);

                foreach (ScenarioSequenceGroupOOE group in collection.SequenceGroups)
                {
                    m_scenarioSequenceGroupPtrMap.Add(group, writer.Stream.Position);

                    writer.Write(0);

                    //Sequences SizedPtr
                    writer.Write(0);
                    writer.Write(group.Sequences.Count);

                    writer.Write(group.Unknown1);
                    writer.Write(group.Unknown2);
                    writer.Write(group.Unknown3);
                    writer.Write(group.Unknown4);
                    writer.Write(group.Unknown5);
                }
            }

            int sequencesStart = (int)writer.Stream.Position;

            foreach(var kv in m_scenarioSequenceGroupPtrMap)
            {
                ScenarioSequenceGroupOOE group = kv.Key;
                int sequenceStart = (int)writer.Stream.Position;

                writer.Stream.RunInPosition(delegate { writer.Write(sequenceStart); }, kv.Value + 4);

                foreach(ScenarioSequenceOOE sequence in group.Sequences)
                {
                    m_scenarioSequencePtrMap.Add(sequence, writer.Stream.Position);

                    writer.Write(sequence.Type);
                    writer.Write(0);
                    writer.Write(sequence.MissionID);
                    writer.Write(0);
                    writer.Write(sequence.Unknown1);

                    //Events SizedPtr
                    writer.Write(0);
                    writer.Write(sequence.Events.Count);

                    writer.Write(sequence.Unknown2);
                    writer.Write(sequence.PlayerPosID);
                    writer.Write(sequence.Unknown3);

                    //UnkDat SizedPtr
                    writer.Write(0);
                    writer.Write(0);
                }
            }

            foreach(var kv in m_scenarioSequencePtrMap)
            {
                ScenarioSequenceOOE sequence = kv.Key;


                if (sequence.UnkDat != null && sequence.UnkDat.Length > 0)
                {
                    int unkDatStart = (int)writer.Stream.Position;

                    for (int i = 0; i < sequence.UnkDat.Length; i++)
                        writer.Write(sequence.UnkDat[i]);

                    writer.Stream.RunInPosition(delegate { writer.Write(unkDatStart); writer.Write(sequence.UnkDat.Length); }, kv.Value + 40);
                }

                int eventsStart = (int)writer.Stream.Position;

                

                foreach (ScenarioEventOOE sEvent in sequence.Events)
                {
                    m_scenarioEventsPtrMap.Add(sEvent, writer.Stream.Position);

                    writer.Write((ushort)32768);
                    writer.Write(sEvent.GetCategory());
                    sEvent.WriteData(writer);
                }

                int eventsEnd = (int)writer.Stream.Position;

                writer.Stream.RunInPosition(delegate { if (sequence.Events.Count > 0) writer.Write(eventsStart); writer.Write((eventsEnd - eventsStart) / 4); }, kv.Value + 20);
            }

            if(scenario.Version == ScenarioVersion.Y4)
            {
                m_y4DataPosition = writer.Stream.Position;

                foreach (int i in scenario.UnknownY4Indices)
                    writer.Write(i);
            }

            m_stringTablePosition = writer.Stream.Position;

            foreach(var kv in m_scenarioSequenceGroupPtrMap)
            {
                writer.Stream.Seek(kv.Value);
                writer.Write(WriteToStringTable(kv.Key.Name));

                foreach (var sequence in kv.Key.Sequences)
                {

                    long pos2 = m_scenarioSequencePtrMap[sequence] + 4;

                    if (!string.IsNullOrEmpty(sequence.ChapterCode))
                    {
                        writer.Stream.Seek(m_scenarioSequencePtrMap[sequence] + 12);
                        writer.Write(WriteToStringTable(sequence.ChapterCode));
                    }

                    writer.Stream.Seek(pos2);

                    writer.Stream.Seek(m_scenarioSequencePtrMap[sequence] + 4);
                    writer.Write(WriteToStringTable(sequence.Name));

                    foreach(var sEvent in sequence.Events)
                    {
                        writer.Stream.Seek(m_scenarioEventsPtrMap[sEvent] + 4);
                        sEvent.WriteStrings(writer, this);
                    }
                }
            }


            writer.Stream.Seek(12);
            writer.Write(sequencesStart);
            writer.Write(m_scenarioSequencePtrMap.Count);
            writer.Write(sequenceGroupsStart);
            writer.Write(m_scenarioSequenceGroupPtrMap.Count);

            if(scenario.Version >= ScenarioVersion.Y4)
            {
                writer.Write((int)m_y4DataPosition);
                writer.Write(scenario.UnknownY4Indices.Count);
            }

            return binary;
        }


        public int WriteToStringTable(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            if (m_stringTable.ContainsKey(value))
                return m_stringTable[value];

            int valStart = 0;

            m_writer.Stream.RunInPosition(delegate
            {
                valStart = (int)m_writer.Stream.Position;
                m_stringTable[value] = valStart;

                m_writer.Write(value, true, UTF8Encoding.UTF8);

                m_stringTablePosition = m_writer.Stream.Position;

            }, m_stringTablePosition);

            return valStart;
        }
    }
            
}
