namespace dumbo.Compiler
{
    public class ReturnCheckResult
    {
        public bool ContainsReturn { get; }

        public ReturnCheckResult(bool containsReturn)
        {
            ContainsReturn = containsReturn;
        }
    }
}
