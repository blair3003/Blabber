using Blabber.Api.Data;
using Blabber.Api.Models;
using Blabber.Api.Repositories;
using CommunityApp.Tests.Fixtures;

namespace Blabber.Tests.Integration
{
    public class AuthorRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly DatabaseFixture _fixture;

        public AuthorRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearData();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllAuthors()
        {
            var users = new List<ApplicationUser>
            {
                new() { Id = "1", UserName = "TestUser1", Email = "test@user.com" },
                new() { Id = "2", UserName = "TestUser2", Email = "test2@user.com" }
            };

            var authors = new List<Author>
            {
                new() { Id = 1, ApplicationUserId = "1", Handle = "TestHandle1", DisplayName = "TestDisplayName1" },
                new() { Id = 2, ApplicationUserId = "2", Handle = "TestHandle2", DisplayName = "TestDisplayName2" }
            };

            using (var context = _fixture.CreateContext())
            {
                context.Users.AddRange(users);
                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);

                var result = await repository.GetAllAsync();

                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.Contains(result, a => a.Handle == "TestHandle1");
                Assert.Contains(result, a => a.Handle == "TestHandle2");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAuthor()
        {
            var authorId = 1;
            var user = new ApplicationUser { Id = "1", UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = "1", Handle = "TestHandle", DisplayName = "TestDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.GetByIdAsync(authorId);

                Assert.NotNull(result);
                Assert.Equal("TestHandle", result.Handle);
            }

        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenAuthorDoesNotExist()
        {
            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.GetByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_CreatesAuthor()
        {
            var applicationUserId = "1";
            var authorId = 1;
            var user = new ApplicationUser { Id = applicationUserId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = applicationUserId, Handle = "TestHandle", DisplayName = "TestDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var newAuthor = await repository.AddAsync(author);

                Assert.NotNull(newAuthor);
            }

            using (var context = _fixture.CreateContext())
            {
                var result = await context.Authors.FindAsync(authorId);

                Assert.NotNull(result);
                Assert.Equal("TestHandle", result.Handle);
            }
        }

        [Fact]
        public async Task AddAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            var author = new Author { Id = 1, ApplicationUserId = "999", Handle = "TestHandle", DisplayName = "TestDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var newAuthor = await repository.AddAsync(author);

                Assert.Null(newAuthor);
            }
        }

        [Fact]
        public async Task UpdateAsync_ModifiesAuthor()
        {
            var applicationUserId = "1";
            var authorId = 1;
            var user = new ApplicationUser { Id = applicationUserId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = applicationUserId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var request = new AuthorUpdateRequest { Id = authorId, Handle = "UpdatedHandle", DisplayName = "UpdatedDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.UpdateAsync(authorId, request);

                Assert.NotNull(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var updatedAuthor = await context.Authors.FindAsync(authorId);

                Assert.NotNull(updatedAuthor);
                Assert.Equal("UpdatedHandle", updatedAuthor.Handle);
                Assert.Equal("UpdatedDisplayName", updatedAuthor.DisplayName);
            }
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenIdMismatch()
        {
            var authorId = 1;
            var wrongAuthorId = 2;
            var request = new AuthorUpdateRequest { Id = authorId, Handle = "TestHandle", DisplayName = "TestDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.UpdateAsync(wrongAuthorId, request);
                Assert.Null(result);
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