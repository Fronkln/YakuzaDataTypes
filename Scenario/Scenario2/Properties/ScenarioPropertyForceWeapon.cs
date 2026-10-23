using System;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioPropertyForceWeapon : ScenarioProperty
    {
        public string Equip;

        internal override void Read(DataReader reader)
        {
            Equip = reader.ReadStringPointer();
        }

        internal override void Write(DataWriter writer, StringTable stringTable)
        {
            base.Write(writer, stringTable);
            writer.Write((int)stringTable.Write(Equip));
        }

        internal override int GetSize()
        {
            return 4;
        }
    }
}
