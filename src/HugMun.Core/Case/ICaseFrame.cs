namespace HugMun.Core
{
    public interface ICaseFrame
    {
        CaseSchema Schema { get; }

        int CaseCount { get; }

        CaseCursor GetCaseCursor();
    }
}