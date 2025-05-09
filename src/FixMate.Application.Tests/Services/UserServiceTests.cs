using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FixMate.Application.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Domain.ValueObjects;

namespace FixMate.Application.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userService = new UserService(_userRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidUser_ShouldCreateUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = new Email("john@example.com"),
                PhoneNumber = "+1234567890"
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(user.Email.Value))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.FullName, result.FullName);
            _userRepositoryMock.Verify(x => x.AddAsync(user), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingEmail_ShouldThrowException()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = new Email("john@example.com"),
                PhoneNumber = "+1234567890"
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(user.Email.Value))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _userService.CreateUserAsync(user));
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FullName = "John Doe",
                Email = new Email("john@example.com"),
                PhoneNumber = "+1234567890"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(user.FullName, result.FullName);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ShouldUpdateUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = new Email("john@example.com"),
                PhoneNumber = "+1234567890"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.UpdateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            _userRepositoryMock.Verify(x => x.Update(user), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
} 