
namespace ClientInformation.Domain.Dtos
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
    }
}
