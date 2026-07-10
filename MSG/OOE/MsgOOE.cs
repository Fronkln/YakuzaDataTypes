using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class MsgOOE
    {
        public int Unknown1;
        public int Unknown2;

        public List<MsgGroupOOE> Groups = new List<MsgGroupOOE>();
        public List<MsgPositionData> Coordinates = new List<MsgPositionData>();


        public static MsgOOE Read(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            return Read(File.ReadAllBytes(filePath));
        }

        public static MsgOOE Read(byte[] buffer)
        {
            DataStream readStream = DataStreamFactory.FromArray(buffer, 0, buffer.Length);
            DataReader reader = new DataReader(readStream) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = System.Text.Encoding.UTF8 };

            MsgOOE msg = new MsgOOE();

            msg.Unknown1 = reader.ReadInt32();
            msg.Unknown2 = reader.ReadInt32();

            //points to groups pointer which is always 24?
            int groupsPtrPtr = reader.ReadInt32();
            int coordinatesPtr = reader.ReadInt32();

            short coordinatesCount = reader.ReadInt16();
            short stringsCount = reader.ReadInt16();

            int stringTablePointer = reader.ReadInt32();

            reader.Stream.Position = groupsPtrPtr;

            int groupsPointer = reader.ReadInt32();
            short groupsCount = reader.ReadInt16();

            string[] strings = new string[0];

            reader.Stream.RunInPosition(delegate { strings = ReadStringTable(reader, stringsCount); }, stringTablePointer);
            reader.Stream.RunInPosition(delegate { msg.Coordinates = ReadCoordinates(reader, coordinatesCount); }, coordinatesPtr);
            reader.Stream.RunInPosition(delegate { msg.Groups = ReadGroups(reader, groupsCount, strings); }, groupsPointer);

            return msg;
        }

        public static void Write(MsgOOE msg, string filePath)
        {
            DataWriter writer = new DataWriter(new DataStream()) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };
            writer.Write(msg.Unknown1);
            writer.Write(msg.Unknown2);

            long pointersStart = writer.Stream.Position;

            //We will return later
            writer.WriteTimes(0, 16);

            long groupHeader = writer.Stream.Position;

            writer.WriteTimes(0, 8);

            long groupsStart = writer.Stream.Position;
