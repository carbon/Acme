using System;

namespace Carbon.Acme
{
    public class AcmeException : Exception
    {
        public AcmeException(string message)
            : base(message) { }

        public AcmeException(Problem problem)
            : base(problem.Detail)
        {
            Problem = problem;
        }

        public Problem Problem { get; }
    }
}