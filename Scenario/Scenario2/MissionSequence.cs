using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace YakuzaDataTypes.Scenario
{
    public class MissionSequence : BaseSequence
    {
        //public int ChapterID;

        public List<ScenarioSetting> Settings = new List<ScenarioSetting>();
        public List<ScenarioProperty>[] Properties;

        public List<int> AccessingNodes = new List<int>();

        public override string GetName()
        {
            if (Settings.Count == 0)
                return "";

            return Settings[0].Name;
        }

        public override int GetSequenceType()
        {
            return 5;
        }
    }
}
