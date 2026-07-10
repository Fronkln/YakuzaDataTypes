using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class Msg
    {
        public Game Game;

        public ushort Flags;
        public byte Type;

        public List<MsgGroup> Groups = new List<MsgGroup>();
        public List<MsgPositionData> Positions = new List<MsgPositionData>();

        public byte[] UnkSection;

        public static ushort GetIDForPropType(string propName, Game game)
        {
            Type propEnumType = null;

            if (game == Game.Y0)
                propEnumType = typeof(MsgPropTypeY0);
            else if (game == Game.Y5)
                propEnumType = typeof(MsgPropTypeY5);
            else
                propEnumType = typeof(MsgPropTypeUnknown);


            return Convert.ToUInt16(Enum.Parse(propEnumType, propName));
        }


        public static Msg Read(string path, Game game)
        {
            if (!File.Exists(path))
                return null;

            return Read(File.ReadAllBytes(path), game);
        }

        public static Msg Read(byte[] buffer, Game game)
        {
            DataStream readStream = DataStreamFactory.FromArray(buffer, 0, buffer.Length);
            DataReader reader = new DataReader(readStream) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };

            Msg msg = new Msg();
            msg.Game = game;
            msg.Flags = reader.ReadUInt16();
            msg.Type = reader.ReadByte();
            byte GroupsCount = reader.ReadByte();
            int groupsPtr = reader.ReadInt32();

            int unk1Ptr = reader.ReadInt32();
            int unk1Count = reader.ReadUInt16();

            int strCount = reader.ReadInt16();
            int strPointer = reader.ReadInt32();

            int pointerUnkSector = reader.ReadInt32();


            long headerEnd = reader.Stream.Position;

            string[] strings = StringTable.Read(reader, strPointer, strCount);

            reader.Stream.Seek(pointerUnkSector, SeekMode.Start);
            msg.UnkSection = reader.ReadBytes((int)(reader.Stream.Length - reader.Stream.Position));

            reader.Stream.Seek(groupsPtr);

            for (int i = 0; i < GroupsCount; i++)
            {
                MsgGroup group = new MsgGroup();

                int pad = reader.ReadInt32();
                int offset = reader.ReadInt32();
                short eventCount = reader.ReadInt16();

                group.Flag1 = reader.ReadInt16();
                group.Flag2 = reader.ReadInt32();

                if (pad != 0)
                    throw new NotImplementedException();

                reader.Stream.RunInPosition(delegate
                {
                    for (int k = 0; k < eventCount; k++)
                    {
                        MsgEvent mEvent = new MsgEvent();
                        int strLen = reader.ReadInt16();
                        int propCount = reader.ReadByte();
                        int padE = reader.ReadByte();
                        int strOffset = reader.ReadInt32();
                        int propOffset = reader.ReadInt32();

                        //Strlen is misleading, it didnt read the morning glory line fully on c06fe Y5, really odd.
                        //So im just gonna read it until the end with no fixed length
                        if (strLen != 0)
                            reader.Stream.RunInPosition(delegate { mEvent.Value = reader.ReadString(); }, strOffset, SeekMode.Start);

                        Type propEnumType = null;

                        if (game == Game.Y0)
                            propEnumType = typeof(MsgPropTypeY0);
                        else if (game == Game.Y5)
                            propEnumType = typeof(MsgPropTypeY5);
                        else
                            propEnumType = typeof(MsgPropTypeUnknown);

                        if (propCount != 0)
                        {
                            reader.Stream.RunInPosition(delegate
                            {

                                for (int j = 0; j < propCount; j++)
                                {

                                    ushort type = reader.ReadUInt16();
 
                                    MsgProp msgProp = null;

                                    string name = Enum.GetName(propEnumType, type);

                                    if(name != null)
                                    {
                                        switch (name)
                                        {
                                            case "Start":
                                                msgProp = new MsgPropStart();
                                                break;
                                            case "DialogueSettings":
                                                msgProp = new MsgPropDialogueSettings();
                                                break;
                                            case "Choice":
                                                msgProp = new MsgPropChoice();
                                                break;
                                            case "ScenarioBranching":
                                                msgProp = new MsgPropScenarioBranching();
                                                break;
                                            case "ChoiceBranching":
                                                msgProp = new MsgPropBranching();
                                                break;
                                            case "ChoiceBranching2":
                                                msgProp = new MsgPropBranching();
                                                break;
                                            case "UnknownBranching":
                                                msgProp = new MsgPropBranching();
                                                break;
                                            case "SpeechPause":
                                                msgProp = new MsgPropSpeechPause();
                                                break;
                                            case "SetSpeaker":
                                                msgProp = new MsgPropSetSpeaker();
                                                break;
                                            case "SetWindowType":
                                                msgProp = new MsgPropSetWindowType();
                                                break;
                                            case "UnknownPropWithString1":
                                                msgProp = new MsgPropUnknownPropWString();
                                                break;
                                            case "PlayHAct":
                                                msgProp = new MsgPropPlayHAct();
                                                break;
                                            case "UnknownPropWithString2":
                                                msgProp = new MsgPropUnknownPropWString2();
                                                break;
                                            case "PlayActionSetAnimation":
                                                msgProp = new MsgPropPlayActionSetAnimation();
                                                break;
                                            case "PlayAnimationLoop":
                                                msgProp = new MsgPropPlayAnimationLoop();
                                                break;
                                            case "SetIdleAnimation":
                                                msgProp = new MsgPropSetIdleAnimation();
                                                break;
                                            case "SetVisibility":
                                                msgProp = new MsgPropSetVisibility();
                                                break;
                                            case "PlayAnimation":
                                            case "PlayAnimationRelated":
                                                msgProp = new MsgPropPlayAnimation();
                                                break;
                                            case "MoveToEntity":
                                                msgProp = new MsgPropMoveToEntity();
                                                break;
                                            case "FaceEntity":
                                                msgProp = new MsgPropFaceEntity();
                                                break;
                                            case "Reactor":
                                                msgProp = new MsgPropReactor();
                                                break;
                                            case "FaceExpression":
                                                msgProp = new MsgPropFaceExpression();
                                                break;
                                            case "CameraParam":
                                                msgProp = new MsgPropCameraParam();
                                                break;
                                            case "TeleportToEntity":
                                                msgProp = new MsgPropTeleportToEntity();
                                                break;
                                            case "TeleportToMsgCoordinate":
                                                msgProp = new MsgPropTeleportToCoordinate();
                                                break;
                                            case "ScreenFadeIn":
                                                msgProp = new MsgPropScreenFadeIn();
                                                break;
                                            case "ScreenFadeOut":
                                                msgProp = new MsgPropScreenFadeOut();
                                                break;
                                            case "PlayBgm":
                                                msgProp = new MsgPropPlayBGM();
                                                break;
                                            case "PlayDialogueSound":
                                                msgProp = new MsgPropPlayDialogueSound();
                                                break;
                                            case "CharacterSpeech":
                                                msgProp = new MsgPropCharacterSpeech();
                                                break;

                                        }
                                    }

                                    if (msgProp == null)
                                        msgProp = new MsgProp();

                                    msgProp.PropType = type;

                                    long propDataEnd = reader.Stream.Position + 14;
                                    msgProp.ReadData(reader, strings, game > Game.Y5);
                                    long end = reader.Stream.Position;



                                    if (end < propDataEnd)
                                        msgProp.UnreadData = reader.ReadBytes((int)(propDataEnd - end));
                                    else if (end > propDataEnd)
                                        throw new Exception($"Property overread by {end - propDataEnd} bytes: " + name);

                                    mEvent.Properties.Add(msgProp);

                                }
                            }, propOffset);
                        }

                        group.Events.Add(mEvent);
                    }
                }, offset, SeekMode.Start);

                msg.Groups.Add(group);
            }
            

            reader.Stream.Seek(unk1Ptr);

            for (int i = 0; i < unk1Count; i++)
            {
                MsgPositionData data = new MsgPositionData();
                data.Position.x = reader.ReadSingle();
                data.Position.y = reader.ReadSingle();
                data.Position.z = reader.ReadSingle();
                data.Angle = reader.ReadInt32();

                msg.Positions.Add(data);
            }


            foreach(var group in msg.Groups)
            {
                foreach (var msgEvent in group.Events)
                    foreach (var msgProp in msgEvent.Properties)
                        msgProp.PostRead(group, msgEvent);
            }

            return msg;
        }
        

        public static void Write(Msg msg, string filePath)
        {
            DataWriter writer = new DataWriter(new DataStream()) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };
            writer.Write(msg.Flags);
            writer.Write(msg.Type);

            if (msg.Groups.Count >= byte.MaxValue + 1)
                throw new Exception("More than 256 groups");

            writer.Write((byte)msg.Groups.Count);
            long pointersStart = writer.Stream.Position;
            writer.WriteTimes(0, 20);

            long groupsStart = writer.Stream.Position;

            writer.WriteTimes(0, msg.Groups.Count * 16);

            Dictionary<MsgGroup, long> eventsPtr = new Dictionary<MsgGroup, long>();
            Dictionary<MsgEvent, long> propertiesPtr = new Dictionary<MsgEvent, long>();

            foreach (MsgGroup group in msg.Groups)
            {
                long eventsStart = writer.Stream.Position;
                eventsPtr[group] = eventsStart;
                writer.WriteTimes(0, group.Events.Count * 12);
            }

            foreach (MsgGroup group in msg.Groups)
            {
                foreach (MsgEvent mEvent in group.Events)
                {
                    long propertiesStart = writer.Stream.Position;
                    propertiesPtr[mEvent] = propertiesStart;

                    writer.WriteTimes(0, mEvent.Properties.Count * 16);
                }
            }

            StringTable subStringTable = new StringTable(writer, writer.Stream.Position);

            writer.Stream.Position = groupsStart;

            foreach (MsgGroup group in msg.Groups)
            {
                writer.Write(0);
                writer.Write((int)eventsPtr[group]);
                writer.Write((ushort)group.Events.Count);
                writer.Write(group.Flag1);
                writer.Write(group.Flag2);

                writer.Stream.RunInPosition(delegate
                {
                    foreach (MsgEvent mEvent in group.Events)
                    {
                        writer.Write((!string.IsNullOrEmpty(mEvent.Value) ? (ushort)mEvent.Value.Length : (ushort)0));
                        writer.Write((byte)mEvent.Properties.Count);
                        writer.Write((byte)0);
                        writer.Write((int)subStringTable.Write(mEvent.Value));
                        writer.Write((int)propertiesPtr[mEvent]);
                    }

                }, eventsPtr[group], SeekMode.Start);

            }

            writer.Stream.Position = subStringTable.Position;

            long positionsStart = writer.Stream.Position;

            foreach (var pos in msg.Positions)
            {
                writer.Write(pos.Position.x);
                writer.Write(pos.Position.y);
                writer.Write(pos.Position.z);
                writer.Write(pos.Angle);
            }

            long stringPointerStart = writer.Stream.Position;
            List<string> strings = msg.GetStrings();

            //only used for count only. lol
            writer.WriteTimes(0, strings.Count * 4);

            StringTableIndexed indexedTable = new StringTableIndexed(writer, writer.Stream.Position);

            Dictionary<MsgProp, MsgGroup> propGroupParents = new Dictionary<MsgProp, MsgGroup>();

            foreach (var group in msg.Groups)
            {
                foreach (var mevent in group.Events)
                {
                    foreach (var prop in mevent.Properties)
                    {
                        propGroupParents[prop] = group;
                        prop.WriteStrings(writer, indexedTable);
                    }
                }
            }

            writer.Stream.Position = stringPointerStart;

            foreach (long pos in indexedTable.TablePositions)
            {
                writer.Write((int)pos);
            }

            foreach (var kv in propertiesPtr)
            {
                writer.Stream.Position = kv.Value;

                foreach (var prop in kv.Key.Properties)
                {
                    writer.Write(prop.PropType);
                    prop.WriteData(propGroupParents[prop], kv.Key, writer, msg.Game > Game.Y5);

                    if (prop.UnreadData != null)
                        writer.Write(prop.UnreadData);
                }
            }



            long unkSectionPtr = writer.Stream.Length;

            writer.Stream.Position = unkSectionPtr;

            if (msg.UnkSection != null && msg.UnkSection.Length > 0)
                writer.Write(msg.UnkSection);

            writer.Stream.Position = pointersStart;
            writer.Write((int)groupsStart);
            writer.Write((int)positionsStart);
            writer.Write((ushort)msg.Positions.Count);
            writer.Write((ushort)strings.Count);
            writer.Write((int)stringPointerStart);

            if (msg.UnkSection != null && msg.UnkSection.Length > 0)
                writer.Write((int)unkSectionPtr);
            else
                writer.Write(0);

            long opos = writer.Stream.Position;
            writer.Stream.Position = 0;

            byte[] buf = new byte[writer.Stream.Length];
            writer.Stream.Read(buf, 0, buf.Length);

            writer.Stream.Position = opos;

            File.WriteAllBytes(filePath, buf);
        }

        internal List<string> GetStrings()
        {
            List<string> strings = new List<string>();

            foreach (var group in Groups)
            {
                foreach (var mevent in group.Events)
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

            return strings;
        }
    }
}
