using FluentResults;

using Microsoft.EntityFrameworkCore;

using Server.Actions;
using Server.Actions.Contracts;
using Server.Endpoints.Extensions;
using Server.Models;
using Server.Persistence;
using Server.Persistence.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WssDbContext>();
builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddTransient<ICompaniesRepository, CompaniesRepository>();
builder.Services.AddTransient<IConsultantsRepository, ConsultantsRepository>();
builder.Services.AddTransient<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddTransient<IGamesRepository, GamesRepository>();
builder.Services.AddTransient<IPlayersRepository, PlayersRepository>();
builder.Services.AddTransient<ISkillsRepository, SkillsRepository>();

builder.Services.AddTransient<IAction<CreateCompanyParams, Result<Company>>, CreateCompany>();
builder.Services.AddTransient<IAction<CreateEmployeeParams, Result<Employee>>, CreateEmployee>();
builder.Services.AddTransient<IAction<CreateGameParams, Result<Game>>, CreateGame>();
builder.Services.AddTransient<IAction<CreatePlayerParams, Result<Player>>, CreatePlayer>();
builder.Services.AddTransient<IAction<JoinGameParams, Result<Player>>, JoinGame>();
builder.Services.AddTransient<IAction<StartGameParams, Result<Game>>, StartGame>();

var app = builder.Build();

var dbContext = new WssDbContext(new DbContextOptionsBuilder().Options, app.Configuration);
var round = new Round(1);

round.Actions.Add(new SendEmployeeForTrainingRoundAction());
round.Actions.Add(new PassMyTurnRoundAction());

dbContext.Add(round);

dbContext.SaveChanges();

var newRound = dbContext.Rounds.FirstOrDefault();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.UseHttpsRedirection();

app.Run();
