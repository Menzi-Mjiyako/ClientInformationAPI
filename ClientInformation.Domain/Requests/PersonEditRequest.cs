using ClientInformation.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Domain.Requests
{
    public class PersonEditRequest
    {
        public Guid ClientId { get; set; }
        public ClientInfoDto ClientInformationEdit { get; set; }
    }
}
