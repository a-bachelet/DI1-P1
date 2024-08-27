// using System;

// using Client.OldRecord;
// using Client.Records;

// using Microsoft.AspNetCore.SignalR.Client;
// using Microsoft.Extensions.DependencyInjection;

// using Spectre.Console;

// namespace Client.OldScreens;

// public class CurrentGameScreen(int gameId, string playerName)
// {
//     public readonly int GameId = gameId;

//     public readonly string PlayerName = playerName;

//     public GameOverview? CurrentGame { get; private set; } = null!;

//     public async Task<bool> Show()
//     {
//         AnsiConsole.Clear();

//         await ConnectToGameHub();
//         await DisplayFetchStatus();





//         return true;
//     }

//     private async Task ConnectToGameHub()
//     {
//         var hubConnection = new HubConnectionBuilder()
//             .WithUrl(new Uri($"wss://localhost:7032/games/{GameId}"), opts =>
//             {
//                 opts.HttpMessageHandlerFactory = (message) =>
//                 {
//                     if (message is HttpClientHandler clientHandler)
//                     {
//                         clientHandler.ServerCertificateCustomValidationCallback +=
//                             (sender, certificate, chain, sslPolicyErrors) => { return true; };
//                     }

//                     return message;
//                 };
//             })
//             .AddJsonProtocol()
//             .Build();

//         hubConnection.On<GameOverview>("CurrentGameUpdated", data => CurrentGame = data);

//         await hubConnection.StartAsync();
//     }

//     private async Task DisplayFetchStatus()
//     {
//         await AnsiConsole.Status().StartAsync("Fetching current game...", async ctx =>
//         {
//             await Task.Delay(1000);

//             while (CurrentGame is null)
//             {
//                 await Task.Delay(0);
//             }
//         });
//     }

//     private async Task DisplayWaitingScreen()
//     {
//         AnsiConsole.Clear();

//         var waitingTable = BuildWaitingTable();

//         AnsiConsole.Write(waitingTable);
//     }

//     private Table BuildWaitingTable()
//     {
//         var table = new Table();

//         table.AddColumn("");

//         table.AddRow($"Game : {CurrentGame!.Name}");

//         var playersText = $"Players ({CurrentGame!.PlayersCount}/{CurrentGame!.MaximumPlayersCount})\n";

//         foreach (var player in CurrentGame!.Players)
//         {
//             playersText += $":black_small_square: {player.Name} | Company";
//             if (player.Name == PlayerName) { playersText += " :star:"; }
//             if (player.Name != CurrentGame!.Players.Last().Name) { playersText += "\n"; }
//         }

//         var playersMarkup = new Markup(playersText);

//         table.AddRow(playersMarkup);

//         table.Border(TableBorder.Rounded);

//         table.HideHeaders();
//         table.ShowRowSeparators();
//         table.Expand();

//         return table;
//     }
// }
