using Blabber.Api.Data;
using Blabber.Api.Models;
using Blabber.Api.Repositories;
using CommunityApp.Tests.Fixtures;

namespace Blabber.Tests.Integration
{
    public class BlabRepositoryTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly DatabaseFixture _fixture = fixture;

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

        public void Dispose()
        {
            using (var context = _fixture.CreateContext())
            {
                context.Blabs.RemoveRange(context.Blabs);
                context.Authors.RemoveRange(context.Authors);
                context.Users.RemoveRange(context.Users);
                context.SaveChanges();
            }

            using (var context = _fixture.CreateContext())
            {
                if (context.Blabs.Any() || context.Authors.Any() || context.Users.Any())
                {
                    throw new InvalidOperationException("Failed to clear data.");
                }
            }
        }

    }
}