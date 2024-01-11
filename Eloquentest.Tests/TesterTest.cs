using FluentAssertions;
using ToolBX.Eloquentest;

namespace Eloquentest.Tests
{
    [TestClass]
    public class TesterTest : Tester<TesterTest.Dummy>
    {
        public interface IService1
        {

        }

        public interface IService2
        {

        }

        public class Service2 : IService2
        {

        }

        public sealed class Dummy
        {
            private readonly IService1 _service1;
            private readonly IService2 _service2;

            public Dummy(IService1 service1, IService2 service2)
            {
                _service1 = service1;
                _service2 = service2;
            }

            public IService2 GetService2() => _service2;
        }

        [TestMethod]
        public void WhenUsingConstructWith_ShouldInjectConcreteObject()
        {
            //Arrange
            IService2 concrete = new Service2();
            ConstructWith(concrete);

            //Act
            var result = Instance.GetService2();

            //Assert
            result.Should().BeSameAs(concrete);
        }

    }
}