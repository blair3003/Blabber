﻿namespace Blabber.Api.Models
{
    public class AuthorView
    {
        public int Id { get; set; }
        public string? Handle { get; set; }
        public string? DisplayName { get; set; }
        public string? DisplayPic { get; set; }
    }
}