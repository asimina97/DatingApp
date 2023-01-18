
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   //GET/ api/users
    public class UsersController : ControllerBase
    {

        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context; //injection

        }

        //API Method 1 get all users
        [HttpGet] //API request
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()  //Get users is a method that returns users
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }
        //API Method 2 get one specific user

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

    }
}