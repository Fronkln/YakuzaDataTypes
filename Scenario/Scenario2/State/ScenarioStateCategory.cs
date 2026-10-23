using System;
using System.Collections.Generic;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioStateCategory
    {
        public string Name;
        public int Unk1;
        public byte[] Unk2 = new byte[32];

        public List<string> States = new List<string>();

        public override string ToString()
        {
            return Name;
        }
    }
}
