using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext context;

        public DatingRepository(DataContext context)
        {
            this.context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await context.Likes.FirstOrDefaultAsync(x => x.LikerId == userId && x.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await context.Photos.FirstOrDefaultAsync(x => x.UserId == userId && x.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await context.Photos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetUser(int id)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var userQuery = context.Users
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

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

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
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messagesQuery = context.Messages.AsQueryable();

            // anti-pattern from udemy course [*]
            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messagesQuery = messagesQuery
                        .Where(x => x.RecipientId == messageParams.UserId)
                        .Where(x => x.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messagesQuery = messagesQuery
                        .Where(x => x.SenderId == messageParams.UserId)
                        .Where(x => x.SenderDeleted == false);
                    break;
                default:
                    messagesQuery = messagesQuery
                        .Where(x => x.RecipientId == messageParams.UserId)
                        .Where(x => x.RecipientDeleted == false)
                        .Where(x => x.IsRead == false);
                    break;
            }

            messagesQuery = messagesQuery.OrderByDescending(x => x.MessageSent);
            return await PagedList<Message>.CreateAsync(messagesQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            return await context.Messages
                .Where(x => (x.RecipientId == userId && x.SenderId == recipientId && x.RecipientDeleted == false)
                    || (x.RecipientId == recipientId && x.SenderId == userId && x.SenderDeleted == false))
                .OrderByDescending(x => x.MessageSent)
                .ToListAsync();
        }
    }
}