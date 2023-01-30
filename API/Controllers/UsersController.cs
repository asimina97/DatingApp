
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Claims;
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
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        private readonly IUserRepository _userRepository; 
        //Instead of DataContext we gonna inject the RepositoryInterface
        public UsersController(IUserRepository userRepository,IMapper mapper,
        IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;

        }

        //API Method 1 get all users
        [AllowAnonymous]
        [HttpGet] //API request
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)  //Get users is a method that returns users
        {
            var currentUser=await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            userParams.CurrentUsername=currentUser.UserName;
            if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender=currentUser.Gender=="male" ? "female" : "male";
            }
            var users=await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
         
            return Ok(users);

        }
        //API Method 2 get one specific user

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);

           
        }

        //API method to update user
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user=await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            if (user==null) return NotFound();

            //update user
            _mapper.Map(memberUpdateDto,user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");


        }

        //API method to post pictures-HttpPost(rootParameter)
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user=await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            if(user==null) return NotFound();
            var result=await _photoService.AddPhotoAsync(file);
            if(result.Error !=null) return BadRequest(result.Error.Message);

            var photo=new Photo
            {
                Url=result.SecureUrl.AbsoluteUri,
                PublicId=result.PublicId
            };
            if(user.Photos.Count==0) photo.isMain=true;

            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync()){
                return CreatedAtAction(nameof(GetUser), new {username=user.UserName},_mapper.Map<PhotoDto>(photo));
            } 
            return BadRequest("Problem adding photo");




        }
        //method to set the main photo
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            //See ClaimsPrincipalExtemsions
            var user=await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            if(user==null) return NotFound();
            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if (photo==null) return NotFound();
            if(photo.isMain) return BadRequest("This is already your main photo");

            var currentMain=user.Photos.FirstOrDefault(x=>x.isMain);
            if(currentMain!=null) currentMain.isMain=false;
            photo.isMain=true;

            if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Problem setting main photo");

        }

        //Method to delete photo
        [HttpDelete("delete-photo/{photoId}")]
            public async Task<ActionResult> DeletePhoto(int photoId){
                var user= await _userRepository. GetUserByUsernameAsync(User.GetUserName());
                var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
                if (photo==null) return NotFound();
                if(photo.isMain) return BadRequest("You cannot delete main photo");
                if(photo.PublicId !=null){
                    var result=await _photoService.DeletePhotoAsync(photo.PublicId);
                    if (result.Error!=null) return BadRequest(result.Error.Message);
                }

                user.Photos.Remove(photo);
                if(await _userRepository.SaveAllAsync()) return Ok();
                return BadRequest("Problem deleting photo");
            }
        }


    }
