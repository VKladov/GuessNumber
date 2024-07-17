using GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public class PlayersQueueGrain : Grain, IPlayersQueueGrain
    {
        private Queue<IPlayerGrain> players = new();
        private const int RoomCapacity = 2;
        private int _roomId;

        public async Task Enqueue(IPlayerGrain player)
        {
            players.Enqueue(player);
            await TryCreateRoom();
        }

        public Task Dequeue(IPlayerGrain player)
        {
            players = new Queue<IPlayerGrain>(players.Where(p => p != player));
            return Task.CompletedTask;
        }

        private async Task TryCreateRoom()
        {
            if (players.Count == RoomCapacity)
            {
                var room = GrainFactory.GetGrain<IRoomGrain>(_roomId);
                _roomId++;

                await room.SetCapacity(RoomCapacity);

                while (players.Count > 0)
                {
                    var player = players.Dequeue();
                    await room.AddPlayer(player);
                    await player.EnterRoom(room);
                }
            }
        }
    }
}
