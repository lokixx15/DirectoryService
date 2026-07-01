using AuthService.Presentation.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

app.Configure();

app.Run();
