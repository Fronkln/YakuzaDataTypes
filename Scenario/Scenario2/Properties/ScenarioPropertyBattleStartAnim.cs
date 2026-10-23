using System;
using System.Collections.Generic;
using Yarhl.IO;


namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyBattleStartAnim : ScenarioProperty
    {
        public string Animation;

        internal override void Read(DataReader reader)
        {
            Animation = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(Animation));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
