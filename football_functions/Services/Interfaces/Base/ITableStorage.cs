using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace football_functions.Services.Interfaces;

public interface ITableStorage<T>
{
    Task<IEnumerable<T>> GetAll();
    T GetById(string id);

    Task<TableResult> InsertOrMerge(T entity);

    Task<T> InsertOrReplace(T entity);
}

