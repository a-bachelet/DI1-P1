using System.Data.Common;

using FluentAssertions;

using Moq;

using Server.Actions;
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Tests.Unit.Actions;

public class CreateGameTest
{
    private readonly Mock<ICompaniesRepository> _companiesRepositoryMock;
    private readonly Mock<IGamesRepository> _gamesRepositoryMock;
    private readonly Mock<IPlayersRepository> _playersRepositoryMock;

    public CreateGameTest()
    {
        _companiesRepositoryMock = new Mock<ICompaniesRepository>();
        _gamesRepositoryMock = new Mock<IGamesRepository>();
        _playersRepositoryMock = new Mock<IPlayersRepository>();

        _companiesRepositoryMock
          .Setup(x => x.IsCompanyNameAvailable(It.IsAny<string>(), It.IsAny<int>()))
          .Returns(Task.Run(() => true));

        _companiesRepositoryMock
          .Setup(x => x.SaveCompany(It.IsAny<Company>()))
          .Returns((Company company) => Task.Run(() =>
          {
              typeof(Company).GetProperty("Id")?.SetValue(company, 1);
          }));

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

        _gamesRepositoryMock
          .Setup(x => x.GetById(It.IsAny<int>()))
          .Returns(Task.Run(() => new Game("Game 1")));

        _gamesRepositoryMock
          .Setup(x => x.GetByPlayerId(It.IsAny<int>()))
          .Returns(Task.Run(() => {
            var game = new Game("Game 1");
            typeof(Game).GetProperty("Id")?.SetValue(game, 1);
            return game;
          }));

        _playersRepositoryMock
          .Setup(x => x.PlayerExists(It.IsAny<int>()))
          .Returns(Task.Run(() => true));

        _playersRepositoryMock
          .Setup(x => x.IsPlayerNameAvailable(It.IsAny<string>(), It.IsAny<int>()))
          .Returns(Task.Run(() => true));

        _playersRepositoryMock
          .Setup(x => x.SavePlayer(It.IsAny<Player>()))
          .Returns((Player player) => Task.Run(() =>
          {
              typeof(Player).GetProperty("Id")?.SetValue(player, 1);
          }));

        _playersRepositoryMock
          .Setup(x => x.GetById(It.IsAny<int>()))
          .Returns(Task.Run(() => new Player("Game 1", 1)));
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithoutAGameName()
    {
        var actionParams = new CreateGameParams("", "Player 1", "Company 1");
        var action = new CreateGame(
          _companiesRepositoryMock.Object,
          _gamesRepositoryMock.Object,
          _playersRepositoryMock.Object
        );

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Game Name' must not be empty.");
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithoutAPlayerName()
    {
        var actionParams = new CreateGameParams("Game 1", "", "Company 1");
        var action = new CreateGame(
          _companiesRepositoryMock.Object,
          _gamesRepositoryMock.Object,
          _playersRepositoryMock.Object
        );

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Player Name' must not be empty.");
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithTooFewRounds()
    {
        var actionParams = new CreateGameParams("Game 1", "Player 1", "Company 1", 14);
        var action = new CreateGame(
          _companiesRepositoryMock.Object,
          _gamesRepositoryMock.Object,
          _playersRepositoryMock.Object
        );

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Rounds' must be greater than or equal to '15'.");
    }

    [Fact]
    public async Task ItShouldNotCreateGameWithATakenGameName()
    {
        _gamesRepositoryMock.Setup(x => x.IsGameNameAvailable(It.IsAny<string>())).Returns(Task.Run(() => false));

        var actionParams = new CreateGameParams("Game 1", "Player 1", "Company 1");
        var action = new CreateGame(
          _companiesRepositoryMock.Object,
          _gamesRepositoryMock.Object,
          _playersRepositoryMock.Object
        );

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

        var actionParams = new CreateGameParams("Game 1", "Player 1", "Company 1");
        var action = new CreateGame(
          _companiesRepositoryMock.Object,
          _gamesRepositoryMock.Object,
          _playersRepositoryMock.Object
        );

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
    }

    [Fact]
    public async Task ItShouldCreateGameWithValidData()
    {
        var actionParams = new CreateGameParams("Game 1", "Player 1", "Company 1");
        var action = new CreateGame(
          _companiesRepositoryMock.Object,
          _gamesRepositoryMock.Object,
          _playersRepositoryMock.Object
        );

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsSuccess.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(0);
    }
}
