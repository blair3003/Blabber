using Blabber.Api.Data;
using Blabber.Api.Models;
using Blabber.Api.Repositories;
using CommunityApp.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Blabber.Tests.Integration
{
    public class BlabRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly DatabaseFixture _fixture;

        public BlabRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearData();
        }

        [Fact]
        public async Task GetAsync_ReturnsBlabs()
        {
            var user = new ApplicationUser { Id = "1", UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = 1, ApplicationUserId = "1", Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blabs = new List<Blab>
            {
                new() { Id = 1, AuthorId = 1, Body = "Test Blab 1" },
                new() { Id = 2, AuthorId = 1, Body = "Test Blab 2" },
                new() { Id = 3, AuthorId = 1, Body = "Test Blab 3" },
            };


            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.AddRange(blabs);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var pageNumber = 1;
                var pageSize = 2;

                var (Blabs, TotalCount) = await repository.GetAsync(pageNumber, pageSize);

                Assert.NotNull(Blabs);
                Assert.Equal(2, Blabs.Count());
                Assert.Equal(3, TotalCount);
                Assert.Contains(Blabs, b => b.Body == "Test Blab 2");
                Assert.Contains(Blabs, b => b.Body == "Test Blab 3");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBlab()
        {
            var authorId = 1;
            var blabId = 1;
            var user = new ApplicationUser { Id = "1", UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = "1", Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.GetByIdAsync(blabId);

                Assert.NotNull(result);
                Assert.Equal("Test Blab", result.Body);
            }

        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenBlabDoesNotExist()
        {
            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.GetByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_CreatesBlab()
        {
            var authorId = 1;
            var user = new ApplicationUser { Id = "1", UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = "1", Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var request = new BlabCreateRequest { AuthorId = authorId, Body = "Test Blab" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.AddAsync(request);

                Assert.NotNull(result);
                Assert.Equal("Test Blab", result.Body);
            }
        }

        [Fact]
        public async Task UpdateAsync_ModifiesBlab()
        {
            var authorId = 1;
            var blabId = 1;
            var user = new ApplicationUser { Id = "1", UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = "1", Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };
            var request = new BlabUpdateRequest { Id = blabId, Body = "Updated Blab" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.UpdateAsync(blabId, request);

                Assert.NotNull(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var result = await context.Blabs.FindAsync(blabId);

                Assert.NotNull(result);
                Assert.Equal("Updated Blab", result.Body);
            }
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenIdMismatch()
        {
            var blabId = 1;
            var wrongBlabId = 2;
            var request = new BlabUpdateRequest { Id = blabId, Body = "Updated Blab" };

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.UpdateAsync(wrongBlabId, request);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesBlab()
        {
            var authorId = 1;
            var blabId = 1;
            var user = new ApplicationUser { Id = "1", UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = "1", Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.DeleteAsync(blabId);

                Assert.NotNull(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var result = await context.Blabs.FindAsync(blabId);

                Assert.NotNull(result);
                Assert.True(result.IsDeleted);
            }
        }

        [Fact]
        public async Task AddLikeAsync_LikesBlab()
        {
            var userId = "1";
            var authorId = 1;
            var blabId = 1;
            var user = new ApplicationUser { Id = userId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = userId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                context.Blabs.Add(blab);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.AddLikeAsync(blabId, authorId);

                Assert.True(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var result = await context.Blabs
                    .Include(b => b.Liked)
                    .FirstOrDefaultAsync(b => b.Id == blabId);

                Assert.NotNull(result);
                Assert.Single(result.Liked);
            }
        }

        [Fact]
        public async Task RemoveLikeAsync_UnlikesBlab()
        {
            var userId = "1";
            var authorId = 1;
            var blabId = 1;
            var user = new ApplicationUser { Id = userId, UserName = "TestUser", Email = "test@user.com" };
            var author = new Author { Id = authorId, ApplicationUserId = userId, Handle = "TestHandle", DisplayName = "TestDisplayName" };
            var blab = new Blab { Id = blabId, AuthorId = authorId, Body = "Test Blab" };

            using (var context = _fixture.CreateContext())
            {
                context.Users.Add(user);
                context.Authors.Add(author);
                blab.Liked.Add(author);
                context.Blabs.Add(blab);
                await context.SaveChangesAsync();
            }

            using (var context = _fixture.CreateContext())
            {
                var repository = new BlabRepository(context);
                var result = await repository.RemoveLikeAsync(blabId, authorId);

                Assert.True(result);
            }

            using (var context = _fixture.CreateContext())
            {
                var result = await context.Blabs
                    .Include(b => b.Liked)
                    .FirstOrDefaultAsync(b => b.Id == blabId);

                Assert.NotNull(result);
                Assert.Empty(result.Liked);
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