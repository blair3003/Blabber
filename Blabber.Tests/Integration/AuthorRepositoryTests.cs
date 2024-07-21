using Blabber.Api.Data;
using Blabber.Api.Models;
using Blabber.Api.Repositories;
using CommunityApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

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
        public async Task GetByUserIdAsync_ReturnsAuthor()
        {
            var userId = "1";
            var authorId = 1;
            var user = new ApplicationUser { Id = userId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = userId, Handle = "TestHandle", DisplayName = "TestDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.GetByUserIdAsync(userId);

                Assert.NotNull(result);
                Assert.Equal("TestHandle", result.Handle);
            }

        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsNull_WhenAuthorDoesNotExist()
        {
            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.GetByUserIdAsync("999");
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_CreatesAuthor()
        {
            var applicationUserId = "1";
            var user = new ApplicationUser { Id = applicationUserId, UserName = "TestUser", Email = "test@user.com" };
            var request = new AuthorCreateRequest { Handle = "TestHandle", DisplayName = "TestDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.AddAsync(request, applicationUserId);

                Assert.NotNull(result);
                Assert.Equal("TestHandle", result.Handle);
            }
        }

        [Fact]
        public async Task AddAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            var applicationUserId = "999";
            var request = new AuthorCreateRequest { Handle = "TestHandle", DisplayName = "TestDisplayName" };

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var newAuthor = await repository.AddAsync(request, applicationUserId);

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

        [Fact]
        public async Task AddFollowerAsync_AddsFollower()
        {
            var userId1 = "1";
            var userId2 = "2";
            var authorId1 = 1;
            var authorId2 = 2;

            var users = new List<ApplicationUser>
            {
                new() { Id = userId1, UserName = "TestUser1", Email = "test@user.com" },
                new() { Id = userId2, UserName = "TestUser2", Email = "test2@user.com" }
            };

            var authors = new List<Author>
            {
                new() { Id = authorId1, ApplicationUserId = userId1, Handle = "TestHandle1", DisplayName = "TestDisplayName1" },
                new() { Id = authorId2, ApplicationUserId = userId2, Handle = "TestHandle2", DisplayName = "TestDisplayName2" }
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
                var result = await repository.AddFollowerAsync(authorId1, authorId2);

                Assert.True(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var author = await context.Authors
                    .Include(a => a.Followers)
                    .FirstOrDefaultAsync(a => a.Id == authorId1);

                Assert.NotNull(author);
                Assert.Single(author.Followers);
            }
        }

        [Fact]
        public async Task RemoveFollowerAsync_RemovesFollower()
        {
            var userId1 = "1";
            var userId2 = "2";
            var authorId1 = 1;
            var authorId2 = 2;

            var users = new List<ApplicationUser>
            {
                new() { Id = userId1, UserName = "TestUser1", Email = "test@user.com" },
                new() { Id = userId2, UserName = "TestUser2", Email = "test2@user.com" }
            };

            var author1 = new Author { Id = authorId1, ApplicationUserId = userId1, Handle = "TestHandle1", DisplayName = "TestDisplayName1" };
            var author2 = new Author { Id = authorId2, ApplicationUserId = userId2, Handle = "TestHandle2", DisplayName = "TestDisplayName2" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.AddRange(users);
                context.Authors.Add(author2);
                author1.Followers.Add(author2);
                context.Authors.Add(author1);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new AuthorRepository(context);
                var result = await repository.RemoveFollowerAsync(authorId1, authorId2);

                Assert.True(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var author = await context.Authors
                    .Include(a => a.Followers)
                    .FirstOrDefaultAsync(a => a.Id == authorId1);

                Assert.NotNull(author);
                Assert.Empty(author.Followers);
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