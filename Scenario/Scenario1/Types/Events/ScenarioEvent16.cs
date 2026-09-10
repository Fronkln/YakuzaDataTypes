using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    internal class ScenarioEvent16 : ScenarioEventOOE
    {
        public int Unknown;
        public int Unknown2;

        internal override void ReadData(DataReader reader)
        {
            Unknown = reader.ReadInt32();
            Unknown2 = reader.ReadInt32();
        }

        internal override void WriteData(DataWriter writer)
        {
            writer.Write(Unknown);
            writer.Write(Unknown2);;
        }

        public override ushort GetCategory()
        {
            return 16;
        }
    }
}
