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
        public async Task GetAllAsync_ReturnsAuthors()
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