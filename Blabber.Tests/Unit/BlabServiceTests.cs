using Blabber.Api.Models;
using Blabber.Api.Repositories;
using Blabber.Api.Services;
using Moq;

namespace Blabber.Tests.Unit
{
    public class BlabServiceTests : IDisposable
    {
        private readonly Mock<IBlabRepository> _mockRepository;
        private readonly BlabService _blabService;

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

        public void Dispose()
        {
            _mockRepository.Reset();
        }

    }
}