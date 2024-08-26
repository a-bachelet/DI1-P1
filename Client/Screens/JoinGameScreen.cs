using System;
using System.Collections.Generic;

using Client.Prompts;
using Client.Records;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

namespace Client.Screens;

public static class JoinGameScreen
{
    static readonly ICollection<JoinableGame> games = [];

    public static async Task<bool> Show()
    {
        AnsiConsole.Clear();

        var hubConnection = BuildHubConnection();
        await hubConnection.StartAsync();
        var choosenGame = await JoinGamePrompt.Show(games);
        AnsiConsole.WriteLine(choosenGame.ToString());
        return true;
    }

    static HubConnection BuildHubConnection()
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(new Uri("wss://localhost:7032/main"), opts =>
            {
                opts.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                    {
                        clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    }

                    return message;
                };
            })
            .AddJsonProtocol()
            .Build();

        hubConnection.On<ICollection<JoinableGame>>("JoinableGamesListUpdated", data =>
        {
            if (data.Count == 0)
            {
                games.Add(null!);
            }
            else
            {
                data.ToList().ForEach(d => games.Add(d));
            }
        });

        return hubConnection;
    }
}
