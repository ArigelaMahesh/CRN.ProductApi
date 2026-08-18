    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Domain.Entities
    {
        public class User
        {
            public int Id { get; set; }

            public string UserName { get; set; } = string.Empty;

            public string PasswordHash { get; set; } = string.Empty;

            public string Role { get; set; } = "User";

            public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        }
    }
