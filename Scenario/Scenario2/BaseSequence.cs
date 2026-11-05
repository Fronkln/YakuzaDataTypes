namespace YakuzaDataTypes.Scenario
{
    public class BaseSequence
    {
        public int ID = 0;
        public int FollowupID = 0;

        public virtual string GetName()
        {
            return null;
        }

        public virtual int GetSequenceType()
        {
            return 0;
        }
    }
}
