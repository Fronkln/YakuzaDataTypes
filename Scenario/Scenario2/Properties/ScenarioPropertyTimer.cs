using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyTimer : ScenarioProperty
    {
        public string Timer;
        public int Unk1;
        public int Unk2;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);

            Timer = reader.ReadStringPointer();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);

            writer.Write((int)stringTable.Write(Timer));
            writer.Write(Unk1);
            writer.Write(Unk2);
        }

        internal override int GetSize()
        {
            return 12;
        }
    }
}
