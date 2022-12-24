using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
        }

        // [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers([FromQuery] UserParams userParams)
        {
            var gender = await _unitOfWork.UserRepository.GetGenderbyUserNameAsync(User.GetUserName());
            userParams.CurrentUserName = User.GetUserName();

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,users.TotalCount,users.TotalPages));
            //var userToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            //return Ok(userToReturn);
            return Ok(users);

            //here we have to wrap the response within OK. only for list case
        }

        
        [HttpGet("{username}", Name ="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            return _mapper.Map<MemberDto>(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser (MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUserName();
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            _mapper.Map(memberUpdateDto,user);
            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.CompleteAsync()) return NoContent();
            return BadRequest();

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
               Url = result.SecureUrl.AbsoluteUri,
               PublicId = result.PublicId     
            }; 

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _unitOfWork.CompleteAsync())
                return CreatedAtAction("GetUser", new {username = user.UserName},_mapper.Map<PhotoDto>(photo)) ;

            return BadRequest("Problem adding photo");
                
         }

         [HttpPut("set-main-photo/{photoId}")]
         public async Task<ActionResult> setMainPhoto(int photoId)
         {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null)
                currentMain.IsMain = false;

            photo.IsMain = true;    

            if (await _unitOfWork.CompleteAsync()) return NoContent();    

            return BadRequest("Failed to set main photo");
         }

         [HttpDelete("delete-photo/{photoId}")]
         public async Task<ActionResult> DeletePhoto(int photoId)
         {
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return  BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _unitOfWork.CompleteAsync()) return Ok();

            return BadRequest();
         }
    }
}



