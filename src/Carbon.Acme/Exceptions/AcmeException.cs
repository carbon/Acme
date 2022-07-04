namespace Carbon.Acme.Exceptions;

public sealed class AcmeException : Exception
{
    public AcmeException(string message)
        : base(message) { }

    public AcmeException(Problem problem)
        : base(problem.Detail)
    {
        Problem = problem;
    }

    public Problem? Problem { get; }
}