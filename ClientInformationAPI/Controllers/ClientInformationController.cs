using ClientInformation.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using ClientInformation.Domain.Requests;
using AutoMapper;
using ClientInformation.Domain.Dtos;
using ClientInformation.Domain.Models;
using ClientInformation.Domain.Responses;
using System.Text;

namespace ClientInformationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientInfoController : ControllerBase
    {
        private readonly IClientInformationRepository _clientInfromationRepository;
        private readonly IMapper _mapper;

        public ClientInfoController(IClientInformationRepository clientInformationRepository, IMapper mapper)
        {
            _clientInfromationRepository = clientInformationRepository;
            _mapper = mapper;
        }

        // GET api/clientinfo
        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                var clients = await _clientInfromationRepository.GetAllClients();

                var clientDtos = _mapper.Map<List<ClientInfo>, List<ClientInformationDto>>(clients.ToList());

                return Ok(new ClientCollectionResponse
                {
                    Clients = clientDtos
                });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        // GET api/clientinfo/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            try
            {
                var client = await _clientInfromationRepository.GetClientById(id);

                if (client == null)
                {
                    return NotFound();
                }

                var clientDto = _mapper.Map<ClientInfo, ClientInformationDto>(client);

                var response = new SingleClientResponse() 
                { 
                    Client = clientDto
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        // POST api/clientinfo
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] NewPersonRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest();
                }

                var dataModel = _mapper.Map<ClientInformationDto, ClientInfo>(request.ClientInformation);

                var creadtedId = await _clientInfromationRepository.AddClient(dataModel);
                return CreatedAtAction(nameof(GetClient), new { id = creadtedId }, request.ClientInformation);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        // PUT api/clientinfo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(PersonEditRequest request)
        {
            try
            {
                if (request.ClientInformationEdit == null || request.ClientInformationEdit.Id != request.ClientId)
                {
                    return BadRequest();
                }

                var existingClient = await _clientInfromationRepository.GetClientById(request.ClientId);
                var existingClientDto = _mapper.Map<ClientInfo, ClientInfoDto>(existingClient);

                if (existingClient == null)
                {
                    return NotFound();
                }

                var updatedClientDto = existingClientDto with
                {
                    FirstName = request.ClientInformationEdit.FirstName,
                    LastName = request.ClientInformationEdit.LastName,
                    Gender = request.ClientInformationEdit.Gender,
                    IdNumber = request.ClientInformationEdit.IdNumber,
                    DateOfBirth = request.ClientInformationEdit.DateOfBirth,
                    Addresses = request.ClientInformationEdit.Addresses,
                    ContactInformation = request.ClientInformationEdit.ContactInformation
                };

                var updatedClient = _mapper.Map<ClientInfoDto, ClientInfo>(updatedClientDto);

                await _clientInfromationRepository.UpdateClient(updatedClient);
                return NoContent();
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        // DELETE api/clientinfo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            try
            {
                var client = await _clientInfromationRepository.GetClientById(id);
                if (client == null)
                {
                    return NotFound();
                }

                await _clientInfromationRepository.DeleteClient(id);
                return NoContent();
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportClientInformation()
        {
            var clientInformation = await _clientInfromationRepository.GetAllClients();
            var exportClientList = clientInformation.Select(ci => new ClientExport
            {
                Id = ci.Id,
                FirstName = ci.FirstName,
                LastName = ci.LastName,
                DateOfBirth = ci.DateOfBirth,
                Gender = ci.Gender,
                IdNumber = ci.IdNumber,
                Addresses = string.Join(";", ci.Addresses.Select(a => $"{a.Street}, {a.City}, {a.PostalCode}, {a.Province}")),
            });

            var csvData = ToCsv(exportClientList);
            var bytes = Encoding.UTF8.GetBytes(csvData);

            return File(bytes, "text/csv", "ClientInformation.csv");
        }

        private string ToCsv(IEnumerable<ClientExport> data)
        {
            var properties = typeof(ClientExport).GetProperties();
            var csvBuilder = new StringBuilder();

            // Header
            csvBuilder.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // Rows
            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item, null)?.ToString() ?? string.Empty);
                csvBuilder.AppendLine(string.Join(",", values));
            }

            return csvBuilder.ToString();
        }
    }
}
