using ClientInformation.Data.DataAccess.Interfaces;
using ClientInformation.Data.Repositories;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Data;

namespace ClientInformation.Data.Tests
{
    [TestFixture]
    public class ClientInformationRepositoryTests
    {

        [Test]
        public async Task AddClient_ShouldAddClient()
        {
            // Arrange
            var dataProvider = Substitute.For<IDataProvider>();
            var repository = new ClientInformationRepository(dataProvider);

            var client = new Domain.Models.ClientInfo
            {
                FirstName = "John",
                LastName = "Doe",
                Addresses = new List<Domain.Models.Address>
            {
                new Domain.Models.Address { Street = "123 Main St" }
            },
                ContactInformation = new List<Domain.Models.ContactInformation>
            {
                new Domain.Models.ContactInformation { EmailAddress = "john.doe@example.com" }
            }
            };

            var newClientId = Guid.NewGuid();
            dataProvider.QueryAsync<Guid>(Arg.Any<string>(), Arg.Any<object>()).Returns(await Task.FromResult(new List<Guid> { newClientId }));

            // Act
            await repository.AddClient(client);

            // Assert
            client.Id.Should().Be(newClientId);
            await dataProvider.Received().ExecuteAsync(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public async Task UpdateClient_ShouldUpdateClient()
        {
            // Arrange
            var dataProvider = Substitute.For<IDataProvider>();
            var repository = new ClientInformationRepository(dataProvider);

            var client = new Domain.Models.ClientInfo
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Addresses = new List<Domain.Models.Address>
            {
                new Domain.Models.Address { Street = "123 Main St" }
            },
                ContactInformation = new List<Domain.Models.ContactInformation>
            {
                new Domain.Models.ContactInformation { EmailAddress = "john.doe@example.com" }
            }
            };

            // Act
            await repository.UpdateClient(client);

            // Assert
            await dataProvider.Received().ExecuteAsync(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public async Task DeleteClient_ShouldDeleteClient()
        {
            // Arrange
            var dataProvider = Substitute.For<IDataProvider>();
            var repository = new ClientInformationRepository(dataProvider);

            var clientId = Guid.NewGuid();

            // Act
            await repository.DeleteClient(clientId);

            // Assert
            await dataProvider.Received().ExecuteAsync(Arg.Any<string>(), Arg.Any<object>());
        }
    }
}