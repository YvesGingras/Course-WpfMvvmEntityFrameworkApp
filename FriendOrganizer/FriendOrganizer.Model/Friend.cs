using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Model
{
    public class Friend
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        [EmailAddress] 
        public string Email { get; set; }

        public int? FavoriteLanguageId { get; set; }

        public ProgrammingLanguage FavoriteLanguage { get; set; }
    }
}
