![Eloquentest](https://github.com/Moreault/Eloquentest/blob/master/eloquentest.png)
# Eloquentest

A simple to use .NET unit testing framework built on top of MSTest and Moq. It also includes built-in support for services that are injected using [AutoInject].

:warning: Eloquentest is moving away from base `Tester` classes in version 3.0.0 and will instead rely on extension and utility methods to give you the tools to build 
your own base `Tester` classes that can implement whichever framework that you like. The current implementation using Moq and AutoFixture will likely be part of an 
`Eloquentest.Classic` package and may become deprecated in version 4.0.0. 

Eloquentest will also be separated into multiple packages supporting different frameworks such as Moq , NSubstitute, AutoFixture, etc.

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

### [AutoCustomization]

```c#
//This Customization will be applied project-wide so you don't have to remember to add them yourself each time in TestInitialize
[AutoCustomization]
public class SomeCustomization : ICustomization
{
    ...
}

//Also works with ISpecimenBuilder
[AutoCustomization]
public class SomeSpecimenBuilder : ISpecimenBuilder
{
    ...
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

It�s all well and good but what if you want to use the generic Tester class while providing your own parameters to the tested class� constructor instead of the automatically generated mocks and fixtures?

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

## AutoFillTester
Works just like `Tester<T>` except that it automatically fills your Instance's `public` `set` and `init` with values. Works in a lot of cases but you might want to stick with `Tester<T>` for others.

## Integration tests
The Eloquentest.Integration namespace (available on nuget.org as a separate package) provides tools to leverage MSTest to execute code without mocking all while using the Eloquentest structure and syntax you may already be familiar with.

IntegrationTester and IntegreationTester<T> replace Tester and Tester<T> and there are no mocks. 

## Ensure
The `Ensure` class comes with common test case helpers that would otherwise require you to write a lot of boilerplate code.

### WhenIsNullOrEmpty and WhenIsNullOrWhiteSpace
You can test `string.IsNullOrEmpty` and `string.IsNullOrWhitespace` cases using the following methods.

```cs
[TestMethod]
public void WhenDirectoryIsNullOrEmpty_Throw() => WhenIsNullOrEmpty(directory =>
{
    //Arrange

    //Act
    var action = () => Instance.DeleteDirectory(directory);

    //Assert
    action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(directory));
});
```

```cs
[TestMethod]
public void WhenFilenameIsNullOrWhitespace_Throw() => WhenIsNullOrWhiteSpace(filename =>
{
    //Arrange
            
    //Act
    var action = () => Instance.DeleteFile(filename);

    //Assert
    action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(filename));
});
```

These helper methods will execute your tests automatically using all cases of empty, null and white spaces without having to rely on `[DataRow]`. The method above is the equivalent of this :

```cs
[TestMethod]
[DataRow("")]
[DataRow(" ")]
[DataRow(null)]
[DataRow("\n")]
[DataRow("\r")]
[DataRow("\t")]
public void WhenFilenameIsNullOrWhitespace_Throw(string filename)
{
    //Arrange
            
    //Act
    var action = () => Instance.DeleteFile(filename);

    //Assert
    action.Should().Throw<ArgumentNullException>().WithParameterName(nameof(filename));
});
```

### ValueEquality
You can test value equality using the following methods.

* All methods named `Equals` that have a `bool` return type and a single parameter
* All `==` operators
* All `!=` operators

```cs
[TestMethod]
public void EnsureValueEquality() => Ensure.ValueEquality<YourType>();
```

This method will automatically test all cases of equality and inequality for your type. Do note that it will not test equals methods that check equality with another unrelated type. You will still need to test those manually.

### ConsistentHashCode
Tests that two equivalent instances of the same type produce the same hash code and that two different instances of the same type produce different hash codes.

```cs
[TestMethod]
public void EnsureConsistentHashCode() => Ensure.ConsistentHashCode<YourType>();
```

### Setup
It works right out of the box if you already use AutoInject in your regular code.

## Breaking changes

1.0.X -> 1.1.X
GetRandom and GetRandomIndex methods have been removed from Eloquentest. Please import and use ToolBX.OPEX from nuget.org instead.

2.0.X -> 2.1.0
`AutoFillTester<T>` was addded. There have been minor changes in when things are instantiated which may affect some users in `Tester` and `Tester<T>`.