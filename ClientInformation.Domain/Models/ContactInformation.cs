using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Domain.Models
{
    public class ContactInformation
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string? TelePhoneNumber { get; set; }
        public string? CellPhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? WorkPhoneNumber { get; set; }
    }
}
