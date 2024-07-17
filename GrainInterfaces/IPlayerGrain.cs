using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task<int> GetScore();
        Task EnterRoom(IRoomGrain room);
        Task ExitRoom();
        Task<IRoomGrain?> GetCurrentRoom();
        Task AddScorePoint();
    }
}
