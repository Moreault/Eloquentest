using Moq;

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

            private void Load()
            {
                throw new InvalidOperationException("Not the right one!");
            }

            private void Load(string name)
            {
                throw new InvalidOperationException("This is the right one! Or is it?");
            }

            private void Load(string name, IService1 service1)
            {

            }
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

        [TestMethod]
        public void InvokeMethod_WhenHasOverloads_InvokeTheOneWithTheSameParametersTypesPassed()
        {
            //Arrange

            //Act
            var action = () => InvokeMethod("Load", "name", new Mock<IService1>().Object);

            //Assert
            action.Should().NotThrow();
        }

        [TestMethod]
        public void InvokeMethod_WhenHasOverloadsAndOneParameterNull_InvokeTheOneWithTheSameParametersTypesPassedWithoutThrowing()
        {
            //Arrange

            //Act
            var action = () => InvokeMethod("Load", "name", null!);

            //Assert
            action.Should().NotThrow();
        }

    }
}