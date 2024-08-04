using ClientInformation.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Domain.Requests
{
    public class NewPersonRequest
    {
        public ClientInformationDto ClientInformation { get; set; }
    }
}
