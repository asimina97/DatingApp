
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
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

        private readonly IUserRepository _userRepository; 
        //Instead of DataContext we gonna inject the RepositoryInterface
        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }

        //API Method 1 get all users
        [AllowAnonymous]
        [HttpGet] //API request
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()  //Get users is a method that returns users
        {
            var users=await _userRepository.GetMembersAsync();
         
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
            var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user=await _userRepository.GetUserByUsernameAsync(username);

            if (user==null) return NotFound();

            //update user
            _mapper.Map(memberUpdateDto,user);
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");


        }

    }
}