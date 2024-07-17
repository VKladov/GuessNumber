using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IRoomGrain : IGrainWithIntegerKey
    {
        Task SetCapacity(int capacity);
        Task AddPlayer(IPlayerGrain player);
        Task RemovePlayer(IPlayerGrain player);
        Task TakeGuess(IPlayerGrain player, int guess);
        Task<bool> IsGameCompleted();
        Task<bool> IsWinner(IPlayerGrain player);
    }
}
