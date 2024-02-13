using Flunt.Validations;

namespace diceclub_api_netcore.Dtos
{
    public class UserDto : Contract<UserDto>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }

        public UserDto(string username, string email, string password, string rePassword)
        {
            UserName = username;
            Email = email;
            Password = password;
            RePassword = rePassword;

            Requires().IsTrue(ValidUserNameOrEmail(), "User name", "Valid username or email must be provided");
            Requires().IsNotNullOrEmpty(Password, "Password", "Invalid password");
            Requires().AreEquals(Password, RePassword, "Password", "Invalid password confirmation");
        }

        private bool ValidUserNameOrEmail()
        {
            var userNameValid = !string.IsNullOrEmpty(UserName) && !UserName.Any(char.IsWhiteSpace) && UserName.Length > 3;
            var emailValid = Requires().IsEmail(Email, nameof(Email)).IsValid;

            return userNameValid || emailValid;
        }
    }
}
