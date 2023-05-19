using System.Runtime.CompilerServices;

namespace Asynclass
{ 
    public abstract class Async<TBase> where TBase : Async<TBase>
    {
        private Func<Task> _initializer = async () => await Task.FromResult(0);
        private bool _initialized;

        protected void Init(Func<Task> initializer)
        {
            if(!_initialized)
            {
                _initializer = initializer;
                _initialized = true;

                return;
            }

            throw new InvalidOperationException("The class has already been initialized");
        }

        private async Task<TBase> Initialize(Func<Task> initializer)
        {
            var baseInstance = (TBase)(object)this;

            await Task.Run(() => initializer());
            return baseInstance;
        }

        public TaskAwaiter<TBase> GetAwaiter()
        {
            try
            {
                TaskAwaiter<TBase>? awaiter = Initialize(_initializer).GetAwaiter();
                return (TaskAwaiter<TBase>)awaiter!;
            }
            catch
            {
                throw;
            }
        }
    }
}