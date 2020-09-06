namespace HugMun
{
    public interface ICaseFrame
    {
        CaseSchema Schema { get; }

        int CaseCount { get; }

        CaseCursor GetCaseCursor();

        ICase GetCase(string id);
    }
}