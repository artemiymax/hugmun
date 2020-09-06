namespace HugMun.Reasoning
{
    public interface IReuser
    {
        TSolution Reuse<TSolution>(ReasonerContext context);
    }
}
