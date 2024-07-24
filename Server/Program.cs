using Server.Persistence;
using Server.Persistence.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WssDbContext>();

builder.Services.AddTransient<IGamesRepository, GamesRepository>();
builder.Services.AddTransient<IPlayersRepository, PlayersRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
