using System;
using System.Threading.Tasks;
using Xunit;

namespace Asynclass.Tests
{
    public class AsyncClassTests
    {
        [Fact]
        public async void AsyncClassShouldWorkProperly()
        {
            var instance = await new UserLogin();
            Assert.Equal("aaaaaaaaaaaaaaaaaaaaaaaaaaaa", instance.Token);
            Assert.Equal(DateTime.MinValue, instance.LastLogin);
        }
    }

    class UserLogin : Async<UserLogin>
    {
        public string? Token { get; set; }
        public DateTime? LastLogin { get; set; }

        public UserLogin()
        {

            Init(async () =>
            {
                var token = await Task.FromResult("aaaaaaaaaaaaaaaaaaaaaaaaaaaa");

                Token = token;
                LastLogin = DateTime.MinValue;
            });
        }
    }
}