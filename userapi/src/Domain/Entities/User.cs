using System;

namespace UserApi.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        protected User() { } // EF

        public User(string name, string email, string passwordHash)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email.ToLowerInvariant();
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdatePasswordHash(string newHash) => PasswordHash = newHash;
    }
}
