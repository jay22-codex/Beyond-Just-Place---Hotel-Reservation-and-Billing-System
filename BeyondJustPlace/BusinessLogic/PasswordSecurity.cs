using System;
using System.Security.Cryptography;

namespace BusinessLogic
{
    // Backward-compatible PBKDF2 migration: older local demo accounts are upgraded
    // from plaintext on their first successful login, without resetting accounts.
    public static class PasswordSecurity
    {
        private const int Iterations=100000;
        public static string Hash(string password)
        {
            if(string.IsNullOrWhiteSpace(password) || password.Length<8)
                throw new InvalidOperationException("Password must have at least 8 characters.");
            byte[] salt=new byte[16];
            using(RandomNumberGenerator rng=RandomNumberGenerator.Create())rng.GetBytes(salt);
            using(Rfc2898DeriveBytes pbkdf=new Rfc2898DeriveBytes(password,salt,Iterations))
                return "PBKDF2$"+Iterations+"$"+Convert.ToBase64String(salt)+"$"+Convert.ToBase64String(pbkdf.GetBytes(32));
        }
        public static bool Verify(string password,string saved)
        {
            if(saved==null)return false;
            if(!saved.StartsWith("PBKDF2$",StringComparison.Ordinal))
                return string.Equals(password,saved,StringComparison.Ordinal);
            string[] parts=saved.Split('$');
            int rounds;
            if(parts.Length!=4 || !int.TryParse(parts[1],out rounds) || rounds<10000)return false;
            try
            {
                byte[] salt=Convert.FromBase64String(parts[2]), stored=Convert.FromBase64String(parts[3]);
                using(Rfc2898DeriveBytes pbkdf=new Rfc2898DeriveBytes(password,salt,rounds))
                {
                    byte[] calc=pbkdf.GetBytes(stored.Length);
                    int mismatch=0;
                    for(int i=0;i<stored.Length;i++)mismatch |= calc[i]^stored[i];
                    return mismatch==0;
                }
            }
            catch(FormatException){return false;}
        }
        public static bool IsLegacy(string saved){return saved!=null && !saved.StartsWith("PBKDF2$",StringComparison.Ordinal);}
    }
}
