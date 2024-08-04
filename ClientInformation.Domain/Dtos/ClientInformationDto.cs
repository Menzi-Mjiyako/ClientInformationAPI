
namespace ClientInformation.Domain.Dtos
{
    public class ClientInformationDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdNumber { get; set; }
        public IList<AddressDto> Addresses { get; set; }
        public IList<ContactInformationDto> ContactInformation { get; set; }

        public ClientInformationDto()
        {
            Addresses = new List<AddressDto>();
            ContactInformation = new List<ContactInformationDto>();
        }
    }
}
