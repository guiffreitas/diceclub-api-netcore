using Flunt.Validations;

namespace diceclub_api_netcore.Dtos
{
    public class UserProfileDto : Contract<UserProfileDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;

        public UserProfileDto(string name, string surname, string about)
        {
            Name = name;
            Surname = surname;
            About = about;

            Requires().IsTrue(Name.Length > 3, nameof(Name), "Invalid");
            Requires().IsTrue(Surname.Length > 3, nameof(Surname), "Invalid");
            Requires().IsTrue(About.Length > 3, nameof(About), "Invalid");
        }
    }
}
