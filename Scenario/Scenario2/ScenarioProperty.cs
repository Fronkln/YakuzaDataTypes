using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioProperty
    {
        public short Category;

        public static ScenarioProperty Read(DataReader reader, long startPtr, long endPtr, int stringTablePtr)
        {
            ushort flag = reader.ReadUInt16();
            if (flag != 0x8000)
                throw new Exception("Invalid setting data.");

            short category = reader.ReadInt16();

            ScenarioProperty prop = null;

            switch ((ScenarioPropertyTypeOE)category)
            {
                default:
                    ScenarioPropertyUnknown unkProp = new ScenarioPropertyUnknown();
                    int intsToRead = (int)(endPtr - startPtr) / 4 - 1;

                    unkProp.UnreadData = new int[intsToRead];

                    for (int i = 0; i < intsToRead; i++)
                    {
                        int value = reader.ReadInt32();

                        if (value >= stringTablePtr && value < reader.Stream.Length)
                        {
                            unkProp.UnreadData[i] = 0xBEEF;
                            unkProp.UnknownStrings.Add(reader.ReadString((long)value));
                            unkProp.UnknownStringIndices.Add(i);

                            System.Diagnostics.Debug.WriteLine(unkProp.UnknownStrings[unkProp.UnknownStrings.Count - 1]);
                        }
                        else
                            unkProp.UnreadData[i] = value;
                    }

                    prop = unkProp;

                    break;

                case ScenarioPropertyTypeOE.BGM:
                    prop = new ScenarioPropertyBGM();
                    break;

                case ScenarioPropertyTypeOE.Auth:
                    prop = new ScenarioPropertyAuth();
                    break;
                case ScenarioPropertyTypeOE.AuthImage:
                    prop = new ScenarioPropertyAuthImage();
                    break;

                case ScenarioPropertyTypeOE.Unknown9:
                    prop = new ScenarioPropertyUnknown() { UnreadData = new int[1] { reader.ReadInt32() } };
                    break;
                case ScenarioPropertyTypeOE.USM:
                    prop = new ScenarioPropertyUSM();
                    break;
                case ScenarioPropertyTypeOE.Followup:
                    prop = new ScenarioPropertyFollowup();
                    break;
                case ScenarioPropertyTypeOE.PIBArchive:
                    prop = new ScenarioPropertyPIBArchive();
                    break;
                case ScenarioPropertyTypeOE.TitleCard:
                    prop = new ScenarioPropertyTitleCard();
                    break;
                case ScenarioPropertyTypeOE.StartHAct:
                    prop = new ScenarioPropertyStartHAct();
                    break;

            }

            prop.Category = category;
            prop.Read(reader);

            return prop;
        }

        internal virtual void Read(DataReader reader)
        {

        }

        internal virtual void Write(DataWriter writer, StringTable stringTable)
        {
            writer.Write((ushort)0x8000);
            writer.Write(Category);
        }

        internal virtual int GetSize()
        {
            throw new NotImplementedException();
        }
    }
}
