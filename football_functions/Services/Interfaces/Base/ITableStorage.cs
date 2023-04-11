using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace football_functions.Services.Interfaces;

public interface ITableStorage<T>
{
    IEnumerable<T> GetAll();

    Task<TableResult> InsertOrMerge(T entity);
}

