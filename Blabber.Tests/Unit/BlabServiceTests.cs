using Blabber.Api.Models;
using Blabber.Api.Repositories;
using Blabber.Api.Services;
using Moq;

namespace Blabber.Tests.Unit
{
    public class BlabServiceTests : IDisposable
    {
        private readonly Mock<IBlabRepository> _mockRepository;
        private readonly IBlabService _blabService;

        public BlabServiceTests()
        {
            _mockRepository = new Mock<IBlabRepository>();
            _blabService = new BlabService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetBlabPageAsync_ReturnsBlabPage()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 2;
            var blabs = new List<Blab>
            {
                new() { Id = 2, AuthorId = 1, Body = "Test Blab 2" },
                new() { Id = 3, AuthorId = 1, Body = "Test Blab 3" }
            };
            var totalCount = 3;

            _mockRepository
                .Setup(repo => repo.GetAsync(pageNumber, pageSize))
                .ReturnsAsync((blabs, totalCount));

            //Act
            var result = await _blabService.GetBlabFeedAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(2, result.Blabs.Count());
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.Contains(result.Blabs, b => b.Body == "Test Blab 2");
            Assert.Contains(result.Blabs, b => b.Body == "Test Blab 3");
        }

        [Fact]
        public async Task GetBlabByIdAsync_ReturnsBlab()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(blabId))
                .ReturnsAsync(blab);

            // Act
            var result = await _blabService.GetBlabByIdAsync(blabId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Blab", result.Body);
            _mockRepository.Verify(repo => repo.GetByIdAsync(blabId), Times.Once);
        }

        [Fact]
        public async Task GetBlabByIdAsync_ReturnsNull_WhenBlabDoesNotExist()
        {
            // Arrange
            var blabId = 999;

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(blabId))
                .ReturnsAsync((Blab?)null);

            // Act
            var result = await _blabService.GetBlabByIdAsync(blabId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(blabId), Times.Once);
        }

        [Fact]
        public async Task AddBlabAsync_CreatesBlab()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var request = new BlabCreateRequest { AuthorId = authorId, Body = "New Blab" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "New Blab" };

            _mockRepository
                .Setup(repo => repo.AddAsync(request))
                .ReturnsAsync(blab);

            // Act
            var result = await _blabService.AddBlabAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(blabId, result.Id);
            Assert.Equal("New Blab", result.Body);
            _mockRepository.Verify(repo => repo.AddAsync(request), Times.Once);
        }

        [Fact]
        public async Task UpdateBlabAsync_ModifiesBlab()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var request = new BlabUpdateRequest { Id = blabId, Body = "Updated Blab" };
            var updatedBlab = new Blab { Id = blabId, AuthorId = authorId, Body = "Updated Blab" };

            _mockRepository
                .Setup(repo => repo.UpdateAsync(blabId, request))
                .ReturnsAsync(updatedBlab);

            // Act
            var result = await _blabService.UpdateBlabAsync(blabId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Blab", result.Body);
            _mockRepository.Verify(repo => repo.UpdateAsync(blabId, request), Times.Once);
        }

        [Fact]
        public async Task DeleteBlabAsync_SoftDeletesBlab()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var deletedBlab = new Blab { Id = blabId, AuthorId = authorId, Body = "Deleted Blab", IsDeleted = true };

            _mockRepository
                .Setup(repo => repo.DeleteAsync(blabId))
                .ReturnsAsync(deletedBlab);

            // Act
            var result = await _blabService.DeleteBlabAsync(blabId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsDeleted);
            Assert.Equal("[Removed]", result.Body);
            _mockRepository.Verify(repo => repo.DeleteAsync(blabId), Times.Once);
        }

        [Fact]
        public async Task AddBlabLikeAsync_LikesBlab()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;

            _mockRepository
                .Setup(repo => repo.AddLikeAsync(blabId, authorId));

            // Act
            await _blabService.AddBlabLikeAsync(blabId, authorId);

            // Assert
            _mockRepository.Verify(repo => repo.AddLikeAsync(blabId, authorId), Times.Once);
        }

        [Fact]
        public async Task RemoveBlabLikeAsync_UnlikesBlab()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;

            _mockRepository
                .Setup(repo => repo.RemoveLikeAsync(blabId, authorId));

            // Act
            await _blabService.RemoveBlabLikeAsync(blabId, authorId);

            // Assert
            _mockRepository.Verify(repo => repo.RemoveLikeAsync(blabId, authorId), Times.Once);
        }


        public void Dispose()
        {
            _mockRepository.Reset();
        }

    }
}