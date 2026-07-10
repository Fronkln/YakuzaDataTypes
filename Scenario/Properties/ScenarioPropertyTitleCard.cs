using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyTitleCard : ScenarioProperty
    {
        public string TitleCard;

        internal override void Read(DataReader reader)
        {
            base.Read(reader);
            TitleCard = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(TitleCard));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
