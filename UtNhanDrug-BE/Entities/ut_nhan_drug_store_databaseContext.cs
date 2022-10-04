using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace UtNhanDrug_BE.Entities
{
    public partial class ut_nhan_drug_store_databaseContext : DbContext
    {
        public ut_nhan_drug_store_databaseContext()
        {
        }

        public ut_nhan_drug_store_databaseContext(DbContextOptions<ut_nhan_drug_store_databaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<EmailValidationStatus> EmailValidationStatuses { get; set; }
        public virtual DbSet<ExternalProvider> ExternalProviders { get; set; }
        public virtual DbSet<HashingAlgorithm> HashingAlgorithms { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<UserLoginDataExternal> UserLoginDataExternals { get; set; }
        public virtual DbSet<UserLoginDatum> UserLoginData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:utNhanDrug");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.HasIndex(e => e.UserAccountId, "UQ__customer__1918BBDB1E338215")
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber, "UQ__customer__A1936A6B4B9B1642")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__customers__user___440B1D61");
            });

            modelBuilder.Entity<EmailValidationStatus>(entity =>
            {
                entity.ToTable("email_validation_statuses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.StatusDescription)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status_description");
            });

            modelBuilder.Entity<ExternalProvider>(entity =>
            {
                entity.ToTable("external_providers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.WebServiceEndPoint)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("web_service_end_point");
            });

            modelBuilder.Entity<HashingAlgorithm>(entity =>
            {
                entity.ToTable("hashing_algorithms");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.ToTable("managers");

                entity.HasIndex(e => e.UserAccountId, "UQ__managers__1918BBDBBB89E3E2")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.Manager)
                    .HasForeignKey<Manager>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__managers__user_a__3B75D760");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("staffs");

                entity.HasIndex(e => e.UserAccountId, "UQ__staffs__1918BBDB89EAE2D8")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .IsRequired()
                    .HasMaxLength(2100)
                    .HasColumnName("avatar");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.IsMale).HasColumnName("is_male");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.Staff)
                    .HasForeignKey<Staff>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__staffs__user_acc__3F466844");
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("user_accounts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserLoginDataExternal>(entity =>
            {
                entity.ToTable("user_login_data_external");

                entity.HasIndex(e => e.UserAccountId, "UQ__user_log__1918BBDBA0511BB2")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ExternalProviderId).HasColumnName("external_provider_id");

                entity.Property(e => e.ExternalProviderToken)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("external_provider_token");

                entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.UserLoginDataExternal)
                    .HasForeignKey<UserLoginDataExternal>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_logi__exter__5629CD9C");

                entity.HasOne(d => d.UserAccountNavigation)
                    .WithOne(p => p.UserLoginDataExternal)
                    .HasForeignKey<UserLoginDataExternal>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_logi__user___5535A963");
            });

            modelBuilder.Entity<UserLoginDatum>(entity =>
            {
                entity.ToTable("user_login_data");

                entity.HasIndex(e => e.UserAccountId, "UQ__user_log__1918BBDB73C9D757")
                    .IsUnique();

                entity.HasIndex(e => e.LoginName, "UQ__user_log__F6D56B57829DFC92")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConfirmationToken)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("confirmation_token");

                entity.Property(e => e.EmailAddressRecovery)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email_address_recovery");

                entity.Property(e => e.EmailValidationStatusId)
                    .HasColumnName("email_validation_status_id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.HashingAlgorithmId).HasColumnName("hashing_algorithm_id");

                entity.Property(e => e.LoginName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("login_name");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("password_hash");

                entity.Property(e => e.PasswordRecoveryToken)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password_recovery_token");

                entity.Property(e => e.RecoveryTokenTime)
                    .HasColumnType("datetime")
                    .HasColumnName("recovery_token_time");

                entity.Property(e => e.TokenGenerationTime)
                    .HasColumnType("datetime")
                    .HasColumnName("token_generation_time");

                entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");

                entity.HasOne(d => d.EmailValidationStatus)
                    .WithMany(p => p.UserLoginData)
                    .HasForeignKey(d => d.EmailValidationStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_logi__email__4F7CD00D");

                entity.HasOne(d => d.HashingAlgorithm)
                    .WithMany(p => p.UserLoginData)
                    .HasForeignKey(d => d.HashingAlgorithmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_logi__hashi__4D94879B");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.UserLoginDatum)
                    .HasForeignKey<UserLoginDatum>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_logi__user___4CA06362");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
