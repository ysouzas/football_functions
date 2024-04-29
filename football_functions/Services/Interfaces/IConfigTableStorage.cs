using System.Threading.Tasks;
using football_functions.Models;

namespace football_functions.Services.Interfaces;

public interface IConfigTableStorage : ITableStorage<ConfigTableStorageEntity>
{
    Task<ConfigTableStorageEntity> GetFirst();

}