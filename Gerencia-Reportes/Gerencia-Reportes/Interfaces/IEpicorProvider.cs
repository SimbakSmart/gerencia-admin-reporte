

using Gerencia_Reportes.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gerencia_Reportes.Interfaces
{
    public interface IEpicorProvider
    {
        Task<List<CallsInQueues>> FetchAllAsync(string queryParams = "");
        Task<List<Queue>> FetchQueuesAsync();
        (bool, string) TestConnection();

    }
}
