using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPlayersQueueGrain : IGrainWithIntegerKey
    {
        Task Enqueue(IPlayerGrain player);
        Task Dequeue(IPlayerGrain player);
    }
}
