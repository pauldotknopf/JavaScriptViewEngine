#if !DI
namespace JavaScriptViewEngine
{
    public interface IOptions<T>
    {
        T Value { get; }
    }

    public class Options<T> : IOptions<T>
    {
        public Options(T options)
        {
            Value = options;
        }

        public T Value { get; private set; }
    }
}
#endif