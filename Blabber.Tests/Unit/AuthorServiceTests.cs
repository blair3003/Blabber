using Blabber.Api.Models;
using Blabber.Api.Repositories;
using Blabber.Api.Services;
using Moq;

namespace Blabber.Tests.Unit
{
    public class AuthorServiceTests : IDisposable
    {
        private readonly Mock<IAuthorRepository> _mockRepository;
        private readonly IAuthorService _authorService;

        public AuthorServiceTests()
        {
            _mockRepository = new Mock<IAuthorRepository>();
            _authorService = new AuthorService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ReturnsAllAuthors()
        {
            // Arrange
            var authors = new List<Author>
            {
                new() { Id = 1, ApplicationUserId = "1", Handle = "TestHandle1", DisplayName = "TestDisplayName1" },
                new() { Id = 2, ApplicationUserId = "2", Handle = "TestHandle2", DisplayName = "TestDisplayName2" }
            };

            _mockRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(authors);

            //Act
            var result = await _authorService.GetAllAuthorsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.Handle == "TestHandle1");
            Assert.Contains(result, a => a.Handle == "TestHandle2");
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ReturnsAuthor()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { Id = authorId, ApplicationUserId = "1", Handle = "TestHandle", DisplayName = "TestDisplayName" };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(authorId))
                .ReturnsAsync(author);

            // Act
            var result = await _authorService.GetAuthorByIdAsync(authorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestHandle", result.Handle);
            _mockRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ReturnsNull_WhenAuthorDoesNotExist()
        {
            // Arrange
            var authorId = 999;

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Author?)null);

            // Act
            var result = await _authorService.GetAuthorByIdAsync(authorId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(authorId), Times.Once);
        }

        [Fact]
        public async Task GetApplicationUserIdByAuthorIdAsync_ReturnsApplicationUserId()
        {
            // Arrange
            var authorId = 1;
            var applicationUserId = "1";
            var author = new Author { Id = authorId, ApplicationUserId = applicationUserId, Handle = "TestHandle", DisplayName = "TestDisplayName" };

            _mockRepository
                .Setup(repo => repo.GetByIdThinAsync(authorId))
                .ReturnsAsync(author);

            // Act
            var result = await _authorService.GetApplicationUserIdByAuthorIdAsync(authorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(applicationUserId, result);
            _mockRepository.Verify(repo => repo.GetByIdThinAsync(authorId), Times.Once);
        }

        [Fact]
        public async Task GetAuthorIdByApplicationUserIdAsync_ReturnsAuthorId()
        {
            // Arrange
            var authorId = 1;
            var applicationUserId = "1";
            var author = new Author { Id = authorId, ApplicationUserId = applicationUserId, Handle = "TestHandle", DisplayName = "TestDisplayName" };

            _mockRepository
                .Setup(repo => repo.GetByUserIdAsync(applicationUserId))
                .ReturnsAsync(author);

            // Act
            var result = await _authorService.GetAuthorIdByApplicationUserIdAsync(applicationUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorId, result);
            _mockRepository.Verify(repo => repo.GetByUserIdAsync(applicationUserId), Times.Once);
        }

        [Fact]
        public async Task AddAuthorAsync_CreatesAuthor()
        {
            // Arrange
            var authorId = 1;
            var applicationUserId = "1";
            var request = new AuthorCreateRequest { ApplicationUserId = applicationUserId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var author = new Author { Id = authorId, ApplicationUserId = applicationUserId, Handle = "TestHandle", DisplayName = "TestDisplayName" };

            _mockRepository
                .Setup(repo => repo.AddAsync(request))
                .ReturnsAsync(author);

            // Act
            var result = await _authorService.AddAuthorAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(authorId, result.Id);
            Assert.Equal("TestHandle", result.Handle);
            _mockRepository.Verify(repo => repo.AddAsync(request), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthorAsync_ModifiesAuthor()
        {
            // Arrange
            var authorId = 1;
            var applicationUserId = "1";
            var request = new AuthorUpdateRequest { Id = authorId, Handle = "UpdatedHandle", DisplayName = "UpdatedDisplayName" };
            var updatedAuthor = new Author { Id = authorId, ApplicationUserId = applicationUserId, Handle = "UpdatedHandle", DisplayName = "UpdatedDisplayName" };

            _mockRepository
                .Setup(repo => repo.UpdateAsync(authorId, request))
                .ReturnsAsync(updatedAuthor);

            // Act
            var result = await _authorService.UpdateAuthorAsync(authorId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UpdatedHandle", result.Handle);
            _mockRepository.Verify(repo => repo.UpdateAsync(authorId, request), Times.Once);
        }

        [Fact]
        public async Task AddAuthorFollowerAsync_AddsAuthor()
        {
            // Arrange
            var authorId = 1;
            var followerId = 2;

            _mockRepository
                .Setup(repo => repo.AddFollowerAsync(authorId, followerId))
                .ReturnsAsync(true);

            // Act
            var result = await _authorService.AddAuthorFollowerAsync(authorId, followerId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.AddFollowerAsync(authorId, followerId), Times.Once);
        }

        [Fact]
        public async Task RemoveAuthorFollowerAsync_RemoveAuthor()
        {
            // Arrange
            var authorId = 1;
            var followerId = 2;

            _mockRepository
                .Setup(repo => repo.RemoveFollowerAsync(authorId, followerId))
                .ReturnsAsync(true);

            // Act
            var result = await _authorService.RemoveAuthorFollowerAsync(authorId, followerId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.RemoveFollowerAsync(authorId, followerId), Times.Once);
        }

        public void Dispose()
        {
            _mockRepository.Reset();
        }

    }
}