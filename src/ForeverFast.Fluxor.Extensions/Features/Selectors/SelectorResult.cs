namespace Fluxor.Extensions
{
    public class SelectorResult<TResult>
    {
        public TResult Result { get; set; }

        public SelectorResult(TResult result)
        {
            Result = result;
        }
    }
}