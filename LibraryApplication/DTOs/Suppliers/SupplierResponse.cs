namespace LibraryApplication.DTOs
{
	public class SupplierCreateDto
	{
		public required string Name { get; set; }
		public string? TaxCode { get; set; }
		public string? ContactPerson { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string? OfficeAddress { get; set; }
	}

	public class SupplierUpdateDto : SupplierCreateDto
	{
		public Guid Id { get; set; }
	}
}