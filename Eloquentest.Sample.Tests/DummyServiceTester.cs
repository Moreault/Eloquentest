using Microsoft.VisualBasic;

namespace Eloquentest.Sample.Tests;

[TestClass]
public class DummyServiceTester
{
    [TestClass]
    public class DeleteFiles : Tester<DummyService>
    {
        [TestMethod]
        public void WhenFilenamesAreNull_Throw()
        {
            //Arrange
            IEnumerable<string> filenames = null!;

            //Act
            var action = () => Instance.DeleteFiles(filenames);

            //Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void WhenOneDirectoryDoesNotExist_Throw()
        {
            //Arrange
            var filenames = Fixture.CreateManyFilePaths().ToList();

            foreach (var filename in filenames)
                GetMock<IDirectory>().Setup(x => x.Exists(filename)).Returns(true);

            GetMock<IDirectory>().Setup(x => x.Exists(filenames.GetRandom())).Returns(false);

            //Act
            var action = () => Instance.DeleteFiles(filenames);

            //Assert
            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void WhenAllDirectoriesExist_DeleteAll()
        {
            //Arrange
            var filenames = Fixture.CreateManyFilePaths().ToList();

            foreach (var filename in filenames)
                GetMock<IDirectory>().Setup(x => x.Exists(Path.GetDirectoryName(filename)!)).Returns(true);

            //Act
            Instance.DeleteFiles(filenames);

            //Assert
            foreach (var filename in filenames)
                GetMock<IFile>().Verify(x => x.Delete(filename), Times.Once);
        }
    }

    [TestClass]
    public class GetSubIds : Tester<DummyService>
    {
        [TestMethod]
        public void Always_ReturnSubs()
        {
            //Arrange
            var id = Fixture.Create<int>();

            var dummy = Fixture.Build<Dummy>().With(x => x.Id, id).Create();
            GetMock<IOtherDummyService>().Setup(x => x.GetDummy(id)).Returns(dummy);

            //Act
            var result = Instance.GetSubs(id);

            //Assert
            result.Should().BeEquivalentTo(dummy.Subs);
        }
    }

    [TestClass]
    public class CreateIDummy : Tester
    {
        [TestMethod]
        public void Always_CreateDummy()
        {
            //Arrange

            //Act
            //See DummyCustomization class
            var result = Fixture.Create<IDummy>();

            //Assert
            result.Should().NotBeNull();
        }
    }

    [TestClass]
    public class SomeDate : Tester<DummyService>
    {
        [TestMethod]
        public void Always_GenerateDate()
        {
            //Arrange

            //Act
            var result = Instance.SomeDate;

            //Assert
            result.Should().NotBe(default);
        }
    }
}