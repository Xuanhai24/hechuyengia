using SbsSW.SwiPlCs;

public class PrologHelper
{
    public PrologHelper()
    {
        if (!PlEngine.IsInitialized)
        {
            string[] param = { "-q", "-s", @"c:/users/huy/onedrive/documents/documents/prolog/benh-o-nguoi.pl" };
            PlEngine.Initialize(param);
        }
    }
    public List<string> GetSymptoms()
    {
        var results = new List<string>();

        using (var q = new PlQuery("symptom(X)"))
        {
            foreach (var sol in q.SolutionVariables)
                results.Add(sol["X"].ToString());
        }
        PlEngine.PlCleanup();
        return results;
    }
}
