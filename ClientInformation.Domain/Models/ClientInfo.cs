using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Domain.Models
{
    public class ClientInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdNumber { get; set; }
        public IList<Address> Addresses { get; set; }
        public IList<ContactInformation> ContactInformation { get; set; }

        public ClientInfo()
        {
            Addresses = new List<Address>();
            ContactInformation = new List<ContactInformation>();
        }
    }
}
