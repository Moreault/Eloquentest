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
