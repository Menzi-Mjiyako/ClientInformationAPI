using ClientInformation.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInformation.Domain.Responses
{
    public class ClientCollectionResponse
    {
        public IEnumerable<ClientInformationDto> Clients { get; set; }

        public ClientCollectionResponse()
        {
                Clients = new List<ClientInformationDto>();
        }
    }
}
