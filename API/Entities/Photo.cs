using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool isMain { get; set; }

        public string PublicId { get; set; }

        //We specify the entity framework relationship between user and photo.In this case dotnet ef migrations is not enough.
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}