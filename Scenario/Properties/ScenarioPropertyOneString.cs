using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    //A property with just one string
    internal class ScenarioPropertyOneString : ScenarioProperty
    {
        public string String;

        internal override void Read(DataReader reader)
        {
            String = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(String));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
