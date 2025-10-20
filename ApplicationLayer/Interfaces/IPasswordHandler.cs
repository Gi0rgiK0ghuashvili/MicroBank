namespace ApplicationLayer.Interfaces
{
    public interface IPasswordHandler
    {
        public void CreateSaltAndHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string password, in byte[] passwordHash, in byte[] passwordSalt);
    }
}
