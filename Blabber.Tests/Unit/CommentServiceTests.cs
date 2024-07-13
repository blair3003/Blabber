using Blabber.Api.Models;
using Blabber.Api.Repositories;
using Blabber.Api.Services;
using Moq;

namespace Blabber.Tests.Unit
{
    public class CommentServiceTests : IDisposable
    {
        private readonly Mock<ICommentRepository> _mockRepository;
        private readonly ICommentService _commentService;

        public CommentServiceTests()
        {
            _mockRepository = new Mock<ICommentRepository>();
            _commentService = new CommentService(_mockRepository.Object);
        }

        [Fact]
        public async Task AddCommentAsync_CreatesComment()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var commentId = 1;
            var request = new CommentCreateRequest { AuthorId = authorId, BlabId = blabId, Body = "Test Comment" };
            var comment = new Comment { Id = commentId, AuthorId = authorId, BlabId = blabId, Body = "Test Comment" };

            _mockRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Comment>()))
                .ReturnsAsync(comment);

            // Act
            var result = await _commentService.AddCommentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(commentId, result.Id);
            Assert.Equal("Test Comment", result.Body);
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Once);
        }

        public void Dispose()
        {
            _mockRepository.Reset();
        }

    }
}