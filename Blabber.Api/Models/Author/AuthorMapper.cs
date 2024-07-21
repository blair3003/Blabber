namespace Blabber.Api.Models
{
    public static class AuthorMapper
    {
        public static AuthorView ToView(this Author author)
        {
            return new AuthorView
            {
                Id = author.Id,
                Handle = author.Handle,
                DisplayName = author.DisplayName,
                DisplayPic = author.DisplayPic
            };
        }

        public static IEnumerable<AuthorView> ToViewList(this IEnumerable<Author> authors)
        {
            return authors.Select(author => author.ToView()).ToList();
        }

        public static Author ToAuthor(this AuthorCreateRequest request, string applicationUserId)
        {
            return new Author
            {
                ApplicationUserId = applicationUserId,
                Handle = request.Handle,
                DisplayName = request.DisplayName,
                DisplayPic = request.DisplayPic
            };
        }

        public static AuthorUpdateRequest ToAuthorUpdateRequest(this Author author)
        {
            return new AuthorUpdateRequest
            {
                Id = author.Id,
                Handle = author.Handle,
                DisplayName = author.DisplayName,
                DisplayPic = author.DisplayPic
            };
        }

        public static void UpdateAuthor(this Author author, AuthorUpdateRequest request)
        {
            author.Handle = request.Handle;
            author.DisplayName = request.DisplayName;
            author.DisplayPic = request.DisplayPic ?? author.DisplayPic;
        }
    }
}