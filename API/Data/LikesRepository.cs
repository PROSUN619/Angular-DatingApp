using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParmas)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likesParmas.Predicate == "liked")
            {
                likes = likes.Where(l => l.SourceUserId == likesParmas.UserId);
                users = likes.Select(l => l.TargetUser);
            }

            if (likesParmas.Predicate == "likedBy")
            {
                likes = likes.Where(l => l.TargetUserId == likesParmas.UserId);
                users = likes.Select(l => l.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto()
            {
                UserName = user.UserName,
                knownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(f => f.IsMain).Url,    
                City = user.City,
                Id = user.Id    
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers,likesParmas.PageNumber, likesParmas.PageSize);

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(u => u.LikedUser)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}