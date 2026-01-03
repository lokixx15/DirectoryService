using DirectoryService.Presentation.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies();

var app = builder.Build();

app.Configure();  

app.Run();

