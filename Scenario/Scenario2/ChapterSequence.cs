using System;
using System.Collections.Generic;


namespace YakuzaDataTypes.Scenario
{
    public class ChapterSequence : BaseSequence
    {
        public string Name = "";
        public int NextChapterID = 0;
        public int Unk1 = 1;

        public List<BaseSequence> Sequences = new List<BaseSequence>();

        public override string GetName()
        {
            return Name;
        }

        public override int GetSequenceType()
        {
            return 0;
        }
    }
}
