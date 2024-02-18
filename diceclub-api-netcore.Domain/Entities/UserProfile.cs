using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace diceclub_api_netcore.Domain.Entities
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }

        //Required DBcontext reference navigation to parent, One to One relation
        public User User { get; set; } = null!;

    }
}
