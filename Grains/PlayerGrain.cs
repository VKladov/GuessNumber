using GrainInterfaces;

namespace Grains
{
    public sealed class PlayerGrain : Grain, IPlayerGrain
    {
        private IRoomGrain? currentRoom;
        private readonly IPersistentState<PlayerSave> save;

        public PlayerGrain(
            [PersistentState(
                stateName: "playerSave",
                storageName: "players")] IPersistentState<PlayerSave> save)
        {
            this.save = save;
        }

        async Task IPlayerGrain.AddScorePoint()
        {
            save.State.Score++;
            await save.WriteStateAsync();
        }

        Task IPlayerGrain.EnterRoom(IRoomGrain room)
        {
            currentRoom = room;
            return Task.CompletedTask;
        }

        Task IPlayerGrain.ExitRoom()
        {
            currentRoom = null;
            return Task.CompletedTask;
        }

        Task<IRoomGrain?> IPlayerGrain.GetCurrentRoom()
        {
            return Task.FromResult(currentRoom);
        }

        Task<int> IPlayerGrain.GetScore()
        {
            return Task.FromResult(save.State.Score);
        }
    }
}
