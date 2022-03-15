![Eloquentest](https://github.com/Moreault/Eloquentest/blob/master/eloquentest.png)
# Eloquentest

A simple to use .NET unit testing framework built on top of MSTest and Moq. It also includes built-in support for services that are injected using [AutoInject].

## Getting started

Here is the dumbest service you can imagine.

```c#
    public class DumbestServiceYouCanImagine
    {
        public string Format(int number)
        {
            return number == 0 ? "It's zero" : "It's not zero";
        }
    }
```

Here is the dumbest unit test for the dumbest service you can imagine.

```c#
    [TestClass]
    public class DumbestServiceYouCanImagineTester : Tester<DumbestServiceYouCanImagine>
    {
        [TestMethod]
        public void WhenNumberIsZero_ReturnItsZero()
        {
            //Act
            var result = Instance.Format(0);

            //Assert
            Assert.AreEqual("It's zero", result);
        }
    }
```

That's all well and good but what if your dumb service had dependencies to other services though?

```c#
public interface ISomeOtherService
{
    public string UserId { get; }
    public IReadOnlyList<int> Roles { get; }
}
```

```c#
public class DumbestServiceYouCanImagine
{
    private readonly ISomeOtherService _someOtherService;

    public DumbestServiceYouCanImagine(ISomeOtherService someOtherService)
    {
        _someOtherService = someOtherService;
    }

    public string DoSomeOtherStuff()
    {
        return _someOtherService.Roles.Contains(8) ? 
            $"User {_someOtherService.UserId} is authorized to do dumb stuff." : 
            $"User {_someOtherService.UserId} is strictly forbidden from doing dumb stuff!";
    }
}
```

```c#
[TestClass]
public class DumbestServiceYouCanImagineTester : Tester<DumbestServiceYouCanImagine>
{
    [TestMethod]
    public void WhenContainsRoleNumberEight_SayThatUserIsAuthorized()
    {
        //Arrange
        var userId = Fixture.Create<string>();
        GetMock<ISomeOtherService>().Setup(x => x.UserId).Returns(userId);
        
        GetMock<ISomeOtherService>().Setup(x => x.Roles).Returns(new List<int> { 8 });

        //Act
        var result = Instance.DoSomeOtherStuff();

        //Assert
        Assert.AreEqual($"User {userId} is authorized to do dumb stuff.", result);
    }

    [TestMethod]
    public void WhenDoesNotContainRoleNumberEight_SayThatUserIsUnauthorized()
    {
        //Arrange
        var userId = Fixture.Create<string>();
        GetMock<ISomeOtherService>().Setup(x => x.UserId).Returns(userId);

        GetMock<ISomeOtherService>().Setup(x => x.Roles).Returns(new List<int>());

        //Act
        var result = Instance.DoSomeOtherStuff();

        //Assert
        Assert.AreEqual($"User {userId} is strictly forbidden from doing dumb stuff!", result);
    }
}
```

## Testing collections

Ever wanted to test **whichever** item from a collection?

```c#
[TestMethod]
public void WhenYouWantWhichever_GetWhichever()
{
    //Arrange
    var list = Fixture.CreateMany<string>().ToList();

    var something = Fixture.Create<Something>();
    GetMock<ISomeShadyService>().Setup(x => x.GetSomething(list.GetRandom())).Returns(something);

    //Act
    var result = Instance.DoSomething(list);

    //Assert
    result.Should.BeEquivalentTo(something);
}
```

You can also do the same thing but with the index only if that's what floats your boat.

```c#
var index = list.GetRandomIndex();
```

## Using the generic tester with custom constructor parameters

It’s all well and good but what if you want to use the generic Tester class while providing your own parameters to the tested class’ constructor instead of the automatically generated mocks and fixtures?

You can do that with the ConstructWith() method.

```c#
[AutoInject]
public class GameObjectFactory : IGameObjectFactory
{
    private readonly IServiceProvider _serviceProvider;

    public GameObjectFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IGameObject Create(GameObjectConfiguration configuration) => new GameObject(_serviceProvider, configuration);
}

public record GameObjectConfiguration(string Name, Vector2<float> InitialPosition, Direction InitialDirection);

//The service provider is passed by the GameObjectFactory and is always the same reference
//Configuration is unique to every instance so we need to be able to override it if we want to test that things are correctly initialized
public GameObject(IServiceProvider serviceProvider, GameObjectConfiguration configuration)
{
    _game = serviceProvider.GetRequiredService<IGame>();
    Name = configuration.Name;
    Position = configuration.InitialPosition;
    Direction = configuration.InitialDirection;
}

[TestClass]
public class GameObjectTester 
{
    [TestClass]
    public class Move : Tester<GameObject>
    {
        [TestMethod]
        public void Always_ChangePosition()
        {
            //Arrange
            var configuration = Fixture.Create<GameObjectConfiguration>();

            //It is important to call ConstructWith before accessing the Instance property!
            ConstructWith(configuration);

            //Act
            Instance.Move(new Vector2<float>(1, 0));

            //Assert
            Instance.Position.Should().Be(configuration.InitialPosition + new Vector2<float>(1, 0));
        }
    }
}
```

## Integration tests
The Eloquentest.Integration namespace (available on nuget.org as a separate package) provides tools to leverage MSTest to execute code without mocking all while using the Eloquentest structure and syntax you may already be familiar with.

IntegrationTester and IntegreationTester<T> replace Tester and Tester<T> and there are no mocks. 

### Setup
It works right out of the box if you already use AutoInject in your regular code.