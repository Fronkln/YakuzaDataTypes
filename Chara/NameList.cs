using System.Collections.Generic;
using System.IO;
using System.Text;
using Yarhl.IO;

namespace YakuzaDataTypes.Chara
{
    public class NameList
    {
        public List<string> Models = new List<string>();

        public static NameList Read(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            return Read(File.ReadAllBytes(filePath));
        }

        public static NameList Read(byte[] buffer)
        {
            NameList nameList = new NameList();

            DataStream readStream = DataStreamFactory.FromArray(buffer, 0, buffer.Length);
            DataReader reader = new DataReader(readStream) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };

            reader.Stream.Position = 16;

            int count = reader.ReadInt32();

            reader.Stream.Position = 64;

            for(int i = 0; i < count; i++)
            {
                nameList.Models.Add(reader.ReadByType(typeof(PXDHash)));
            }

            return nameList;
        }

        public static void Write(NameList nameList, string filePath)
        {
            DataWriter writer = new DataWriter(new DataStream()) { Endianness = EndiannessMode.BigEndian, DefaultEncoding = Encoding.UTF8 };

            writer.Write((long)33619968);
            writer.WriteTimes(0, 56);

            foreach(string str in nameList.Models)
            {
                writer.WriteOfType<PXDHash>(str);
            }

            File.WriteAllBytes(filePath, writer.Stream.ToArray());
        }
    }
}
