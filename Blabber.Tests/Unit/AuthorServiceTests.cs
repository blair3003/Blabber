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
        }

        public void Dispose()
        {
            _mockRepository.Reset();
        }

    }
}