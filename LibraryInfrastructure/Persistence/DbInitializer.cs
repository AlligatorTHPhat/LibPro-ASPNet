using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.ValueObjects;
using System;
using System.Linq;

namespace LibraryInfrastructure.Persistence
{
    public static class DbInitializer
    {
        public static readonly Guid SystemStaffAccountId = Guid.Parse("B4C1A8B2-2D1E-4B7E-9F8D-123456789ABC");
        public static void Seed(LibraryDbContext context)
        {
            if (context.Accounts.Any()) return;
            var passwordHasher = new LibraryInfrastructure.Security.PasswordHasher();
            var hashedPw = passwordHasher.Hash("123456");

            // --- 1. SEED ACCOUNTS & PROFILE ---
            var staffAccount = new Account("SYS_LIB", hashedPw, "system@library.com", UserRole.Librarian, false);
            typeof(Account).GetProperty("Id").SetValue(staffAccount, SystemStaffAccountId);
            var directorAccount = new Account("admin", hashedPw, "director@library.com", UserRole.Director, false);
            context.Accounts.Add(staffAccount);
            context.Accounts.Add(directorAccount);

            var staffInfo = new Staff("STF001", "THỦ THƯ HỆ THỐNG", new Gender("Male",1), new DateOnly(2005, 11, 23),
                new Address("280 An Dương Vương", "Phường Chợ Quán", "Quận 5", "TP. Hồ Chí Minh"), "0123456781", staffAccount.Id, false);
            context.Staffs.Add(staffInfo);

            var directorInfo = new Staff("ADMIN", "Trần Hoàng Phát", new Gender("Male",1), new DateOnly(2005, 11, 23),
                                new Address("280 An Dương Vương", "Phường Chợ Quán", "Quận 5", "TP. Hồ Chí Minh"), "0123456789", directorAccount.Id, false);
            context.Staffs.Add(directorInfo);

            // --- 2. SEED PUBLISHER (Nhà xuất bản) ---
            var pubVanHoc = new Publisher("Nhà xuất bản Văn Học", "info@nxbvanhoc.com.vn");
            var pubTre = new Publisher("Nhà xuất bản Trẻ", "hopthubandoc@nxbtre.com.vn");
            context.Publishers.AddRange(pubVanHoc, pubTre);

            // --- 3. SEED SUPPLIER (Nhà cung cấp) ---
            var supNhaNam = new Supplier("Nhã Nam", "Liên hệ Nhã Nam", "bookstore@nhanam.vn");
            var supADong = new Supplier("Á Đông", "Liên hệ Á Đông", "contact@adong.com");
            context.Suppliers.AddRange(supNhaNam, supADong);

            // --- 4. AUDIT LOG ---
            context.AuditLogs.Add(new AuditLog(null, "System", "SeedData", "Database", "All", "None", "Full Seed Data (Accounts, Books, Categories) Created"));

            // --- 5. SAVE CHANGES ---
            context.SaveChanges();
        }
    }
}