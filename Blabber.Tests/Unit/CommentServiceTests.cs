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
        public async Task GetCommentAsync_ReturnsComment()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var commentId = 1;
            var comment = new Comment { Id = commentId, BlabId = blabId, AuthorId = authorId, Body = "Test Comment" };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _commentService.GetCommentByIdAsync(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Comment", result.Body);
            _mockRepository.Verify(repo => repo.GetByIdAsync(commentId), Times.Once);
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
                .Setup(repo => repo.AddAsync(request))
                .ReturnsAsync(comment);

            // Act
            var result = await _commentService.AddCommentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(commentId, result.Id);
            Assert.Equal("Test Comment", result.Body);
            _mockRepository.Verify(repo => repo.AddAsync(request), Times.Once);
        }

        [Fact]
        public async Task UpdateCommentAsync_ModifiesComment()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var commentId = 1;
            var request = new CommentUpdateRequest { Body = "Updated Comment" };
            var updatedComment = new Comment { Id = commentId, BlabId = blabId, AuthorId = authorId, Body = "Updated Comment" };

            _mockRepository
                .Setup(repo => repo.UpdateAsync(commentId, request))
                .ReturnsAsync(updatedComment);

            // Act
            var result = await _commentService.UpdateCommentAsync(commentId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Comment", result.Body);
            _mockRepository.Verify(repo => repo.UpdateAsync(commentId, request), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_SoftDeletesComment()
        {
            // Arrange
            var authorId = 1;
            var blabId = 1;
            var commentId = 1;
            var request = new CommentUpdateRequest { Body = "Updated Comment" };
            var deletedComment = new Comment { Id = commentId, BlabId = blabId, AuthorId = authorId, Body = "Deleted Comment", IsDeleted = true };

            _mockRepository
                .Setup(repo => repo.DeleteAsync(commentId))
                .ReturnsAsync(deletedComment);

            // Act
            var result = await _commentService.DeleteCommentAsync(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsDeleted);
            Assert.Equal("[Removed]", result.Body);
            _mockRepository.Verify(repo => repo.DeleteAsync(commentId), Times.Once);
        }

        public void Dispose()
        {
            _mockRepository.Reset();
        }

    }
}