using Flunt.Validations;

namespace diceclub_api_netcore.Dtos
{
    public class UserDto : Contract<UserDto>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }

        public UserDto(string username, string email, DateTime birthDate, string password, string rePassword)
        {
            UserName = username;
            Email = email;
            BirthDate = birthDate;
            Password = password;
            RePassword = rePassword;

            Requires().IsTrue(ValidUserName(), nameof(UserName), "Invalid") ;
            Requires().IsEmail(Email, nameof(Email), "Invalid");
            Requires().IsTrue(ValidBirthDate(), nameof(BirthDate), "Invalid");
            Requires().IsNotNullOrEmpty(Password, nameof(Password), "Invalid");
            Requires().AreEquals(Password, RePassword, nameof(rePassword), "Invalid");
        }

        private bool ValidUserName()
        {
            return !string.IsNullOrEmpty(UserName) && !UserName.Any(char.IsWhiteSpace) && UserName.Length > 3;
        }

        private bool ValidBirthDate()
        {
            var existValue = Requires().IsNotNull(BirthDate, nameof(BirthDate)).IsValid;
            var notFutureDate = Requires().IsTrue(BirthDate < DateTime.UtcNow, nameof(BirthDate)).IsValid;

            return existValue && notFutureDate;
        }
    }
}
