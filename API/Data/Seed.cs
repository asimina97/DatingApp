

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {

        public static async Task SeedUsers(DataContext context)
        {
            //check if we have any users
            if(await context.Users.AnyAsync()) return;
            //read from the generated json file
            var userData=await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options =new JsonSerializerOptions{PropertyNameCaseInsensitive=true};

            //JSON to C# object
            var users=JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)
            {
                using var hmac=new HMACSHA512();

                user.UserName=user.UserName.ToLower();
                user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt=hmac.Key;
                context.Users.Add(user);
            }

            //Save users data in database

            await context.SaveChangesAsync();
        }
        



    }
}