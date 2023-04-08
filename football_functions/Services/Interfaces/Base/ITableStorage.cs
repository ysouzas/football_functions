using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using football_functions.Models;

namespace football_functions.Services.Interfaces;

public interface ITableStorage<T>
{
    IEnumerable<PlayerTableStorageEntity> GetAll();
}

