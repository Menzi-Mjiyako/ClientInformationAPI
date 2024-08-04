using NUnit.Framework;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClientInformation.Data.Repositories.Interfaces;
using ClientInformationAPI.Controllers;
using AutoMapper;
using ClientInformation.Domain.Requests;
using ClientInformation.Domain.Dtos;
using ClientInformation.Domain.Models;
using ClientInformation.Domain.Responses;

namespace ClientInformationAPI.Tests
{
    [TestFixture]
    public class ClientInfoControllerTests
    {
        private IClientInformationRepository _clientInformationRepository;
        private IMapper _mapper;
        private ClientInfoController _controller;

        [Test]
        public async Task GetClients_ShouldReturnOkWithClients()
        {
            // Arrange
            var client = new ClientInfo { Id = Guid.NewGuid() };
            var clients = new List<ClientInfo> { client };
            var clientDto = new ClientInformationDto { Id = Guid.NewGuid() };
            var clientDtos = new List<ClientInformationDto> { clientDto };
            var response = new ClientCollectionResponse { Clients = clientDtos };

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.GetAllClients().Returns(await Task.FromResult(clients));
            _mapper.Map<List<ClientInfo>, List<ClientInformationDto>>(clients).Returns(await Task.FromResult(clientDtos));
            var _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.GetClients();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<ClientCollectionResponse>(okResult.Value);
        }

        [Test]
        public async Task GetClient_ShouldReturnOkWithClient()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new ClientInfo { Id = clientId };
            var clientDto = new ClientInformationDto { Id = clientId };
            var response = new ClientCollectionResponse
            {
                Clients = new List<ClientInformationDto> 
                {
                    clientDto
                }
            };

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.GetClientById(clientId).Returns(Task.FromResult(client));
            _mapper.Map<ClientInfo, ClientInformationDto>(client).Returns(await Task.FromResult(clientDto));

            var _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.GetClient(clientId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsInstanceOf<ClientCollectionResponse>(okResult.Value);
        }

        [Test]
        public async Task GetClient_ShouldReturnNotFoundWhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.GetClientById(clientId).Returns(await Task.FromResult<ClientInfo>(null));

            var _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.GetClient(clientId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task CreateClient_ShouldReturnCreatedAtActionWithClient()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var clientInformationDto = new ClientInformationDto { Id = clientId, FirstName = "John", LastName = "Doe" };
            var clientInfo = new ClientInfo { Id = clientId, FirstName = "John", LastName = "Doe" };
            var request = new NewPersonRequest { ClientInformation = clientInformationDto };

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.AddClient(clientInfo).Returns(await Task.FromResult<Guid>(clientInfo.Id));
            _mapper.Map<ClientInformationDto, ClientInfo>(clientInformationDto).Returns(clientInfo);

            var _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.CreateClient(request);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }

        [Test]
        public async Task CreateClient_ShouldReturnBadRequestWhenClientIsNull()
        {
            // Arrange
            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.CreateClient(null);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task UpdateClient_ShouldReturnNoContent()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var clientDto = new ClientInfoDto { Id = clientId, FirstName = "John", LastName = "Doe" };
            var client = new ClientInfo { Id = clientId, FirstName = "John", LastName = "Doe" };
            var request = new PersonEditRequest { ClientId = clientId, ClientInformationEdit = clientDto };

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.GetClientById(clientId).Returns(Task.FromResult(client));
            _mapper.Map<ClientInfo, ClientInfoDto>(client).Returns(clientDto);
            _mapper.Map<ClientInfoDto, ClientInfo>(clientDto).Returns(client);

            await _clientInformationRepository.UpdateClient(client);
            var _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.UpdateClient(request);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateClient_ShouldReturnBadRequestWhenClientIsNull()
        {
            // Arange
            var clientId = Guid.NewGuid();

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _controller = await GetController(_clientInformationRepository, _mapper);

            //Act
            var result = await _controller.UpdateClient(new PersonEditRequest { ClientId = clientId, ClientInformationEdit = null });

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task UpdateClient_ShouldReturnBadRequestWhenClientIdDoesNotMatch()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var clientDto = new ClientInfoDto { Id = clientId, FirstName = "John", LastName = "Doe" };
            var request = new PersonEditRequest { ClientId = Guid.NewGuid(), ClientInformationEdit = clientDto };

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.UpdateClient(request);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task UpdateClient_ShouldReturnNotFoundWhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var clientDto = new ClientInfoDto { Id = clientId, FirstName = "John", LastName = "Doe" };
            var request = new PersonEditRequest { ClientId = clientId, ClientInformationEdit = clientDto };

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.GetClientById(clientId).Returns(Task.FromResult<ClientInfo>(null));

            _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.UpdateClient(request);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteClient_ShouldReturnNoContent()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var client = new ClientInfo { Id = clientId };

            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.GetClientById(clientId).Returns(Task.FromResult(client));
            await _clientInformationRepository.DeleteClient(clientId);

            _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.DeleteClient(clientId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteClient_ShouldReturnNotFoundWhenClientDoesNotExist()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            _clientInformationRepository = Substitute.For<IClientInformationRepository>();
            _mapper = Substitute.For<IMapper>();

            _clientInformationRepository.GetClientById(clientId).Returns(await Task.FromResult<ClientInfo>(null));
            _controller = await GetController(_clientInformationRepository, _mapper);

            // Act
            var result = await _controller.DeleteClient(clientId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }


        private async Task<ClientInfoController> GetController(IClientInformationRepository _clientInformationRepository, IMapper _mapper)
        {
            return new ClientInfoController(_clientInformationRepository, _mapper);
        }
    }
}