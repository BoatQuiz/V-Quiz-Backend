using V_Quiz_Backend.Interface;

namespace V_Quiz_Backend.Services
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);


        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
