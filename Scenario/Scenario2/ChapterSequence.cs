using System;
using System.Collections.Generic;


namespace YakuzaDataTypes.Scenario
{
    public class ChapterSequence : BaseSequence
    {
        public string Name = "";
        public int NextChapterID = 0;
        public int Unk1 = 1;

        public List<MissionSequence> Missions = new List<MissionSequence>();

        public override string GetName()
        {
            return Name;
        }
    }
}
