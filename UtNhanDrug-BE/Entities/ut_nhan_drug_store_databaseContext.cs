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

        public virtual DbSet<ActiveSubstance> ActiveSubstances { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<DosageUnit> DosageUnits { get; set; }
        public virtual DbSet<EmailValidationStatus> EmailValidationStatuses { get; set; }
        public virtual DbSet<ExternalProvider> ExternalProviders { get; set; }
        public virtual DbSet<HashingAlgorithm> HashingAlgorithms { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductActiveSubstance> ProductActiveSubstances { get; set; }
        public virtual DbSet<SamplePrescription> SamplePrescriptions { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
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

            modelBuilder.Entity<ActiveSubstance>(entity =>
            {
                entity.ToTable("active_substances");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ActiveSubstanceCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__active_su__creat__10566F31");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ActiveSubstanceUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__active_su__updat__114A936A");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brands");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.BrandCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__brands__created___71D1E811");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.BrandUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__brands__updated___72C60C4A");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CategoryCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__categorie__creat__778AC167");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.CategoryUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__categorie__updat__787EE5A0");
            });

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

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.ToTable("diseases");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.DiseaseCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__diseases__create__2B0A656D");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.DiseaseUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__diseases__update__2BFE89A6");
            });

            modelBuilder.Entity<DosageUnit>(entity =>
            {
                entity.ToTable("dosage_units");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");
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

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.HasIndex(e => e.Barcode, "UQ__products__C16E36F83E5A2D0C")
                    .IsUnique();

                entity.HasIndex(e => e.DrugRegistrationNumber, "UQ__products__EF7E09095E2CC137")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Barcode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("barcode");

                entity.Property(e => e.BrandId).HasColumnName("brand_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.Dosage)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("dosage");

                entity.Property(e => e.DosageUnitId).HasColumnName("dosage_unit_id");

                entity.Property(e => e.DrugRegistrationNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("drug_registration_number");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MinimumQuantity)
                    .HasColumnName("minimum_quantity")
                    .HasDefaultValueSql("((30))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("smallmoney")
                    .HasColumnName("price");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__brand___01142BA1");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__catego__02084FDA");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProductCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__create__0A9D95DB");

                entity.HasOne(d => d.DosageUnit)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.DosageUnitId)
                    .HasConstraintName("FK__products__dosage__05D8E0BE");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK__products__unit_i__06CD04F7");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ProductUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__products__update__0B91BA14");
            });

            modelBuilder.Entity<ProductActiveSubstance>(entity =>
            {
                entity.ToTable("product_active_substance");

                entity.HasIndex(e => new { e.ProductId, e.ActiveSubstanceId }, "UQ__product___12D3DF52AB12EDDE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveSubstanceId).HasColumnName("active_substance_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.ActiveSubstance)
                    .WithMany(p => p.ProductActiveSubstances)
                    .HasForeignKey(d => d.ActiveSubstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__activ__160F4887");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductActiveSubstances)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__produ__151B244E");
            });

            modelBuilder.Entity<SamplePrescription>(entity =>
            {
                entity.ToTable("sample_prescriptions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CustomerWeight)
                    .HasColumnType("decimal(3, 2)")
                    .HasColumnName("customer_weight");

                entity.Property(e => e.DiseaseId).HasColumnName("disease_id");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__creat__31B762FC");

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.SamplePrescriptions)
                    .HasForeignKey(d => d.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__disea__2EDAF651");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__sample_pr__updat__32AB8735");
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

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("units");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");
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
