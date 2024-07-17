using GrainInterfaces;

namespace Grains
{
    internal class RoomGrain : Grain, IRoomGrain
    {
        private List<IPlayerGrain> players = new();
        private Dictionary<IPlayerGrain, int> playerGuesses = new();
        private int capacity;
        private bool isGameCompleted;
        private IPlayerGrain? winner;
        private Random random = new();

        public Task SetCapacity(int capacity)
        {
            this.capacity = capacity;
            return Task.CompletedTask;
        }

        public Task AddPlayer(IPlayerGrain player)
        {
            if (capacity == 0)
            {
                throw new InvalidOperationException("Room capacity is not set");
            }

            players.RemoveAll(x => x.GetPrimaryKey() == player.GetPrimaryKey());
            players.Add(player);
            return Task.CompletedTask;
        }

        public Task RemovePlayer(IPlayerGrain player)
        {
            players.RemoveAll(x => x.GetPrimaryKey() == player.GetPrimaryKey());
            return Task.CompletedTask;
        }

        public Task<bool> IsGameCompleted()
        {
            return Task.FromResult(isGameCompleted);
        }

        public Task<bool> IsWinner(IPlayerGrain player)
        {
            var playerId = player.GetPrimaryKey();
            return Task.FromResult(playerId == winner.GetPrimaryKey());
        }

        public async Task TakeGuess(IPlayerGrain player, int guess)
        {
            if (playerGuesses.TryGetValue(player, out _))
            {
                throw new InvalidOperationException($"Player with id {player.GetPrimaryKey()} already made guess");
            }
            playerGuesses.Add(player, guess);
            if (playerGuesses.Count == players.Count)
            {
                await DefineWinner();
            }
        }

        private async Task DefineWinner()
        {
            var targetNumber = random.Next(0, 100);
            var minDelta = 100;
            foreach (var (player, guess) in playerGuesses) 
            { 
                var delta = Math.Abs(guess - targetNumber);
                if (delta < minDelta)
                {
                    minDelta = delta;
                    winner = player;
                }
            }

            if (winner != null)
            {
                await winner.AddScorePoint();
                await Cleanup();
            }

            isGameCompleted = true;
        }

        private async Task Cleanup()
        {
            foreach (var player in players)
            {
                await player.ExitRoom();
            }

            players.Clear();
        }
    }
}
