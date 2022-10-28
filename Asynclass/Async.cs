using System.Runtime.CompilerServices;

namespace Asynclass
{
    public abstract class Async<TBase> where TBase : Async<TBase>
    {
        private Func<TBase, Task> _initializer = async a => await Task.Delay(0);
        private Action<List<Exception>> _catcher = async a => await Task.Delay(0);
        private bool _initialized;
        private bool _throwOnError = false;
        private uint _retryTimes = 1;

        public void Config(Action<Options> optionsExpression)
        {
            if (!_initialized)
            {
                var defaultOptions = new Options();
                optionsExpression(defaultOptions);

                _retryTimes = defaultOptions.RetryTimes;
                _throwOnError = defaultOptions.ThrowOnError;

                return;
            }

            throw new InvalidOperationException("Can't set a configuration in a initialized async class");
        }

        public void Init(Func<TBase, Task> initializer)
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

        private async Task<TBase> Initialize(Func<TBase, Task> initializer)
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
                if(_throwOnError)
                {
                    throw new AggregateException(exceptions);
                }

                _catcher(exceptions);
            }

            return (TaskAwaiter<TBase>)awaiter!;
        }
    }
}