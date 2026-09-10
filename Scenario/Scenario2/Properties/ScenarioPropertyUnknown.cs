using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyUnknown : ScenarioProperty
    {
        //Includes string pointers, they are marked with 0xBEEF
        public int[] UnreadData;
        public List<string> UnknownStrings = new List<string>();
        public List<int> UnknownStringIndices = new List<int>();

        internal override int GetSize()
        {
            return UnreadData.Length * 4;
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);

            for (int i = 0; i < UnreadData.Length; i++)
            {
                if(!UnknownStringIndices.Contains(i))
                    writer.Write(UnreadData[i]);
                else
                   writer.Write((int)stringTable.Write(UnknownStrings[UnknownStringIndices.IndexOf(i)]));
            }
        }
    }
}