;

            Dictionary<MsgGroupOOE, int> groupPointers = new Dictionary<MsgGroupOOE, int>();
            Dictionary<MsgSubGroupOOE, int> subGroupPointers = new Dictionary<MsgSubGroupOOE, int>();
            Dictionary<MsgEventOOE, int> eventPointers = new Dictionary<MsgEventOOE, int>();
            Dictionary<MsgEventOOE, int> eventStringPointers = new Dictionary<MsgEventOOE, int>();
            Dictionary<MsgPropertyOOE, int> propertyPointers = new Dictionary<MsgPropertyOOE, int>();

            //Write the overall structure of the file first
            foreach (var group in msg.Groups)
            {
                groupPointers[group] = (int)writer.Stream.Position;
                writer.WriteTimes(0, 12);
            }

            foreach (var group in msg.Groups)
            {
                foreach (var subGroup in group.Subgroups)
                {
                    subGroupPointers[subGroup] = (int)writer.Stream.Position;
                    writer.WriteTimes(0, 16);
                }
            }

            foreach (var group in msg.Groups)
            {
                foreach (var subGroup in group.Subgroups)
                {
                    foreach (var _event in subGroup.Events)
                    {
                        eventPointers[_event] = (int)writer.Stream.Position;
                        writer.WriteTimes(0, 12);
                    }
                }
            }

            long propertiesStart = writer.Stream.Position;

            foreach (var group in msg.Groups)
            {
                foreach (var subGroup in group.Subgroups)
                {
                    foreach (var _event in subGroup.Events)
                    {
                        foreach(var prop in _event.Properties)
                        {
                            propertyPointers[prop] = (int)writer.Stream.Position;
                            writer.WriteTimes(0, 16);
                        }
                    }
                }
            }


            foreach (var group in msg.Groups)
            {
                foreach (var subGroup in group.Subgroups)
                    foreach (var _event in subGroup.Events)
                    {
                        int stringPos = (int)writer.Stream.Position;
                        writer.Write(!string.IsNullOrEmpty(_event.Text) ? _event.Text : "");
                        eventStringPointers[_event] = stringPos;
                    }
            }

            writer.Align(4);


            long coordinatesPosition = writer.Stream.Position;

            for(int i = 0; i < msg.Coordinates.Count; i++)
            {
                var coordinate = msg.Coordinates[i];

                writer.Write(coordinate.Position.x);
                writer.Write(coordinate.Position.y);
                writer.Write(coordinate.Position.z);
                writer.Write(coordinate.Angle);
            }

            long stringPointerStart = writer.Stream.Position;
            List<string> strings = msg.GetStrings();

            //only used for count only. lol
            writer.WriteTimes(0, strings.Count * 4);

            StringTableIndexed indexedTable = new StringTableIndexed(writer, writer.Stream.Position);

            Dictionary<MsgPropertyOOE, MsgEventOOE> propGroupParents = new Dictionary<MsgPropertyOOE, MsgEventOOE>();

            foreach (var group in msg.Groups)
            {
                foreach(var sGroup in group.Subgroups)
                {
                    foreach (var mevent in sGroup.Events)
                    {
                        foreach (var prop in mevent.Properties)
                        {
                            propGroupParents[prop] = mevent;
                            prop.WriteStrings(writer, indexedTable);
                        }
                    }
                }
            }

            writer.Stream.Position = stringPointerStart;

            foreach (long pos in indexedTable.TablePositions)
            {
                writer.Write((int)pos);
            }


            foreach(var kv in groupPointers)
            {
                writer.Stream.Position = kv.Value;

                writer.Write(kv.Key.Unknown1);
                writer.Write((byte)kv.Key.Subgroups.Count);
                writer.Write(kv.Key.Unknown2);
                writer.Write(kv.Key.Subgroups.Count >= 0 ? subGroupPointers[kv.Key.Subgroups[0]] : 0);
                writer.Write(kv.Key.Unknown3);
                writer.Write(kv.Key.Unknown4);
                writer.Write(kv.Key.Unknown5);

                foreach(var subGroup in kv.Key.Subgroups)
                {
                    writer.Stream.Position = subGroupPointers[subGroup];
                    writer.Write(subGroup.Unknown1);
                    writer.Write(subGroup.Events.Count > 0 ? eventPointers[subGroup.Events[0]] : 0);
                    writer.Write((short)subGroup.Events.Count);
                    writer.Write(subGroup.Unknown2);
                    writer.Write(subGroup.Unknown3);
                    writer.Write(subGroup.Unknown4);

                    foreach(var _event in subGroup.Events)
                    {
                        writer.Stream.Position = eventPointers[_event];
                        writer.Write((short)(string.IsNullOrEmpty(_event.Text) ? 0 : _event.Text.Length));
                        writer.Write((byte)_event.Properties.Count);
                        writer.Write(_event.Unknown);
                        writer.Write(eventStringPointers[_event]);
                        writer.Write(_event.Properties.Count >= 0 ? propertyPointers[_event.Properties[0]] : 0);

                        foreach(var prop in _event.Properties)
                        {
                            writer.Stream.Position = propertyPointers[prop];
                            writer.Write((short)prop.Type);
                            prop.Write(writer);

                            if(prop.UnreadData != null)
                                writer.Write(prop.UnreadData);
                        }
                    }
                }
            }

            writer.Stream.Position = 8;
            writer.Write((int)groupHeader);

            writer.Write((int)coordinatesPosition);
            writer.Write((short)msg.Coordinates.Count);
            writer.Write((short)strings.Count);
            writer.Write((int)stringPointerStart);

            writer.Stream.Position = groupHeader;
            writer.Write((int)groupsStart);
            writer.Write((short)msg.Groups.Count);

            writer.Stream.Position = 0;

            byte[] buf = new byte[writer.Stream.Length];
            writer.Stream.Read(buf, 0, buf.Length);

            File.WriteAllBytes(filePath, buf);
        }

        private static string[] ReadStringTable(DataReader reader, int count)
        {
            string[] strings = new string[count];

            for (int i = 0; i < count; i++)
            {
                int pointer = reader.ReadInt32();
                reader.Stream.RunInPosition(delegate { strings[i] = reader.ReadString(); }, pointer);
            }

            return strings;
        }

        private static List<MsgPositionData> ReadCoordinates(DataReader reader, int count)
        {
            List<MsgPositionData> coordinates = new List<MsgPositionData>();

            for (int i = 0; i < count; i++)
            {
                MsgPositionData posDat = new MsgPositionData();
                posDat.Position = reader.ReadPXDVector3();
                posDat.Angle = reader.ReadInt32();

                coordinates.Add(posDat);
            }

            return coordinates;
        }

        private static List<MsgGroupOOE> ReadGroups(DataReader reader, int count, string[] stringTable)
        {
            List<MsgGroupOOE> groups = new List<MsgGroupOOE>();

            for (int i = 0; i < count; i++)
            {
                MsgGroupOOE group = new MsgGroupOOE();
                group.Unknown1 = reader.ReadInt16();

                byte subGroupCount = reader.ReadByte();
                group.Unknown2 = reader.ReadByte();
                int subGroupDataPointer = reader.ReadInt32();

                group.Unknown3 = reader.ReadByte();
                group.Unknown4 = reader.ReadInt16();
                group.Unknown5 = reader.ReadByte();

                reader.Stream.RunInPosition(delegate { group.Subgroups = ReadSubGroups(reader, subGroupCount, stringTable); }, subGroupDataPointer);

                groups.Add(group);
            }

            return groups;
        }


        private static List<MsgSubGroupOOE> ReadSubGroups(DataReader reader, int count, string[] stringTable)
        {
            List<MsgSubGroupOOE> subGroups = new List<MsgSubGroupOOE>();

            for (int k = 0; k < count; k++)
            {
                long eventHeaderStart = reader.Stream.Position;
                int unknown = reader.ReadInt32();
                int eventsPointer = reader.ReadInt32();
                short eventsCount = reader.ReadInt16();
                short unknown2 = reader.ReadInt16();
                short unknown3 = reader.ReadInt16();
                short unknown4 = reader.ReadInt16();

                MsgSubGroupOOE subGroup = new MsgSubGroupOOE();

                subGroup.Unknown1 = unknown;
                subGroup.Unknown2 = unknown2;
                subGroup.Unknown3 = unknown3;
                subGroup.Unknown4 = unknown4;

                reader.Stream.RunInPosition(delegate { subGroup.Events = ReadGroupEvents(reader, eventsCount, stringTable); }, eventsPointer);

                subGroups.Add(subGroup);
            }

            return subGroups;
        }

        private static List<MsgEventOOE> ReadGroupEvents(DataReader reader, int count, string[] stringTable)
        {
            List<MsgEventOOE> events = new List<MsgEventOOE>();

            for(int i = 0; i < count; i++)
            {
                MsgEventOOE msgEvent = new MsgEventOOE();

                short textLength = reader.ReadInt16();
                byte propertyCount = reader.ReadByte();
                byte unknown = reader.ReadByte();
                int textPointer = reader.ReadInt32();
                int propertiesPointer = reader.ReadInt32();

                if(textPointer > 0)
                {
                    reader.Stream.RunInPosition(delegate { msgEvent.Text = reader.ReadString(textLength); }, textPointer);
                }    

                if(propertiesPointer > 0)
                {
                    reader.Stream.RunInPosition(delegate { msgEvent.Properties = ReadEventProperties(reader, propertyCount, stringTable); }, propertiesPointer);
                }

                msgEvent.Unknown = unknown;
                events.Add(msgEvent);
            }

            return events;
        }

        private static List<MsgPropertyOOE> ReadEventProperties(DataReader reader, int count, string[] stringTable)
        {
            List<MsgPropertyOOE> props = new List<MsgPropertyOOE>();

            for(int i = 0; i < count; i++)
            {
                MsgPropertyOOE prop = null;
                short type = reader.ReadInt16();

                switch((MsgPropTypeOOE)type)
                {
                    default:
                        prop = new MsgPropertyOOE();
                        break;
                    case MsgPropTypeOOE.SpeechPause:
                        prop = new MSGPropertyOOESpeechPause();
                        break;
                    case MsgPropTypeOOE.SetSpeaker:
                        prop = new MsgPropertyOOESetSpeaker();
                        break;
                    case MsgPropTypeOOE.PlayActionSetAnimation:
                        prop = new MsgPropertyOOEPlayActionsetAnim();
                        break;
                    case MsgPropTypeOOE.PlayAnimation:
                        prop = new MsgPropertyOOEPlayAnim();
                        break;
                    case MsgPropTypeOOE.DialogueSettings:
                        prop = new MsgPropertyOOEDialogueSettings();
                        break;
                }

                prop.Type = (MsgPropTypeOOE)type;

                long start = reader.Stream.Position;
                long end = reader.Stream.Position + 14;

                prop.Read(reader, stringTable);

                if (reader.Stream.Position < end)
                    prop.UnreadData = reader.ReadBytes((int)(end - reader.Stream.Position));

                props.Add(prop);
            }

            return props;
        }

        internal List<string> GetStrings()
        {
            List<string> strings = new List<string>();

            foreach (var group in Groups)
            {
                foreach (var sGroup in group.Subgroups)
                {
                    foreach (var mevent in sGroup.Events)
                    {
                        foreach (var prop in mevent.Properties)
                        {
                            List<string> validStrings = prop.GetStrings().Where(x => x != "(NULL)").Distinct().ToList();

                            foreach (string str in validStrings)
                                if (!strings.Contains(str))
                                    strings.Add(str);
                        }
                    }
                }
            }

            return strings;
        }
    }
}