using System;
using Microsoft.EntityFrameworkCore;
using LibraryDomain.Entities;
using LibraryDomain.ValueObjects;
using LibraryDomain.Enums;

namespace LibraryInfrastructure.Persistence
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        // --- DbSet Area ---
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookItem> BookItems { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanDetail> LoanDetails { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<FinePayment> FinePayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(a => a.Username).IsUnique(); 

                entity.Property(a => a.Email).IsRequired().HasMaxLength(255);

                entity.Property(a => a.Role)
                      .HasConversion(v => v.ToString(), v => (UserRole)Enum.Parse(typeof(UserRole), v));

                entity.HasOne(a => a.Reader)
                      .WithOne(r => r.Account)
                      .HasForeignKey<Reader>(r => r.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Staff)
                      .WithOne(s => s.Account)
                      .HasForeignKey<Staff>(s => s.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                      .IsRequired(false);

                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.Action).HasMaxLength(50);
                entity.Property(e => e.EntityName).HasMaxLength(100);
                entity.Property(e => e.EntityId).HasMaxLength(100);

                entity.Property(e => e.OldValues).HasColumnType("nvarchar(max)");
                entity.Property(e => e.NewValues).HasColumnType("nvarchar(max)");

                entity.Property(e => e.Timestamp).IsRequired();
                entity.HasIndex(e => e.Timestamp);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title).IsRequired().HasMaxLength(500);

                entity.Property(b => b.Isbn)
                    .HasConversion(v => v.Value, v => new Isbn(v))
                    .HasColumnName("ISBN")
                    .HasMaxLength(13)
                    .IsRequired();
                entity.HasIndex(b => b.Isbn).IsUnique();

                entity.HasOne(b => b.Publisher)
                      .WithMany(p => p.Books)
                      .HasForeignKey(b => b.PublisherId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(b => b.Supplier)
                      .WithMany(s => s.SuppliedBooks)
                      .HasForeignKey(b => b.SupplierId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(b => b.Category)
                      .WithMany(c => c.Books)
                      .HasForeignKey(b => b.CategoryId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(b => b.BookItems)
                      .WithOne(bi => bi.Book)
                      .HasForeignKey(bi => bi.BookId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(b => b.DigitalFilePath).HasMaxLength(1000);
                entity.Property(b => b.CoverImageUrl).HasMaxLength(1000);

                entity.Property(b => b.Status)
                      .HasConversion(v => v.ToString(), v => (BookStatus)Enum.Parse(typeof(BookStatus), v));
            });

            modelBuilder.Entity<BookItem>(entity =>
            {
                entity.HasKey(bi => bi.Id);

                entity.Property(bi => bi.Barcode)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.HasIndex(bi => bi.Barcode).IsUnique();

                entity.Property(bi => bi.ShelfLocation)
                      .HasMaxLength(255);

                entity.Property(bi => bi.Status)
                      .HasConversion(v => v.ToString(), v => (BookStatus)Enum.Parse(typeof(BookStatus), v))
                      .HasMaxLength(50);

                entity.HasOne(bi => bi.Book)
                      .WithMany(b => b.BookItems) 
                      .HasForeignKey(bi => bi.BookId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(255);
                entity.HasIndex(c => c.Name).IsUnique();

                entity.Property(c => c.Description)
                      .HasMaxLength(1000);

                entity.HasMany(c => c.Books)
                      .WithOne(b => b.Category)
                      .HasForeignKey(b => b.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                var navigation = entity.Metadata.FindNavigation(nameof(Category.Books));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.LoanDate).IsRequired();

                entity.HasOne(l => l.Reader)
                      .WithMany( r => r.Loans) 
                      .HasForeignKey(l => l.ReaderId)
                      .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(l => l.IssuedByStaff)
                      .WithMany()
                      .HasForeignKey(l => l.IssuedByStaffId)
                      .OnDelete(DeleteBehavior.Restrict); 

                entity.HasMany(l => l.Details)
                      .WithOne(d => d.Loan)
                      .HasForeignKey(d => d.LoanId)
                      .OnDelete(DeleteBehavior.Cascade);

                var navigation = entity.Metadata.FindNavigation(nameof(Loan.Details));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<LoanDetail>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.OwnsOne(d => d.FineAmount, a =>
                {
                    a.Property(m => m.Amount)
                        .HasColumnName("FineAmount")
                        .HasPrecision(18, 2);
                    a.Property(m => m.Currency)
                        .HasColumnName("FineCurrency")
                        .HasMaxLength(10)
                        .HasDefaultValue("VND");
                });

                entity.Property(d => d.Status)
                      .HasConversion(v => v.ToString(), v => (LoanStatus)Enum.Parse(typeof(LoanStatus), v))
                      .HasMaxLength(50);

                entity.Property(d => d.DueDate).IsRequired();
                entity.Property(d => d.ReturnDate).IsRequired(false);

                entity.Property(d => d.AccessToken)
                      .HasMaxLength(2000)
                      .IsRequired(false);

                entity.HasOne(d => d.Loan)
                      .WithMany(l => l.Details)
                      .HasForeignKey(d => d.LoanId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.BookItem)
                      .WithMany()
                      .HasForeignKey(d => d.BookItemId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Reader>(entity =>
            {
                entity.HasKey(r => r.Id);

                // 1. LibraryCardNumber là duy nhất và bắt buộc
                entity.Property(r => r.LibraryCardNumber)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.HasIndex(r => r.LibraryCardNumber).IsUnique();

                entity.Property(r => r.FullName).IsRequired().HasMaxLength(200);

                entity.Property(r => r.Gender)
                    .HasConversion(v => v.Value, v => Gender.FromValue(v)) // Chuyển đổi Object <-> int
                    .HasColumnName("Gender")
                    .IsRequired();

                // 2. Cấu hình Value Object Address (Gộp chung vào bảng Reader)
                entity.OwnsOne(r => r.Address, a =>
                {
                    a.Property(p => p.Street).HasColumnName("Street").HasMaxLength(200);
                    a.Property(p => p.District).HasColumnName("District").HasMaxLength(100);
                    a.Property(p => p.City).HasColumnName("City").HasMaxLength(100);
                    a.Property(p => p.Ward).HasColumnName("Address_Ward").HasMaxLength(100);
                });

                // 3. Quan hệ 1:1 với Account
                entity.HasOne(r => r.Account)
                      .WithOne(a => a.Reader)
                      .HasForeignKey<Reader>(r => r.AccountId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa Account thì Reader bay màu theo

                // 4. Cấu hình Enum ReaderType
                entity.Property(r => r.Type)
                      .HasConversion(v => v.ToString(), v => (ReaderType)Enum.Parse(typeof(ReaderType), v))
                      .HasMaxLength(50);

                // 5. Mapping các danh sách ẩn (Backing Fields)
                var loanNavigation = entity.Metadata.FindNavigation(nameof(Reader.Loans));
                loanNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

                var resNavigation = entity.Metadata.FindNavigation(nameof(Reader.Reservations));
                resNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(res => res.Id);

                // 1. Cấu hình các cột ngày tháng
                entity.Property(res => res.ReservedDate).IsRequired();
                entity.Property(res => res.ReadyDate).IsRequired(false);
                entity.Property(res => res.ExpiryDate).IsRequired(false);

                // 2. Cấu hình Enum Status
                entity.Property(res => res.Status)
                      .HasConversion(v => v.ToString(), v => (ReservationStatus)Enum.Parse(typeof(ReservationStatus), v))
                      .HasMaxLength(50);

                // 3. Quan hệ với Reader
                entity.HasOne(res => res.Reader)
                      .WithMany(r => r.Reservations)
                      .HasForeignKey(res => res.ReaderId)
                      .OnDelete(DeleteBehavior.Restrict); // Không cho xóa độc giả nếu đang có phiếu đặt chỗ

                // 4. Quan hệ với BookItem
                entity.HasOne(res => res.BookItem)
                      .WithMany() // Một cuốn sách vật lý có thể được đặt nhiều lần trong vòng đời của nó
                      .HasForeignKey(res => res.BookItemId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FinePayment>(entity =>
            {
                entity.HasKey(fp => fp.Id);

                // 1. Cấu hình Value Object Money (Sử dụng OwnsOne)
                entity.OwnsOne(fp => fp.Amount, a =>
                {
                    a.Property(m => m.Amount)
                     .HasColumnName("PaidAmount") // Tên cột trong DB
                     .HasPrecision(18, 2);        // Độ chính xác tiền tệ

                    a.Property(m => m.Currency)
                     .HasColumnName("PaidCurrency")
                     .HasMaxLength(10)
                     .HasDefaultValue("VND");
                });

                entity.Property(fp => fp.PaymentMethod)
                      .HasMaxLength(100)
                      .IsRequired(false); // Sẽ null cho đến khi MarkAsPaid

                entity.Property(fp => fp.PaymentDate).IsRequired();
                entity.Property(fp => fp.IsPaid).IsRequired();

                // 2. Quan hệ với LoanDetail (1-1 hoặc N-1 tùy logic)
                // Ở đây ta giả định mỗi LoanDetail (cuốn sách lỗi) có một bản ghi phạt
                entity.HasOne(fp => fp.LoanDetail)
                      .WithMany() // Hoặc WithOne tùy bạn khai báo ở LoanDetail
                      .HasForeignKey(fp => fp.LoanDetailId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(s => s.Id);

                // 1. Mã nhân viên là duy nhất
                entity.Property(s => s.StaffCode)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.HasIndex(s => s.StaffCode).IsUnique();

                entity.Property(s => s.FullName).IsRequired().HasMaxLength(200);

                entity.Property(s => s.Gender)
                        .HasConversion(v => v.Value, v => Gender.FromValue(v))
                        .HasColumnName("Gender")
                        .IsRequired();

                // 2. Cấu hình Value Object Address (Gộp chung vào bảng Staff giống Reader)
                entity.OwnsOne(s => s.Address, a =>
                {
                    a.Property(p => p.Street).HasColumnName("Street").HasMaxLength(200);
                    a.Property(p => p.District).HasColumnName("District").HasMaxLength(100);
                    a.Property(p => p.City).HasColumnName("City").HasMaxLength(100);
                    a.Property(p => p.Ward).HasColumnName("Address_Ward").HasMaxLength(100);
                });

                // 3. Quan hệ 1:1 với Account
                entity.HasOne(s => s.Account)
                      .WithOne(a => a.Staff)
                      .HasForeignKey<Staff>(s => s.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                // 1. Khóa chính
                entity.HasKey(p => p.Id);

                // 2. Tên NXB là bắt buộc, giới hạn độ dài để tối ưu
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(255);

                // 3. Thông tin liên hệ (Email, Phone, Address)
                entity.Property(p => p.Email)
                      .HasMaxLength(150);

                entity.Property(p => p.PhoneNumber)
                      .HasMaxLength(20);

                entity.Property(p => p.OfficeAddress)
                      .HasMaxLength(500);

                // 4. Cấu hình quan hệ 1-N (Crucial!)
                // Ở đây ta nói với EF Core: "Sử dụng field _books để quản lý ICollection Books"
                entity.HasMany(p => p.Books)
                      .WithOne(b => b.Publisher)
                      .HasForeignKey(b => b.PublisherId)
                      .OnDelete(DeleteBehavior.SetNull);

                // 5. Chỉ định cho EF biết cách truy cập vào List ẩn (Field Mapping)
                var navigation = entity.Metadata.FindNavigation(nameof(Publisher.Books));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(255);
                entity.Property(s => s.TaxCode).HasMaxLength(50);

                // Cấu hình quan hệ 1-N với cái tên "SuppliedBooks" cho khớp với Entity
                entity.HasMany(s => s.SuppliedBooks) // Tên chính xác trong Supplier.cs
                      .WithOne(b => b.Supplier)      // Mỗi Book có một Supplier
                      .HasForeignKey(b => b.SupplierId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Cho phép EF Core đổ dữ liệu vào cái field ẩn _suppliedBooks
                var navigation = entity.Metadata.FindNavigation(nameof(Supplier.SuppliedBooks));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });
        }
    }
}