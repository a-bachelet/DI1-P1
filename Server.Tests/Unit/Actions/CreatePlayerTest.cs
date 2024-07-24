using FluentAssertions;

using Moq;

using Server.Actions;
using Server.Persistence.Contracts;

namespace Server.Tests.Unit.Actions;

public class CreatePlayerTest
{
    private Mock<IGamesRepository> _gamesRepositoryMock;
    private Mock<IPlayersRepository> _playersRepositoryMock;

    public CreatePlayerTest()
    {
        _gamesRepositoryMock = new Mock<IGamesRepository>();
        _playersRepositoryMock = new Mock<IPlayersRepository>();

        _gamesRepositoryMock.Setup(x => x.GameExists(It.IsAny<int>())).Returns(Task.Run(() => true));
        _playersRepositoryMock.Setup(x => x.IsPlayerNameAvailable(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.Run(() => true));
    }

    [Fact]
    public async Task ItShouldNotCreatePlayerWithoutAPlayerName()
    {
        var actionParams = new CreatePlayerParams("", 1);
        var action = new CreatePlayer(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Player Name' must not be empty.");
    }

    [Fact]
    public async Task ItShouldNotCreatePlayerWhenGameDoesNotExist()
    {
        _gamesRepositoryMock.Setup(x => x.GameExists(It.IsAny<int>())).Returns(Task.Run(() => false));

        var actionParams = new CreatePlayerParams("Player 1", 1);
        var action = new CreatePlayer(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("Game with Id \"1\" not found.");
    }

    [Fact]
    public async Task ItShouldNotCreatePlayerWithATakenPlayerName()
    {
        _playersRepositoryMock.Setup(x => x.IsPlayerNameAvailable(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.Run(() => false));

        var actionParams = new CreatePlayerParams("Player 1", 1);
        var action = new CreatePlayer(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsFailed.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(1);
        actionResult.Errors.First().Message.Should().Be("'Player Name' is already in use.");
    }

    [Fact]
    public async Task ItShouldCreatePlayerWithValidData()
    {
        var actionParams = new CreatePlayerParams("Player 1", 1);
        var action = new CreatePlayer(_gamesRepositoryMock.Object, _playersRepositoryMock.Object);

        var actionResult = await action.PerformAsync(actionParams);

        actionResult.IsSuccess.Should().BeTrue();
        actionResult.Errors.Count.Should().Be(0);
    }
}
