using AutoMapper;
using DatingApp.API.Controllers;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using DatingApp.API.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DatingApp.API.Tests.UnitTests.Controllers
{
    //TODO Create further test cases
    public class UsersControllerTests : IDisposable
    {
        private UsersController controller;
        private IMapper mapper;

        private Mock<IDatingRepository> datingRepositoryMock;
        private Mock<HttpContext> httpContextMock;
        private Mock<HttpResponse> httpResponse;

        public UsersControllerTests()
        {
            datingRepositoryMock = new Mock<IDatingRepository>();
            httpContextMock = new Mock<HttpContext>();

            httpContextMock
                .Setup(stp => stp.User)
                .Returns(ClaimsPrincipalHelper.CreateFakeUser());

            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfiles>()));
            controller = new UsersController(datingRepositoryMock.Object, mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object }
            };
        }

        public void Dispose()
        {
            datingRepositoryMock = null;
            httpContextMock = null;
            httpResponse = null;

            mapper = null;
            controller = null;
        }

        //TODO For expansion
        [Fact]
        public async Task GetUser_WhenIdIsAny_ReturnsEmptyObject()
        {
            // Arrange
            datingRepositoryMock
                .Setup(stp => stp.GetUser(It.IsAny<int>()))
                .ReturnsAsync(new User());

            // Act
            var response = await controller.GetUser(It.IsAny<int>()) as OkObjectResult;

            // Assert
            response.Value.Should().BeOfType<UserForDetailedDto>();
        }

        //TODO For expansion
        [Fact]
        public async Task GetUsers_WhenAnyUser_ReturnsDefaultObject()
        {
            // Arrange
            datingRepositoryMock
                .Setup(stp => stp.GetUser(It.IsAny<int>()))
                .ReturnsAsync(new User());
            datingRepositoryMock
                .Setup(stp => stp.GetUsers(It.IsAny<UserParams>()))
                .ReturnsAsync(new PagedList<User>(
                    new List<User>()
                    {
                        new User()
                    }, 1, 1, 1));

            httpResponse = new Mock<HttpResponse>();
            httpResponse
                .Setup(stp => stp.Headers.Add(string.Empty, StringValues.Empty));
            httpContextMock
                .Setup(stp => stp.Response)
                .Returns(httpResponse.Object);

            // Act
            var response = await controller.GetUsers(new UserParams()) as OkObjectResult;

            // Assert
            response.Value.Should().BeOfType<List<UserForListDto>>();
        }
    }
}
