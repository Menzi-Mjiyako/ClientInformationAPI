

namespace ClientInformation.Domain.Dtos
{
    public class ContactInformationDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string? TelePhoneNumber { get; set; }
        public string? CellPhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? WorkPhoneNumber { get; set; }
    }
}
