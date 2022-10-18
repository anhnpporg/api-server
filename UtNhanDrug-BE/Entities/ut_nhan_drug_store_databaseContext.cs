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
        public virtual DbSet<Consignment> Consignments { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<EmailValidationStatus> EmailValidationStatuses { get; set; }
        public virtual DbSet<ExternalProvider> ExternalProviders { get; set; }
        public virtual DbSet<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
        public virtual DbSet<GoodsReceiptNoteLog> GoodsReceiptNoteLogs { get; set; }
        public virtual DbSet<GoodsReceiptNoteType> GoodsReceiptNoteTypes { get; set; }
        public virtual DbSet<HashingAlgorithm> HashingAlgorithms { get; set; }
        public virtual DbSet<InventorySystemReport> InventorySystemReports { get; set; }
        public virtual DbSet<InventorySystemReportType> InventorySystemReportTypes { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductActiveSubstance> ProductActiveSubstances { get; set; }
        public virtual DbSet<ProductUnit> ProductUnits { get; set; }
        public virtual DbSet<RouteOfAdministration> RouteOfAdministrations { get; set; }
        public virtual DbSet<SamplePrescription> SamplePrescriptions { get; set; }
        public virtual DbSet<SamplePrescriptionDetail> SamplePrescriptionDetails { get; set; }
        public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<StockStrengthUnit> StockStrengthUnits { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
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
                    .HasConstraintName("FK__active_su__creat__01142BA1");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ActiveSubstanceUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__active_su__updat__02084FDA");
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
                    .HasConstraintName("FK__brands__created___5AEE82B9");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.BrandUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__brands__updated___5BE2A6F2");
            });

            modelBuilder.Entity<Consignment>(entity =>
            {
                entity.ToTable("consignments");

                entity.HasIndex(e => e.Barcode, "UQ__consignm__C16E36F89266A89B")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Barcode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("barcode");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("date")
                    .HasColumnName("expiry_date");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ManufacturingDate)
                    .HasColumnType("date")
                    .HasColumnName("manufacturing_date");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Consignments)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__consignme__creat__2BFE89A6");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Consignments)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__consignme__produ__282DF8C2");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.Consignments)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__consignme__updat__2CF2ADDF");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.HasIndex(e => e.UserAccountId, "UQ__customer__1918BBDBB6E0DF94")
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber, "UQ__customer__A1936A6BD705AECF")
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
                    .HasMaxLength(500)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.DiseaseCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__diseases__create__14270015");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.DiseaseUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__diseases__update__151B244E");
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

            modelBuilder.Entity<GoodsReceiptNote>(entity =>
            {
                entity.ToTable("goods_receipt_notes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConsignmentId).HasColumnName("consignment_id");

                entity.Property(e => e.ConvertedQuantity).HasColumnName("converted_quantity");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.GoodsReceiptNoteTypeId).HasColumnName("goods_receipt_note_type_id");

                entity.Property(e => e.PurchasePrice)
                    .HasColumnType("smallmoney")
                    .HasColumnName("purchase_price");

                entity.Property(e => e.PurchasePriceBaseUnit)
                    .HasColumnType("smallmoney")
                    .HasColumnName("purchase_price_base_unit");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.HasOne(d => d.Consignment)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.ConsignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__consi__3864608B");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__creat__3D2915A8");

                entity.HasOne(d => d.GoodsReceiptNoteType)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.GoodsReceiptNoteTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__goods__37703C52");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__goods_rec__suppl__395884C4");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__unit___3B40CD36");
            });

            modelBuilder.Entity<GoodsReceiptNoteLog>(entity =>
            {
                entity.ToTable("goods_receipt_note_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GoodsReceiptNoteId).HasColumnName("goods_receipt_note_id");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("note");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.GoodsReceiptNote)
                    .WithMany(p => p.GoodsReceiptNoteLogs)
                    .HasForeignKey(d => d.GoodsReceiptNoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__goods__40058253");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.GoodsReceiptNoteLogs)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__goods_rec__updat__41EDCAC5");
            });

            modelBuilder.Entity<GoodsReceiptNoteType>(entity =>
            {
                entity.ToTable("goods_receipt_note_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");
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

            modelBuilder.Entity<InventorySystemReport>(entity =>
            {
                entity.ToTable("inventory_system_reports");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConsignmentId).HasColumnName("consignment_id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.InventorySystemReportTypeId).HasColumnName("inventory_system_report_type_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("title");

                entity.HasOne(d => d.Consignment)
                    .WithMany(p => p.InventorySystemReports)
                    .HasForeignKey(d => d.ConsignmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__inventory__consi__47A6A41B");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InventorySystemReports)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__inventory__creat__498EEC8D");

                entity.HasOne(d => d.InventorySystemReportType)
                    .WithMany(p => p.InventorySystemReports)
                    .HasForeignKey(d => d.InventorySystemReportTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__inventory__inven__46B27FE2");
            });

            modelBuilder.Entity<InventorySystemReportType>(entity =>
            {
                entity.ToTable("inventory_system_report_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.ToTable("managers");

                entity.HasIndex(e => e.UserAccountId, "UQ__managers__1918BBDB6BDAEC94")
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

                entity.HasIndex(e => e.Barcode, "UQ__products__C16E36F8A21C76E2")
                    .IsUnique();

                entity.HasIndex(e => e.DrugRegistrationNumber, "UQ__products__EF7E0909AB378FB4")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Barcode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("barcode");

                entity.Property(e => e.BrandId).HasColumnName("brand_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.DrugRegistrationNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("drug_registration_number");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsConsignment).HasColumnName("is_consignment");

                entity.Property(e => e.IsMedicine).HasColumnName("is_medicine");

                entity.Property(e => e.MinimumQuantity).HasColumnName("minimum_quantity");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.RouteOfAdministrationId).HasColumnName("route_of_administration_id");

                entity.Property(e => e.ShelfId).HasColumnName("shelf_id");

                entity.Property(e => e.StockStrength)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("stock_strength");

                entity.Property(e => e.StockStrengthUnitId).HasColumnName("stock_strength_unit_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__brand___6A30C649");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProductCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__create__70DDC3D8");

                entity.HasOne(d => d.RouteOfAdministration)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.RouteOfAdministrationId)
                    .HasConstraintName("FK__products__route___6E01572D");

                entity.HasOne(d => d.Shelf)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ShelfId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__shelf___6B24EA82");

                entity.HasOne(d => d.StockStrengthUnit)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StockStrengthUnitId)
                    .HasConstraintName("FK__products__stock___6D0D32F4");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ProductUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__products__update__71D1E811");
            });

            modelBuilder.Entity<ProductActiveSubstance>(entity =>
            {
                entity.ToTable("product_active_substance");

                entity.HasIndex(e => new { e.ProductId, e.ActiveSubstanceId }, "UQ__product___12D3DF524BF08665")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveSubstanceId).HasColumnName("active_substance_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.ActiveSubstance)
                    .WithMany(p => p.ProductActiveSubstances)
                    .HasForeignKey(d => d.ActiveSubstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__activ__06CD04F7");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductActiveSubstances)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__produ__05D8E0BE");
            });

            modelBuilder.Entity<ProductUnit>(entity =>
            {
                entity.ToTable("product_unit");

                entity.HasIndex(e => new { e.ProductId, e.UnitId }, "UQ__product___2A3888490BC56630")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConversionValue)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("conversion_value");

                entity.Property(e => e.IsBaseUnit).HasColumnName("is_base_unit");

                entity.Property(e => e.Price)
                    .HasColumnType("smallmoney")
                    .HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductUnits)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_u__produ__0C85DE4D");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.ProductUnits)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_u__unit___0D7A0286");
            });

            modelBuilder.Entity<RouteOfAdministration>(entity =>
            {
                entity.ToTable("route_of_administrations");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");
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

                entity.Property(e => e.DiseaseId).HasColumnName("disease_id");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__creat__1AD3FDA4");

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.SamplePrescriptions)
                    .HasForeignKey(d => d.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__disea__17F790F9");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__sample_pr__updat__1BC821DD");
            });

            modelBuilder.Entity<SamplePrescriptionDetail>(entity =>
            {
                entity.ToTable("sample_prescription_detail");

                entity.HasIndex(e => new { e.SamplePrescriptionId, e.ProductId }, "UQ__sample_p__07C1DA1AF728A389")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.Dose).HasColumnName("dose");

                entity.Property(e => e.DoseBasedOnBodyWeight)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("dose_based_on_body_weight");

                entity.Property(e => e.FrequencyPerDay).HasColumnName("frequency_per_day");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.SamplePrescriptionId).HasColumnName("sample_prescription_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.Property(e => e.Use)
                    .HasMaxLength(200)
                    .HasColumnName("use");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionDetailCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__creat__236943A5");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.SamplePrescriptionDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__produ__208CD6FA");

                entity.HasOne(d => d.SamplePrescription)
                    .WithMany(p => p.SamplePrescriptionDetails)
                    .HasForeignKey(d => d.SamplePrescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__sampl__1F98B2C1");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionDetailUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__sample_pr__updat__245D67DE");
            });

            modelBuilder.Entity<Shelf>(entity =>
            {
                entity.ToTable("shelves");

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
                    .WithMany(p => p.ShelfCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__shelves__created__60A75C0F");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ShelfUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__shelves__updated__619B8048");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("staffs");

                entity.HasIndex(e => e.UserAccountId, "UQ__staffs__1918BBDBD44DF760")
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

            modelBuilder.Entity<StockStrengthUnit>(entity =>
            {
                entity.ToTable("stock_strength_units");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");

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
                    .WithMany(p => p.SupplierCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__suppliers__creat__339FAB6E");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SupplierUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__suppliers__updat__3493CFA7");
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

                entity.HasIndex(e => e.UserAccountId, "UQ__user_log__1918BBDBD4278BBA")
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

                entity.HasIndex(e => e.UserAccountId, "UQ__user_log__1918BBDB781DFE75")
                    .IsUnique();

                entity.HasIndex(e => e.LoginName, "UQ__user_log__F6D56B57C654FD21")
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
