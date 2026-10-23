using System;
using System.Collections.Generic;
using Yarhl.IO;


namespace YakuzaDataTypes.Scenario
{
    internal class ScenarioProperty9 : ScenarioProperty
    {
        public int Unk1;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);

            Unk1 = reader.ReadInt32();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);

            writer.Write(Unk1);
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
