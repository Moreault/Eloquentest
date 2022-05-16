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
}