using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(x => x.LikerId == userId && x.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.FirstOrDefaultAsync(x => x.UserId == userId && x.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var userQuery = _context.Users
                .Include(x => x.Photos)
                .OrderByDescending(x => x.LastActive)
                .AsQueryable();

            userQuery = userQuery.Where(x => x.Id != userParams.UserId);
            userQuery = userQuery.Where(x => x.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                userQuery = userQuery.Where(x => userLikers.Contains(x.Id));
            }
            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                userQuery = userQuery.Where(x => userLikees.Contains(x.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                const int numberOfCurrentYear = 1;
                var minDataOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - numberOfCurrentYear);
                var maxDataOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

                userQuery = userQuery.Where(x => x.DateOfBirth >= minDataOfBirth && x.DateOfBirth <= maxDataOfBirth);
            }
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                // anti-pattern from udemy course [*]
                switch (userParams.OrderBy)
                {
                    case "created":
                        userQuery = userQuery.OrderByDescending(x => x.Created);
                        break;
                    default:
                        userQuery = userQuery.OrderByDescending(x => x.LastActive);
                    break;
                }
            }

            return await PagedList<User>.CreateAsync(userQuery, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            IEnumerable<int> userLikes;

            var user = await _context.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (likers)
            {
                userLikes = user.Likers.Where(x => x.LikeeId == id).Select(x => x.LikerId);
            }
            else
            {
                userLikes = user.Likees.Where(x => x.LikerId == id).Select(x => x.LikeeId);
            }

            return userLikes;
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}