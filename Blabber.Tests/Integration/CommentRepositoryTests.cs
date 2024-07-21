using Blabber.Api.Data;
using Blabber.Api.Models;
using Blabber.Api.Repositories;
using CommunityApp.Tests.Fixtures;

namespace Blabber.Tests.Integration
{
    public class CommentRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly DatabaseFixture _fixture;

        public CommentRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearData();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsComment()
        {
            var userId = "1";
            var authorId = 1;
            var blabId = 1;
            var commentId = 1;
            var user = new ApplicationUser { Id = userId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = userId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };
            var comment = new Comment { Id = commentId, AuthorId = authorId, BlabId = blabId, Body = "Test Comment" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new CommentRepository(context);
                var result = await repository.GetByIdAsync(commentId);

                Assert.NotNull(result);
                Assert.Equal("Test Comment", result.Body);
            }
        }

        [Fact]
        public async Task AddAsync_CreatesComment()
        {
            var applicationUserId = "1";
            var authorId = 1;
            var blabId = 1;
            var user = new ApplicationUser { Id = applicationUserId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = applicationUserId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };
            var request = new CommentCreateRequest { AuthorId = authorId, BlabId = blabId, Body = "Test Comment" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new CommentRepository(context);
                var result = await repository.AddAsync(request);

                Assert.NotNull(result);
                Assert.Equal("Test Comment", result.Body);
            }
        }

        [Fact]
        public async Task UpdateAsync_ModifiesComment()
        {
            var userId = "1";
            var authorId = 1;
            var blabId = 1;
            var commentId = 1;
            var user = new ApplicationUser { Id = userId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = userId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };
            var comment = new Comment { Id = commentId, AuthorId = authorId, BlabId = blabId, Body = "Test Comment" };
            var request = new CommentUpdateRequest { Id = commentId, Body = "Updated Comment" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new CommentRepository(context);
                var result = await repository.UpdateAsync(commentId, request);

                Assert.NotNull(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var result = await context.Comments.FindAsync(commentId);

                Assert.NotNull(result);
                Assert.Equal("Updated Comment", result.Body);
            }
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesComment()
        {
            var userId = "1";
            var authorId = 1;
            var blabId = 1;
            var commentId = 1;
            var user = new ApplicationUser { Id = userId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = userId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };
            var comment = new Comment { Id = commentId, AuthorId = authorId, BlabId = blabId, Body = "Test Comment" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                context.Comments.Add(comment);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new CommentRepository(context);
                var result = await repository.DeleteAsync(commentId);

                Assert.NotNull(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var result = await context.Comments.FindAsync(commentId);

                Assert.NotNull(result);
                Assert.True(result.IsDeleted);
            }
        }

        public void Dispose()
        {
            _fixture.ClearData();

            using (var context = _fixture.CreateContext())
            {
                if (context.Blabs.Any() || context.Comments.Any() || context.Authors.Any() || context.Users.Any())
                {
                    throw new InvalidOperationException("Failed to clear data.");
                }
            }
        }

    }
}