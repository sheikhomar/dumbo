namespace dumbo.Compiler
{
    class ReachabilityResult
    {
        public bool TerminatesNormally { get; }

        public ReachabilityResult(bool terminatesNormally)
        {
            TerminatesNormally = terminatesNormally;
        }
    }
}