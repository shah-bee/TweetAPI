using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPI.Data;
using TweetAPI.Domain;

namespace TweetAPI.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDataContext dataContext;

        public PostService(ApplicationDataContext dbContext)
        {
            dataContext = dbContext;
        }

        public async Task<bool> CreatePostAsync(Post postToUpdate)
        {
            await dataContext.Posts.AddAsync(postToUpdate);
            var result = await dataContext.SaveChangesAsync();
            if (result > 0)
                return true;
            return false;
        }

        async Task<bool> IPostService.DeletePostAsync(Guid postId)
        {
            var post = await dataContext.Posts.SingleOrDefaultAsync(o => o.Id.Equals(postId));
            dataContext.Posts.Remove(post);
            var updated = await dataContext.SaveChangesAsync();
            if (updated > 0)
                return true;
            return false;
        }

        async Task<List<Post>> IPostService.GetAllAsync()
        {
            return await dataContext.Posts.ToListAsync();
        }

        async Task<Post> IPostService.GetPostAsync(Guid postId)
        {
            return await dataContext.Posts.SingleOrDefaultAsync(p => p.Id.Equals(postId));
        }

        async Task<bool> IPostService.UpdatePostAsync(Guid postId, Post postToUpdate)
        {
            dataContext.Posts.Update(postToUpdate);
            var updated = await dataContext.SaveChangesAsync();
            if (updated > 0)
                return true;
            return false;
           
        }
    }
}
