using DatingApp.API.Controllers;
using DatingApp.API.Data.Interfaces;
using Moq;
using NUnit.Framework;

namespace DatingApp.Tests.Unit_tests.Controllers
{
    [TestFixture]
    public class UsersControllerTest
    {
        private UsersController controller;
        private Mock<IDatingRepository> datingRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            datingRepositoryMock = new Mock<IDatingRepository>();

            controller = new UsersController(datingRepositoryMock.Object);
        }
    }
}
