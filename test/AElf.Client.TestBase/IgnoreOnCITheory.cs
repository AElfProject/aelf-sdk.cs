namespace AElf.Client.TestBase;

public sealed class IgnoreOnCITheory : TheoryAttribute
{
    public IgnoreOnCITheory()
    {
        if (IsOnCI()) Skip = "Ignore on CI running to save execution time.";
    }

    private static bool IsOnCI()
    {
        return Environment.GetEnvironmentVariable("CI_TEST") != null;
    }
}