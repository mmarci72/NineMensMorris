using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Persistence
{
    public interface IMalomDataAccess
    {
        Task<MalomTable> LoadAsync(String path);
        
        Task SaveAsync(String path, MalomTable table);


    }
}
