using System.Collections.Generic;

namespace CBHWA.Models
{
    interface IStateRepository
    {
        IList<State> GetAll();
    }
}
