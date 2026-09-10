using System;
using System.Collections.Generic;
using Yarhl.IO;

namespace YakuzaDataTypes.Scenario
{
    public class ScenarioEventOOE
    {
        internal virtual void ReadData(DataReader reader)
        {

        }

        internal virtual void WriteData(DataWriter writer)
        {

        }

        internal virtual void WriteStrings(DataWriter writer, ScenarioOOEWriter ooeWriter)
        {

        }

        public virtual ushort GetCategory()
        {
            return 0xDEAD;
        }
    }
}
