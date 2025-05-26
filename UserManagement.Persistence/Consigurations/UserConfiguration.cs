using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UserManagement.Core.Models;

namespace UserManagement.Persistence.Consigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName(PersistentsConstants.PK_USER_INDEX);

            builder.Property(e => e.Id)
                .HasColumnName(PersistentsConstants.USER_ID);

            builder.Property(e => e.Name)
                .IsRequired(true)
                .HasMaxLength(PersistentsConstants.NAME_LENGTH);

            builder.Property(e => e.Email)
                .IsRequired(true)
                .HasMaxLength(PersistentsConstants.EMAIL_LENGTH);

            builder.HasIndex(e => e.Email)
                .IsUnique(true)
                .HasDatabaseName(PersistentsConstants.EMAIL_INDEX_NAME);

            builder.Property(e => e.PasswordHash)
                .IsRequired(true)
                .HasMaxLength(PersistentsConstants.PASSWORD_HASH_LENGTH);

            builder.Property(e => e.RegistrationDate)
                .IsRequired(true);

            builder.Property(e => e.LastLoginDate)
                .IsRequired(false);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}
