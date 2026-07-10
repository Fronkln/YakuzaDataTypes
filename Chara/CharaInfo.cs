using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Chara
{
    public class CharaInfo
    {
        public CharaInfoVersion Version;

        /// <summary>
        /// Access the version specific data by casting it to the type like (CharaInfoEntryKenzan)</summary>
        public List<CharaInfoEntry> Characters = new List<CharaInfoEntry>();

        /// <summary>
        /// Only applicable to version Yakuza5</summary>
        public List<int> ParSizes = new List<int>();

        /// <summary>
        /// Only applicable to version Yakuza5</summary>
        public List<int> ParCounts = new List<int>();

        public static CharaInfo Read(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return Read(File.ReadAllBytes(filePath));
        }

        public static CharaInfo Read(byte[] buffer)
        {
            CharaInfo common = new CharaInfo();

            DataStream readStream = DataStreamFactory.FromArray(buffer, 0, buffer.Length);
            DataReader reader = new DataReader(readStream) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };

            reader.Stream.Position = 0x10;

            int characterCount = reader.ReadInt32();
            int modelCount = reader.ReadInt32();

            reader.Stream.Position += 8;

            int val = reader.ReadInt32();

            if (val == 0)
            {
                common.Version = CharaInfoVersion.Yakuza5;
                reader.Stream.Position += 4;
            }
            else
            {
                int val2 = 0;

                reader.Stream.RunInPosition(delegate { val2 = reader.ReadInt32(); }, 0x1C, SeekMode.Current);

                if (val2 == 0)
                    common.Version = CharaInfoVersion.DeadSouls;
                else
                    common.Version = CharaInfoVersion.Kenzan;

                reader.Stream.Position -= 4;
            }

            long charactersStartPtr = reader.Stream.Position;

            //0x28 in Y5, 0x20 on rest is the size for a character entry
            long modelsStartPtr = (reader.Stream.Position) + (characterCount * 0x20);

            reader.Stream.Position = modelsStartPtr;

            PXDHash[] models = new PXDHash[modelCount];

            for (int i = 0; i < modelCount; i++)
            {
                PXDHash hash = reader.ReadByType(typeof(PXDHash));
                models[i] = hash;
            }

            reader.Stream.Position = charactersStartPtr;


            CharaInfoEntry[] characters = new CharaInfoEntry[characterCount];

            for (int i = 0; i < characters.Length; i++)
            {
                CharaInfoEntry charEntry = null;

                switch (common.Version)
                {
                    case CharaInfoVersion.Kenzan:
                        {
                            CharaInfoEntryKenzan kenzanEntry = new CharaInfoEntryKenzan();
                            kenzanEntry.Name = models[reader.ReadInt16()];

                            short faceModelIdx = reader.ReadInt16();
                            short topModelIdx = reader.ReadInt16();

                            kenzanEntry.FaceModel = (faceModelIdx != -1 ? models[faceModelIdx] : new PXDHash());
                            kenzanEntry.TopModel = (topModelIdx != -1 ? models[topModelIdx] : new PXDHash());
                            
                            //unused three vars
                            reader.Stream.Position += 10;

                            kenzanEntry.Height = reader.ReadByte();
                            kenzanEntry.Animations = reader.ReadByte();

                            kenzanEntry.Unknown1 = reader.ReadInt16();
                            kenzanEntry.Unknown2 = reader.ReadInt16();
                            kenzanEntry.Unknown3 = reader.ReadInt16();
                            kenzanEntry.Unknown4 = reader.ReadByte();
                            kenzanEntry.Unknown5 = reader.ReadByte();
                            kenzanEntry.Unknown6 = reader.ReadInt16();
                            kenzanEntry.Unknown7 = reader.ReadByte();
                            kenzanEntry.Unknown8 = reader.ReadByte();
                            kenzanEntry.Unknown9 = reader.ReadByte();
                            kenzanEntry.Unknown10 = reader.ReadByte();

                            charEntry = kenzanEntry;

                            break;
                        }
                }

                characters[i] = charEntry;
            }


            common.Characters = characters.ToList();


            return common;
        }

        public static void Write(CharaInfo info, string filePath)
        {
            DataWriter writer = new DataWriter(new DataStream()) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };

            writer.Write((long)33619968);
            writer.Write((long)0);
            writer.Write(info.Characters.Count);
            long modelCountAddr = writer.Stream.Position;
            writer.Write(0); //we will return to this later
            writer.Write((int)(info.Version + 1)); //if not zero this means it was written with this library, we can easy detect version
            writer.Write(new byte[] { 0x4A, 0x48, 0x52, 0x0 });


            long charactersStart = writer.Stream.Position;

            int charaSize = info.Version == CharaInfoVersion.Kenzan ? 0x20 : 0x28;
            writer.WriteTimes(0, charaSize * info.Characters.Count);

            short currentModelIndice = 0;
            Dictionary<string, short> modelIndices = new Dictionary<string, short>();

            short[] nameIndices = new short[info.Characters.Count];
            short[] faceModelIndices = new short[info.Characters.Count];
            short[] topModelIndices = new short[info.Characters.Count];

            long modelsStart = writer.Stream.Position;


            //Write character names
            for (int i = 0; i < info.Characters.Count; i++)
            {
                var character = info.Characters[i];

                short nameIndex;

                if (!modelIndices.ContainsKey(character.Name))
                {
                    modelIndices[character.Name] = currentModelIndice;
                    nameIndex = currentModelIndice;
                    currentModelIndice++;

                    writer.WriteOfType<PXDHash>(character.Name);
                }
                else
                    nameIndex = modelIndices[character.Name];

                nameIndices[i] = nameIndex;
            }

            //Write face models
            for (int i = 0; i < info.Characters.Count; i++)
            {
                var character = info.Characters[i];

                short faceModelIndex = -1;

                if (!string.IsNullOrEmpty(character.FaceModel))
                {
                    if (!modelIndices.ContainsKey(character.FaceModel))
                    {
                        modelIndices[character.FaceModel] = currentModelIndice;
                        faceModelIndex = currentModelIndice;
                        currentModelIndice++;

                        writer.WriteOfType<PXDHash>(character.FaceModel);
                    }
                    else
                        faceModelIndex = modelIndices[character.FaceModel];
                }


                faceModelIndices[i] = faceModelIndex;
            }


            //Write top models
            for (int i = 0; i < info.Characters.Count; i++)
            {
                var character = info.Characters[i];

                short topModelIndex = -1;

                if (!string.IsNullOrEmpty(character.TopModel))
                {
                    if (!modelIndices.ContainsKey(character.TopModel))
                    {
                        modelIndices[character.TopModel] = currentModelIndice;
                        topModelIndex = currentModelIndice;
                        currentModelIndice++;

                        writer.WriteOfType<PXDHash>(character.TopModel);
                    }
                    else
                        topModelIndex = modelIndices[character.TopModel];
                }

                topModelIndices[i] = topModelIndex;
            }

            //Finish character data
            writer.Stream.Position = charactersStart;

            for (int i = 0; i < info.Characters.Count; i++)
            {
                var character = info.Characters[i];

                writer.Write(nameIndices[i]);
                writer.Write(faceModelIndices[i]);
                writer.Write(topModelIndices[i]);


                switch (info.Version)
                {
                    case CharaInfoVersion.Kenzan:
                        {
                            var kenzanChara = character as CharaInfoEntryKenzan;

                            writer.WriteTimes(0, 10);
                            writer.Write(character.Height);
                            writer.Write(kenzanChara.Animations);
                            writer.Write(kenzanChara.Unknown1);
                            writer.Write(kenzanChara.Unknown2);
                            writer.Write(kenzanChara.Unknown3);
                            writer.Write(kenzanChara.Unknown4);
                            writer.Write(kenzanChara.Unknown5);
                            writer.Write(kenzanChara.Unknown6);
                            writer.Write(kenzanChara.Unknown7);
                            writer.Write(kenzanChara.Unknown8);
                            writer.Write(kenzanChara.Unknown9);
                            writer.Write(kenzanChara.Unknown10);

                            break;
                        }
                }
            }

            //Dont forget to write the model count
            writer.Stream.Position = modelCountAddr;
            writer.Write(modelIndices.Count);

            File.WriteAllBytes(filePath, writer.Stream.ToArray());
        }

        //Get all models used in the order they appear.
        public List<string> GetModelsList()
        {
            HashSet<string> models = new HashSet<string>();

            foreach(var chara in Characters)
            {
                models.Add(chara.Name);
            }

            foreach (var chara in Characters)
            {
                if(!string.IsNullOrEmpty(chara.FaceModel))
                    models.Add(chara.FaceModel);
            }

            foreach (var chara in Characters)
            {
                if (!string.IsNullOrEmpty(chara.TopModel))
                    models.Add(chara.TopModel);
            }

            //TODO DS AND Y5 BOTTOM AND HAIR MODELS HERE

            return models.ToList();
        }
    }
}
