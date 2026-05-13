using AutoMapper;
using LibraryApplication.DTOs;
using LibraryApplication.DTOs.Accounts;
using LibraryApplication.DTOs.Audits;
using LibraryApplication.DTOs.Categories;
using LibraryApplication.DTOs.Loans;
using LibraryApplication.DTOs.Readers;
using LibraryApplication.DTOs.Reservation;
using LibraryApplication.DTOs.Reservevations;
using LibraryApplication.DTOs.Staffs;
using LibraryDomain.Entities;
using LibraryDomain.Enums;
using LibraryDomain.ValueObjects;

namespace LibraryApplication.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuditLog, AuditResponse>();

            CreateMap<Category, CategoryResponse>();

            CreateMap<BookItem, BookItemResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) 
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.ShelfLocation));

            CreateMap<Book, BookResponse>()
                .ForMember(dest => dest.Isbn, opt => opt.MapFrom(src =>
                    src.Isbn.Value))
                .ForMember(dest => dest.TotalCopies, opt => opt.MapFrom(src =>
                    src.BookItems.Count(bi => !bi.IsDeleted)))
                .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src =>
                    src.BookItems.Count(bi => !bi.IsDeleted && bi.Status == BookStatus.Available)))
                .ForMember(dest => dest.BookItems, opt => opt.MapFrom(src =>
                    src.BookItems.Where(bi => !bi.IsDeleted).ToList()))
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src =>
                    src.CoverImageUrl));

            CreateMap<CreateBookRequest, Book>()
                .ForMember(dest => dest.Isbn, opt => opt.MapFrom(src => new LibraryDomain.ValueObjects.Isbn(src.Isbn)))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateBookRequest, Book>()
                .ForMember(dest => dest.CoverImageUrl, opt => opt.Ignore());
            //CreateMap<RegisterRequest, Account>();

            /*CreateMap<RegisterReaderRequest, Account>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());*/

            CreateMap<Account, AccountResponse>();
            CreateMap<RegisterReaderRequest, Reader>()
                .ConstructUsing(src => new Reader(
                    "", src.FullName, Gender.FromValue(src.Gender), src.DateOfBirth,
                    new Address(src.Street, src.Ward, src.District, src.City),
                    src.PhoneNumber, Guid.Empty, src.Type, false
                ));
            CreateMap<Reader, ReaderResponse>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src =>
                    src.IsMembershipActive() && src.IsDeleted))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src =>
                    src.Gender.Value))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                    $"{src.Address.Street}, {src.Address.Ward}, {src.Address.District}, {src.Address.City}"))
                .ForMember(dest => dest.ReaderTypeName, opt => opt.MapFrom(src =>
                    src.Type.ToString())); 
            CreateMap<CreateReaderRequest, Reader>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => Gender.FromValue(src.Gender)))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address(src.Street, "", "", src.City)));
            CreateMap<UpdateReaderRequest, Reader>();

            CreateMap<RegisterStaffRequest, Staff>()
                .ConstructUsing(src => new Staff(
                        "", src.FullName, Gender.FromValue(src.Gender), src.DateOfBirth,
                        new Address(src.Street, src.Ward, src.District, src.City),
                        src.PhoneNumber, Guid.Empty, false
                ));
            CreateMap<Staff, StaffResponse>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src =>
                    src.IsDeleted))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.Gender.Name))
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src =>
                        $"{src.Address.Street}, {src.Address.Ward}, {src.Address.District}, {src.Address.City}"));

            CreateMap<CreateStaffRequest, Staff>();
            CreateMap<UpdateStaffRequest, Staff>();

            CreateMap<Loan, LoanResponse>()
                .ForMember(dest => dest.ReaderName, opt => opt.MapFrom(src =>
                    src.Reader.FullName))
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src =>
                    src.IssuedByStaff.FullName))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src =>
                    src.Details))
                .ForMember(dest => dest.LoanTicketNumber, opt => opt.MapFrom(src =>
                    $"L-{src.LoanDate:yyyyMMdd}-{src.Id.ToString().Substring(0, 4).ToUpper()}"));

            CreateMap<LoanDetail, LoanDetailResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src =>
                    src.BookItem != null ? src.BookItem.BookId : Guid.Empty))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src =>
                    src.BookItem != null && src.BookItem.Book != null ? src.BookItem.Book.Title : "Sách số/Không xác định"))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src =>
                    src.BookItem != null ? src.BookItem.Barcode : null))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.ReaderName, opt => opt.MapFrom(src =>
                    src.Loan != null && src.Loan.Reader != null ? src.Loan.Reader.FullName : "N/A"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.FineAmount, opt => opt.MapFrom(src =>
                    src.FineAmount != null ? src.FineAmount.Amount : 0))
                .ForMember(dest => dest.IsDigital, opt => opt.MapFrom(src => src.AccessToken != null))
                .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.AccessToken));

            CreateMap<Publisher, PublisherCreateDto>();
            CreateMap<Publisher, PublisherUpdateDto>();
            CreateMap<Supplier, SupplierCreateDto>();
            CreateMap<Supplier, SupplierUpdateDto>();

            CreateMap<Reservation, ReservationResponse>()
                .ForMember(dest => dest.ReaderName, opt => opt.MapFrom(src => src.Reader.FullName))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.BookItem.Book.Title))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.BookItem.Barcode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ReservedDate, opt => opt.MapFrom(src => src.ReservedDate))
                .ForMember(dest => dest.ReadyDate, opt => opt.MapFrom(src => src.ReadyDate))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate));

            CreateMap<Reservation, ReserveBookRequest>();
        }
    }
}