using System.Reflection;

using FluentAssertions;

using Moq;

using Server.Actions;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Tests.Unit.Actions;

public class CreateGameTest
{
    private Mock<IGamesRepository> _gamesRepositoryMock;
    private Mock<IPlayersRepository> _playersRepositoryMock;

    public CreateGameTest()
    {
        _gamesRepositoryMock = new Mock<IGamesRepository>();
        _playersRepositoryMock = new Mock<IPlayersRepository>();

        _gamesRepositoryMock
          .Setup(x => x.IsGameNameAvailable(It.IsAny<string>()))
          .Returns(Task.Run(() => true));

        _gamesRepositoryMock
          .Setup(x => x.GameExists(It.IsAny<int>()))
          .Returns(Task.Run(() => true));

        _gamesRepositoryMock
          .Setup(x => x.SaveGame(It.IsAny<Game>()))
          .Returns((Game game) => Task.Run(() =>
          {
              typeof(Game).GetProperty("Id")?.SetValue(game, 1);
          }));

        _playersRepositoryMock
          .Setup(x => x.IsPlayerNameAvailable(It.IsAny<string>(), It.IsAny<int>()))
          .Returns(Task.Run(() => true));
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithoutAGameName()
    {
        var actionParams = new CreateGameParams("", "Player 1");
        var action = new CreateGame(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Game Name' must not be empty.");
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithoutAPlayerName()
    {
        var actionParams = new CreateGameParams("Game 1", "");
        var action = new CreateGame(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Player Name' must not be empty.");
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithTooFewRounds()
    {
        var actionParams = new CreateGameParams("Game 1", "Player 1", 14);
        var action = new CreateGame(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Rounds' must be greater than or equal to '15'.");
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithATakenGameName()
    {
        _gamesRepositoryMock.Setup(x => x.IsGameNameAvailable(It.IsAny<string>())).Returns(Task.Run(() => false));

        var actionParams = new CreateGameParams("Game 1", "Player 1");
        var action = new CreateGame(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Game Name' is already in use.");
    }

    [Fact]
    public async Task ItShouldNotCreateGameIfCreatePlayerFails()
    {
        _playersRepositoryMock
          .Setup(x => x.IsPlayerNameAvailable(It.IsAny<string>(), It.IsAny<int>()))
          .Returns(Task.Run(() => false));

        var actionParams = new CreateGameParams("Game 1", "Player 1");
        var action = new CreateGame(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
    }

    [Fact]
    public async Task ItShouldCreateGameWithValidData()
    {
        var actionParams = new CreateGameParams("Game 1", "Player 1");
        var action = new CreateGame(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsSuccess.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(0);
    }
}
