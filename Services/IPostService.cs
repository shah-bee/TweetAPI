using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPI.Controllers.V1.Requests;
using TweetAPI.Domain;

namespace TweetAPI.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllAsync();

        Task<Post> GetPostAsync(Guid postId);

        Task<bool> UpdatePostAsync(Guid postId, Post postToUpdate);

        Task<bool> CreatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);
    }
}
