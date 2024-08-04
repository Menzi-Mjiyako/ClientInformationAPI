using ClientInformation.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Domain.Responses
{
    public class SingleClientResponse
    {
        public ClientInformationDto Client { get; set; }
    }
}
