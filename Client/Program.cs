using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GrainInterfaces;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering();
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .UseConsoleLifetime();

using IHost host = builder.Build();
await host.StartAsync();

IClusterClient client = host.Services.GetRequiredService<IClusterClient>();

var player = client.GetGrain<IPlayerGrain>(Guid.NewGuid());

while (true)
{
    var score = await player.GetScore();

    Console.WriteLine($"Your score: {score}. Id: {player.GetPrimaryKeyString()}");
    var queue = client.GetGrain<IPlayersQueueGrain>(0);
    await queue.Enqueue(player);

    Console.WriteLine("Waiting other players to join");

    IRoomGrain? room = null;
    while (room == null)
    {
        await Task.Delay(1000);
        room = await player.GetCurrentRoom();
    }

    Console.WriteLine("All players joined");
    Console.WriteLine("Enter number from 0 to 100:");

    var guess = 0;
    while (true)
    {
        var input = Console.ReadLine();
        if (int.TryParse(input, out guess) && guess >= 0 && guess <= 100)
        {
            break;
        }
        Console.WriteLine("Wrong input. Try again:");
    }

    await room.TakeGuess(player, guess);

    Console.WriteLine("Waiting all players to guess");

    while (!await room.IsGameCompleted())
    {
        await Task.Delay(1000);
    }

    Console.WriteLine("Game completed");

    var won = await room.IsWinner(player);

    Console.WriteLine(won ? "You are winner!" : "You are loser!");

    Console.WriteLine("Again ? (y / n)");

    var key = Console.ReadKey();
    if (key.KeyChar != 'y')
    {
        break;
    }
    Console.WriteLine();
}


await host.StopAsync();