

using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController (DataContext context,ITokenService tokenService,IMapper mapper)
        {
            _tokenService = tokenService;
            _context = context;
            _mapper=mapper;
        
        }
         //method for register a new user 
        [HttpPost("register")]  //POST :api/account/register
       
        //we are goint to replace the parmeters string username,string password with DTO
        public async Task<ActionResult<UserDto>>Register(RegisterDto registerDto)
        { 
            //check if the user already exists
              if(await UserExists(registerDto.Username)) return BadRequest("User already exists");


            //GENERATE THE HASH PASSWORD/WE CREATE A NEW INSTANCE IN THE CLASS
            var user=_mapper.Map<AppUser>(registerDto);
            using var hmac=new HMACSHA512();
            
            
                user.UserName=registerDto.Username.ToLower();
                user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                user.PasswordSalt=hmac.Key;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username=user.UserName,
                Token=_tokenService.CreateToken(user)
            };
        }
        //method to login user
        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
             var user = await _context.Users
             .Include(p=>p.Photos)
             .SingleOrDefaultAsync(x=>x.UserName==loginDto.Username);
             if (user == null) return Unauthorized("invalid username");


             using var hmac = new HMACSHA512(user.PasswordSalt);
             var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

             for (int i=0;i<computedHash.Length;i++)
             {
                if (computedHash[i]!=user.PasswordHash[i]) return Unauthorized("invalid password");
             }
              return new UserDto
            {
                Username=user.UserName,
                Token=_tokenService.CreateToken(user),
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.isMain)?.Url
            };
        }

        //method to check if the user already esixts
        private async Task<bool> UserExists(string username)
        {
           return await _context.Users.AnyAsync(x=>x.UserName == username.ToLower());
        }
    }
}