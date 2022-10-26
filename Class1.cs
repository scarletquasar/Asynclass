using System.Runtime.CompilerServices;
using Asynclass;

var a = await new Test("a", "b");

Console.WriteLine(a.Potato);
Console.WriteLine(a.Banana);

public class Test : Async<Test>
{
    public string? Banana { get; set; }
    public string? Potato { get; set; }

    public Test(string potato, string banana)
    {
        Setup(async instance =>
        {
            await Task.Delay(1);

            instance.Potato = potato;
            instance.Banana = banana;
        });
    }
}

namespace Asynclass
{
    public abstract class Async<TBase>
    {
        private Action<TBase> _initializer = async a => await Task.Delay(0);

        public void Setup(Action<TBase> initializer)
        {
            _initializer = initializer;
        }

        private async Task<TBase> Initialize(Action<TBase> initializer)
        {
            var baseInstance = (TBase)(object)this;

            await Task.Run(() => initializer(baseInstance));
            return baseInstance;
        }

        public TaskAwaiter<TBase> GetAwaiter()
        {
            return Initialize(_initializer).GetAwaiter();
        }
    }
}