using ClientInformation.Data.DataAccess.Interfaces;
using ClientInformation.Data.Repositories.Interfaces;
using ClientInformation.Domain.Models;
using System.Data;
using System.Transactions;

namespace ClientInformation.Data.Repositories
{
    public class ClientInformationRepository : IClientInformationRepository
    {
        private readonly IDataProvider _dataProvider;

        public ClientInformationRepository(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<IEnumerable<ClientInfo>> GetAllClients()
        {
            var sql = @"SELECT * FROM ClientInformation WITH (NOLOCK);";

            // Retrieve all clients
            var clients = (await _dataProvider.QueryAsync<ClientInfo>(sql)).ToList();

            // Retrieve all addresses and contact information
            var addresslist = await GetAllAddresses();
            var contactlist = await GetAllContactInformation();

            // Map addresses and contact information to clients
            foreach (var client in clients)
            {
                client.Addresses = addresslist.Where(a => a.ClientId == client.Id).ToList();
                client.ContactInformation = contactlist.Where(c => c.ClientId == client.Id).ToList();
            }

            return clients;
        }

        private async Task<IEnumerable<Address>> GetAllAddresses()
        {
            var sql = @"SELECT * FROM Address WITH (NOLOCK);";

            // Retrieve all addresses
            var addresses = (await _dataProvider.QueryAsync<Address>(sql)).ToList();

            return addresses;
        }

        private async Task<IEnumerable<ContactInformation>> GetAllContactInformation()
        {
            var sql = @"SELECT * FROM ContactInformation WITH (NOLOCK);";

            // Retrieve all contact information
            var contacts = (await _dataProvider.QueryAsync<ContactInformation>(sql)).ToList();

            return contacts;
        }

        public async Task<ClientInfo> GetClientById(Guid id)
        {
            var sql = @"SELECT * FROM ClientInformation WHERE Id = @Id;";
            var client = (await _dataProvider.QueryAsync<ClientInfo>(sql, new { Id = id })).FirstOrDefault();

            var addresslist = await GetAllAddresses();
            var contactlist = await GetAllContactInformation();

            if (client != null)
            {
                client.Addresses = addresslist.Where(a => a.ClientId == client.Id).ToList();
                client.ContactInformation = contactlist.Where(c => c.ClientId == client.Id).ToList();
            }

            return client;
        }

        public async Task<Guid> AddClient(ClientInfo client)
        {
            var sql = @"INSERT INTO ClientInformation (FirstName, LastName, Gender, DateOfBirth, IdNumber) 
                        OUTPUT INSERTED.Id
                        VALUES (@FirstName, @LastName, @Gender, @DateOfBirth, @IdNumber);";

            var id = (await _dataProvider.QueryAsync<Guid>(sql, new
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Gender = client.Gender,
                DateOfBirth = client.DateOfBirth,
                IdNumber = client.IdNumber
            })).Single();

            client.Id = id;

            foreach (var address in client.Addresses)
            {
                address.ClientId = id;
                var addressSql = @"INSERT INTO Address (ClientId, Street, City, Province, PostalCode) 
                               VALUES (@ClientId, @Street, @City, @Province, @PostalCode)";
                await _dataProvider.ExecuteAsync(addressSql, new
                {
                    ClientId = address.ClientId,
                    Street = address.Street,
                    City = address.City,
                    Province = address.Province,
                    PostalCode = address.PostalCode
                });
            }

            foreach (var contact in client.ContactInformation)
            {
                contact.ClientId = id;
                var contactSql = @"INSERT INTO ContactInformation (ClientId,TelePhoneNumber, CellPhoneNumber, EmailAddress, WorkPhoneNumber) 
                               VALUES (@ClientId,@TelePhoneNumber, @CellPhoneNumber, @EmailAddress, @WorkPhoneNumber)";
                await _dataProvider.ExecuteAsync(contactSql, new
                {
                    ClientId = contact.ClientId,
                    TelePhoneNumber = contact.TelePhoneNumber,
                    CellPhoneNumber = contact.CellPhoneNumber,
                    EmailAddress = contact.EmailAddress,
                    WorkPhoneNumber = contact.WorkPhoneNumber
                });
            }

            return id;
        }

        public async Task UpdateClient(ClientInfo client)
        {
            var sql = @"UPDATE ClientInformation 
                    SET FirstName = @FirstName, 
                        LastName = @LastName,
                        Gender = @Gender,
                        DateOfBirth = @DateOfBirth,
                        IdNumber =@IdNumber
                    WHERE Id = @Id";
            await _dataProvider.ExecuteAsync(sql, new
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Gender = client.Gender,
                DateOfBirth = client.DateOfBirth,
                IdNumber = client.IdNumber,
                Id = client.Id
            });

            var deleteAddressSql = @"DELETE FROM Address WHERE Id = @Id";
            await _dataProvider.ExecuteAsync(deleteAddressSql, new { Id = client.Id });

            foreach (var address in client.Addresses)
            {
                address.Id = client.Id;
                var insertAddressSql = @"INSERT INTO Address (ClientId, Street, City, Province, PostalCode) 
                                     VALUES (@ClientId, @Street, @City, @Province, @PostalCode)";
                await _dataProvider.ExecuteAsync(insertAddressSql, new
                {
                    Street = address.Street,
                    City = address.City,
                    Province = address.Province,
                    PostalCode = address.PostalCode,
                    ClientId = client.Id
                });
            }

            var deleteContactSql = @"DELETE FROM ContactInformation WHERE Id = @Id";
            await _dataProvider.ExecuteAsync(deleteContactSql, new { Id = client.Id });

            foreach (var contact in client.ContactInformation)
            {
                contact.Id = client.Id;
                var insertContactSql = @"INSERT INTO ContactInformation (ClientId, TelePhoneNumber, CellPhoneNumber, EmailAddress, WorkPhoneNumber) 
                                     VALUES (@ClientId, @TelePhoneNumber, @CellPhoneNumber, @EmailAddress, @WorkPhoneNumber)";
                await _dataProvider.ExecuteAsync(insertContactSql, new
                {
                    TelePhoneNumber = contact.TelePhoneNumber,
                    CellPhoneNumber = contact.CellPhoneNumber,
                    EmailAddress = contact.EmailAddress,
                    WorkPhoneNumber = contact.WorkPhoneNumber,
                    ClientId = client.Id
                });
            }
        }

        public async Task DeleteClient(Guid clientId)
        {
            var sqlDeleteAddresses = "DELETE FROM Address WHERE ClientId = @Id";
            var sqlDeleteContact = "DELETE FROM ContactInformation WHERE ClientId = @Id";
            var sqlDeleteClient = "DELETE FROM ClientInformation WHERE Id = @Id";

            await _dataProvider.ExecuteAsync(sqlDeleteAddresses, new { Id = clientId });
            await _dataProvider.ExecuteAsync(sqlDeleteContact, new { Id = clientId });
            await _dataProvider.ExecuteAsync(sqlDeleteClient, new { Id = clientId });
        }
    }
}
