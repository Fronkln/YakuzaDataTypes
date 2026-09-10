using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioOOE
    {
        public List<ScenarioCollection> Collections = new List<ScenarioCollection>();
        public List<int> UnknownY4Indices = new List<int>();
        public ScenarioVersion Version;

        public static ScenarioOOE Read(string path)
        {
            return ReadFromBuffer(File.ReadAllBytes(path));
        }

        public static ScenarioOOE ReadFromBuffer(byte[] buffer)
        {
            DataStream readStream = DataStreamFactory.FromArray(buffer, 0, buffer.Length);
            DataReader reader = new DataReader(readStream) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };

            reader.Stream.Position += 4;

            ScenarioOOE scenario = new ScenarioOOE();     
            ScenarioHeaderOOE header = reader.Read<ScenarioHeaderOOE>();

            ScenarioVersion ver;

            //dont forget to write to be 48 for 4 too
            if (header.UnkDataY4.Pointer != 0)
                ver = ScenarioVersion.Y4;
            else
                ver = ScenarioVersion.Y3;

            scenario.Version = ver;
            scenario.ReadCollections(reader, header.Collections, ver);

            if (ver == ScenarioVersion.Y4)
                scenario.ReadY4Indices(reader, header.UnkDataY4, ver);

            return scenario;
        }

        private void ReadCollections(DataReader reader, SizedPointer ptr, ScenarioVersion version)
        {
            reader.Stream.Seek(ptr.Pointer);

            Collections = new List<ScenarioCollection>();

            for(int i = 0; i < ptr.Size; i++)
            {
                ScenarioCollection collection = new ScenarioCollection();
                SizedPointer sequenceGroupsPtr = reader.Read<SizedPointer>();

                collection.Unknown1 = reader.ReadInt32();
                collection.Unknown2 = reader.ReadInt32();

                reader.Stream.RunInPosition(delegate { collection.ReadScenarioSequenceGroups(reader, sequenceGroupsPtr, version); }, 0);

                Collections.Add(collection);
            }
        }

        private void ReadY4Indices(DataReader reader, SizedPointer ptr, ScenarioVersion version)
        {
            UnknownY4Indices = new List<int>();

            reader.Stream.Seek(ptr.Pointer);

            for(int i = 0; i < ptr.Size; i++)
            {
                UnknownY4Indices.Add(reader.ReadInt32());
            }
        }

        public static void Write(ScenarioOOE scenario, string path)
        {
            BinaryFormat output = (BinaryFormat)ConvertFormat.With<ScenarioOOEWriter>(scenario);

            File.WriteAllBytes(path, output.Stream.ToArray());
        }
    }
}
