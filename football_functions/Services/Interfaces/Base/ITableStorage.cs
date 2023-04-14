using System.Collections.Generic;
using System.Threading.Tasks;
using football_functions.Models;
using Microsoft.Azure.Cosmos.Table;

namespace football_functions.Services.Interfaces;

public interface ITableStorage<T>
{
    Task<IEnumerable<PlayerTableStorageEntity>> GetAll();
    PlayerTableStorageEntity GetById(string id);

    Task<TableResult> InsertOrMerge(T entity);
}

