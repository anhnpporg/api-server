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
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerPointTransaction> CustomerPointTransactions { get; set; }
        public virtual DbSet<DataSettingSystemCustomerPoint> DataSettingSystemCustomerPoints { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<EmailValidationStatus> EmailValidationStatuses { get; set; }
        public virtual DbSet<GoodsIssueNote> GoodsIssueNotes { get; set; }
        public virtual DbSet<GoodsIssueNoteType> GoodsIssueNoteTypes { get; set; }
        public virtual DbSet<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
        public virtual DbSet<GoodsReceiptNoteLog> GoodsReceiptNoteLogs { get; set; }
        public virtual DbSet<GoodsReceiptNoteType> GoodsReceiptNoteTypes { get; set; }
        public virtual DbSet<HashingAlgorithm> HashingAlgorithms { get; set; }
        public virtual DbSet<InventorySystemReport> InventorySystemReports { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductActiveSubstance> ProductActiveSubstances { get; set; }
        public virtual DbSet<ProductUnitPrice> ProductUnitPrices { get; set; }
        public virtual DbSet<RouteOfAdministration> RouteOfAdministrations { get; set; }
        public virtual DbSet<SamplePrescription> SamplePrescriptions { get; set; }
        public virtual DbSet<SamplePrescriptionDetail> SamplePrescriptionDetails { get; set; }
        public virtual DbSet<Shelf> Shelves { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
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
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

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
                    .HasConstraintName("FK__active_su__creat__68487DD7");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ActiveSubstanceUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__active_su__updat__693CA210");
            });

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.ToTable("batches");

                entity.HasIndex(e => e.Barcode, "UQ__batches__C16E36F8D784A73B")
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
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

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
                    .WithMany(p => p.BatchCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__batches__created__7B5B524B");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__batches__product__787EE5A0");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.BatchUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__batches__updated__7C4F7684");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brands");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

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
                    .HasConstraintName("FK__brands__created___4F7CD00D");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.BrandUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__brands__updated___5070F446");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.HasIndex(e => e.PhoneNumber, "UQ__customer__A1936A6B6227D918")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.FullName)
                    .HasMaxLength(150)
                    .HasColumnName("full_name");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.TotalPoint).HasColumnName("total_point");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CustomerCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__customers__creat__09A971A2");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.CustomerUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__customers__updat__0A9D95DB");
            });

            modelBuilder.Entity<CustomerPointTransaction>(entity =>
            {
                entity.ToTable("customer_point_transactions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");

                entity.Property(e => e.IsReciept)
                    .IsRequired()
                    .HasColumnName("is_reciept")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Point).HasColumnName("point");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerPointTransactions)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__customer___custo__2A164134");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.CustomerPointTransactions)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__customer___invoi__2B0A656D");
            });

            modelBuilder.Entity<DataSettingSystemCustomerPoint>(entity =>
            {
                entity.ToTable("data_setting_system_customer_point");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ToMoney).HasColumnName("to_money");

                entity.Property(e => e.ToPoint).HasColumnName("to_point");
            });

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.ToTable("diseases");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.DiseaseCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__diseases__create__2FCF1A8A");

                entity.HasOne(d => d.DeletedByNavigation)
                    .WithMany(p => p.DiseaseDeletedByNavigations)
                    .HasForeignKey(d => d.DeletedBy)
                    .HasConstraintName("FK__diseases__delete__31B762FC");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.DiseaseUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__diseases__update__30C33EC3");
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

            modelBuilder.Entity<GoodsIssueNote>(entity =>
            {
                entity.ToTable("goods_issue_notes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BatchId).HasColumnName("batch_id");

                entity.Property(e => e.ConvertedQuantity).HasColumnName("converted_quantity");

                entity.Property(e => e.GoodsIssueNoteTypeId).HasColumnName("goods_issue_note_type_id");

                entity.Property(e => e.OrderDetailId).HasColumnName("order_detail_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("unit");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("money")
                    .HasColumnName("unit_price");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.GoodsIssueNotes)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_iss__batch__2739D489");

                entity.HasOne(d => d.GoodsIssueNoteType)
                    .WithMany(p => p.GoodsIssueNotes)
                    .HasForeignKey(d => d.GoodsIssueNoteTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_iss__goods__25518C17");

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.GoodsIssueNotes)
                    .HasForeignKey(d => d.OrderDetailId)
                    .HasConstraintName("FK__goods_iss__order__2645B050");
            });

            modelBuilder.Entity<GoodsIssueNoteType>(entity =>
            {
                entity.ToTable("goods_issue_note_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<GoodsReceiptNote>(entity =>
            {
                entity.ToTable("goods_receipt_notes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BaseUnitPrice)
                    .HasColumnType("money")
                    .HasColumnName("base_unit_price");

                entity.Property(e => e.BatchId).HasColumnName("batch_id");

                entity.Property(e => e.ConvertedQuantity).HasColumnName("converted_quantity");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.GoodsReceiptNoteTypeId).HasColumnName("goods_receipt_note_type_id");

                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("money")
                    .HasColumnName("total_price");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("unit");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__batch__14270015");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__creat__17F790F9");

                entity.HasOne(d => d.GoodsReceiptNoteType)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.GoodsReceiptNoteTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__goods__1332DBDC");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK__goods_rec__invoi__151B244E");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.GoodsReceiptNotes)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__goods_rec__suppl__160F4887");
            });

            modelBuilder.Entity<GoodsReceiptNoteLog>(entity =>
            {
                entity.ToTable("goods_receipt_note_logs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GoodsReceiptNoteId).HasColumnName("goods_receipt_note_id");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("note");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.GoodsReceiptNote)
                    .WithMany(p => p.GoodsReceiptNoteLogs)
                    .HasForeignKey(d => d.GoodsReceiptNoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__goods_rec__goods__1AD3FDA4");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.GoodsReceiptNoteLogs)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__goods_rec__updat__1BC821DD");
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
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<InventorySystemReport>(entity =>
            {
                entity.ToTable("inventory_system_reports");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BatchId).HasColumnName("batch_id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.IsRead).HasColumnName("is_read");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("title");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.InventorySystemReports)
                    .HasForeignKey(d => d.BatchId)
                    .HasConstraintName("FK__inventory__batch__43D61337");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.InventorySystemReports)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__inventory__produ__44CA3770");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoices");

                entity.HasIndex(e => e.Barcode, "UQ__invoices__C16E36F85176E871")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Barcode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("barcode");

                entity.Property(e => e.BodyWeight)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("body_weight");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.DayUse).HasColumnName("day_use");

                entity.Property(e => e.Discount)
                    .HasColumnType("money")
                    .HasColumnName("discount");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("money")
                    .HasColumnName("total_price");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__invoices__create__10566F31");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__invoices__custom__0E6E26BF");
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.ToTable("managers");

                entity.HasIndex(e => e.UserAccountId, "UQ__managers__1918BBDB083EC6CE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.Manager)
                    .HasForeignKey<Manager>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__managers__user_a__3B75D760");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_detail");

                entity.HasIndex(e => new { e.InvoiceId, e.ProductId }, "UQ__order_de__B1FDDA975EEB1E0A")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DayUse).HasColumnName("day_use");

                entity.Property(e => e.Dose).HasColumnName("dose");

                entity.Property(e => e.Frequency).HasColumnName("frequency");

                entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("money")
                    .HasColumnName("total_price");

                entity.Property(e => e.UnitDose).HasColumnName("unit_dose");

                entity.Property(e => e.Use)
                    .HasMaxLength(200)
                    .HasColumnName("use");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__order_det__invoi__1F98B2C1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__order_det__produ__208CD6FA");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.HasIndex(e => e.Barcode, "UQ__products__C16E36F8452F4342")
                    .IsUnique();

                entity.HasIndex(e => e.DrugRegistrationNumber, "UQ__products__EF7E0909293322A7")
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
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

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

                entity.Property(e => e.IsManagedInBatches).HasColumnName("is_managed_in_batches");

                entity.Property(e => e.IsUseDose).HasColumnName("is_use_dose");

                entity.Property(e => e.MininumInventory).HasColumnName("mininum_inventory");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.RouteOfAdministrationId).HasColumnName("route_of_administration_id");

                entity.Property(e => e.ShelfId).HasColumnName("shelf_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__brand___5CD6CB2B");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProductCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__create__628FA481");

                entity.HasOne(d => d.RouteOfAdministration)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.RouteOfAdministrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__products__route___5EBF139D");

                entity.HasOne(d => d.Shelf)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ShelfId)
                    .HasConstraintName("FK__products__shelf___5DCAEF64");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ProductUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__products__update__6383C8BA");
            });

            modelBuilder.Entity<ProductActiveSubstance>(entity =>
            {
                entity.ToTable("product_active_substance");

                entity.HasIndex(e => new { e.ProductId, e.ActiveSubstanceId }, "UQ__product___12D3DF522A402413")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveSubstanceId).HasColumnName("active_substance_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.ActiveSubstance)
                    .WithMany(p => p.ProductActiveSubstances)
                    .HasForeignKey(d => d.ActiveSubstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__activ__6E01572D");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductActiveSubstances)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_a__produ__6D0D32F4");
            });

            modelBuilder.Entity<ProductUnitPrice>(entity =>
            {
                entity.ToTable("product_unit_prices");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConversionValue).HasColumnName("conversion_value");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsBaseUnit).HasColumnName("is_base_unit");

                entity.Property(e => e.IsDoseBasedOnBodyWeightUnit).HasColumnName("is_dose_based_on_body_weight_unit");

                entity.Property(e => e.IsPackingSpecification).HasColumnName("is_packing_specification");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("unit");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProductUnitPriceCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_u__creat__73BA3083");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductUnitPrices)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__product_u__produ__70DDC3D8");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ProductUnitPriceUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__product_u__updat__74AE54BC");
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
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");

                entity.Property(e => e.DiseaseId).HasColumnName("disease_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__creat__367C1819");

                entity.HasOne(d => d.DeletedByNavigation)
                    .WithMany(p => p.SamplePrescriptionDeletedByNavigations)
                    .HasForeignKey(d => d.DeletedBy)
                    .HasConstraintName("FK__sample_pr__delet__3864608B");

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.SamplePrescriptions)
                    .HasForeignKey(d => d.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__disea__3493CFA7");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__sample_pr__updat__37703C52");
            });

            modelBuilder.Entity<SamplePrescriptionDetail>(entity =>
            {
                entity.ToTable("sample_prescription_detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");

                entity.Property(e => e.Dose).HasColumnName("dose");

                entity.Property(e => e.Frequency).HasColumnName("frequency");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ProductUnitPriceId).HasColumnName("product_unit_price_id");

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
                    .HasConstraintName("FK__sample_pr__creat__3F115E1A");

                entity.HasOne(d => d.DeletedByNavigation)
                    .WithMany(p => p.SamplePrescriptionDetailDeletedByNavigations)
                    .HasForeignKey(d => d.DeletedBy)
                    .HasConstraintName("FK__sample_pr__delet__40F9A68C");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.SamplePrescriptionDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__produ__3C34F16F");

                entity.HasOne(d => d.ProductUnitPrice)
                    .WithMany(p => p.SamplePrescriptionDetails)
                    .HasForeignKey(d => d.ProductUnitPriceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__produ__3D2915A8");

                entity.HasOne(d => d.SamplePrescription)
                    .WithMany(p => p.SamplePrescriptionDetails)
                    .HasForeignKey(d => d.SamplePrescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__sample_pr__sampl__3B40CD36");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SamplePrescriptionDetailUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__sample_pr__updat__40058253");
            });

            modelBuilder.Entity<Shelf>(entity =>
            {
                entity.ToTable("shelves");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

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
                    .HasConstraintName("FK__shelves__created__5535A963");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ShelfUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__shelves__updated__5629CD9C");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("staffs");

                entity.HasIndex(e => e.UserAccountId, "UQ__staffs__1918BBDB984FD859")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.IsMale).HasColumnName("is_male");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.UrlAvartar)
                    .IsRequired()
                    .HasMaxLength(2100)
                    .HasColumnName("url_avartar");

                entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.Staff)
                    .HasForeignKey<Staff>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__staffs__user_acc__3F466844");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("name");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("phone_number");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.SupplierCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__suppliers__creat__02FC7413");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SupplierUpdatedByNavigations)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__suppliers__updat__03F0984C");
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("user_accounts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'SE Asia Standard Time'))");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<UserLoginDatum>(entity =>
            {
                entity.ToTable("user_login_data");

                entity.HasIndex(e => e.UserAccountId, "UQ__user_log__1918BBDB926AE834")
                    .IsUnique();

                entity.HasIndex(e => e.LoginName, "UQ__user_log__F6D56B578920F8C4")
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
                    .HasConstraintName("FK__user_logi__email__4AB81AF0");

                entity.HasOne(d => d.HashingAlgorithm)
                    .WithMany(p => p.UserLoginData)
                    .HasForeignKey(d => d.HashingAlgorithmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_logi__hashi__48CFD27E");

                entity.HasOne(d => d.UserAccount)
                    .WithOne(p => p.UserLoginDatum)
                    .HasForeignKey<UserLoginDatum>(d => d.UserAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__user_logi__user___47DBAE45");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
