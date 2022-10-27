using System.Threading.Tasks;
using Xunit;

namespace Asynclass.Tests
{
    public class AsyncClassTests
    {
        [Fact]
        public async void AsyncClassShouldWorkProperly()
        {
            var instance = await new MockedClass(10, 20);

            Assert.Equal(10, instance.ValueA);
            Assert.Equal(20, instance.ValueB);
        }
    }

    class MockedClass : Async<MockedClass>
    {
        public int ValueA { get; set; }
        public int ValueB { get; set; }

        public MockedClass(int valueA, int valueB)
        {
            Init(async instance =>
            {
                await Task.Delay(100);
                instance.ValueA = valueA;
                instance.ValueB = valueB;
            });
        }
    }
}