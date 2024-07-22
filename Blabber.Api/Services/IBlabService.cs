﻿using Blabber.Api.Models;

namespace Blabber.Api.Services
{
    public interface IBlabService
    {
        Task<BlabFeed> GetBlabFeedAsync(int pageNumber, int pageSize, int? authorId = null);
        Task<BlabView?> GetBlabByIdAsync(int id);
        Task<BlabView?> AddBlabAsync(BlabCreateRequest request);
        Task<BlabView?> UpdateBlabAsync(int id, BlabUpdateRequest request);
        Task<BlabView?> DeleteBlabAsync(int id);
        Task<bool> AddBlabLikeAsync(int blabId, int authorId);
        Task<bool> RemoveBlabLikeAsync(int blabId, int authorId);
    }
}