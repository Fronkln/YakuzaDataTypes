using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.MSG
{
    public class StringTableIndexed
    {
        public List<string> Table = new List<string>();
        public List<long> TablePositions = new List<long>();
        public long StartPosition { get; private set; }
        public long Position { get; private set; }

        private DataWriter m_writer;


        public StringTableIndexed(DataWriter writer, long pos)
        {
            StartPosition = pos;
            Position = pos;
            m_writer = writer;
        }

        public short Write(string str, bool allowDuplicate = false)
        {
            int index = Table.IndexOf(str);

            if (index != -1 && !allowDuplicate)
                return (short)index;

            long strStart = 0;

            m_writer.Stream.RunInPosition(delegate
            {
                strStart = m_writer.Stream.Position;
                m_writer.Write(str, true);
                Table.Add(str);
                TablePositions.Add(strStart);

                Position = m_writer.Stream.Position;
            }, Position);


            return (short)(Table.Count - 1);
        }


        public static string[] Read(DataReader reader, long position, int count)
        {
            string[] strings = new string[count];

            reader.Stream.RunInPosition(delegate
            {
                int[] pointers = new int[count];

                for(int i = 0; i <  count; i++)
                    pointers[i] = reader.ReadInt32();

                for(int i = 0; i < count; i++)
                {
                    int addr = pointers[i];
                    reader.Stream.Seek(addr, SeekMode.Start);
                    string str = reader.ReadString();

                    strings[i] = str;
         
                }
            }, position, SeekMode.Start);

            return strings;
        }
    }
}
