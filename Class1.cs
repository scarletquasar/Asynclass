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
        SetRetryTimes(10);

        Init(async instance =>
        {
            await Task.Delay(1);

            instance.Potato = potato;
            instance.Banana = banana;
        });

        Catch(errors =>
        {
            errors.ForEach(error => Console.WriteLine(error.Message));
        });
    }
}

namespace Asynclass
{
    public abstract class Async<TBase>
    {
        private Action<TBase> _initializer = async a => await Task.Delay(0);
        private Action<List<Exception>> _catcher = async a => await Task.Delay(0);
        private bool _initialized;
        private int _retryTimes = 1;

        public void SetRetryTimes(int retryTimes)
        {
            if (!_initialized)
            {
                _retryTimes = retryTimes;

                return;
            }

            throw new InvalidOperationException("Can't set retry times in a initialized async class");
        }

        public void Init(Action<TBase> initializer)
        {
            if(!_initialized)
            {
                _initializer = initializer;
                _initialized = true;

                return;
            }

            throw new InvalidOperationException("The class has already been initialized");
        }

        public void Catch(Action<List<Exception>> catcher)
        {
            if(_initialized)
            {
                _catcher = catcher;

                return;
            }

            throw new InvalidOperationException("Can't define a catcher in a non-initialized async class");
        }

        private async Task<TBase> Initialize(Action<TBase> initializer)
        {
            var baseInstance = (TBase)(object)this;

            await Task.Run(() => initializer(baseInstance));
            return baseInstance;
        }

        public TaskAwaiter<TBase> GetAwaiter()
        {
            TaskAwaiter<TBase>? awaiter = null;
            List<Exception> exceptions = new();

            while(awaiter is null && _retryTimes > 0)
            {
                try
                {
                    awaiter = Initialize(_initializer).GetAwaiter();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                finally
                {
                    _retryTimes--;
                }
            }
            
            try
            {
                if (awaiter is null)
                    throw new ArgumentNullException();
            }
            catch
            {
                _catcher(exceptions);
            }

            return (TaskAwaiter<TBase>)awaiter!;
        }
    }
}