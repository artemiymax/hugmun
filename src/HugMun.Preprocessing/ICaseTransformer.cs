using HugMun.Core;

namespace HugMun.Preprocessing
{
    public interface ICaseTransformer
    {
        void Prepare(ICaseFrame cases);

        ICaseFrame Transform(ICaseFrame cases);
    }
}