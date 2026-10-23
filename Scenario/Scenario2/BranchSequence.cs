using System;
using System.Collections.Generic;

namespace YakuzaDataTypes.Scenario
{
    public class BranchSequence : BaseSequence
    {
        public string LogicalOperation;
        public List<int> SequenceIDs = new List<int>();
        public int EndOfBranchSequenceID = -1;

        public List<MissionSequence> BranchSequences = new List<MissionSequence>();

        public BranchSequence() : base()
        {
            ID = -1;
        }

        public override int GetSequenceType()
        {
            return 4;
        }
    }
}
