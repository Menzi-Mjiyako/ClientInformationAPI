using ClientInformation.Domain.Models;

namespace ClientInformation.Data.Repositories.Interfaces
{
    public interface IClientInformationRepository
    {
        Task<IEnumerable<ClientInfo>> GetAllClients();
        Task<ClientInfo> GetClientById(Guid id);
        Task<Guid> AddClient(ClientInfo client);
        Task UpdateClient(ClientInfo client);
        Task DeleteClient(Guid id);
    }
}
