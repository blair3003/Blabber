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

        public static Author ToAuthor(this AuthorCreateRequest request)
        {
            return new Author
            {
                ApplicationUserId = request.ApplicationUserId,
                Handle = request.Handle,
                DisplayName = request.DisplayName,
                DisplayPic = request.DisplayPic
            };
        }

        public static AuthorEditRequest ToAuthorEditRequest(this Author author)
        {
            return new AuthorEditRequest
            {
                Handle = author.Handle,
                DisplayName = author.DisplayName,
                DisplayPic = author.DisplayPic
            };
        }

        public static void EditAuthor(this Author author, AuthorEditRequest request)
        {
            author.Handle = request.Handle;
            author.DisplayName = request.DisplayName;
            author.DisplayPic = request.DisplayPic;
        }
    }
}
