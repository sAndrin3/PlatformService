using System.Net;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5135);
    
    if (builder.Environment.IsDevelopment())
    {
        serverOptions.Listen(IPAddress.Any, 7127, listenOptions =>
        {
            listenOptions.UseHttps(); 
        });
    }
});

// var env = builder.Environment;  
// var configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        Console.WriteLine("--> Using  SqlServer dB");
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"));
    }
    else
    {
        Console.WriteLine("--> Using InMem dB");
        options.UseInMemoryDatabase("InMem");
    }
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();  
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context => 
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});


Console.WriteLine($"---> Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"---> CommandService Endpoint {builder.Configuration["CommandService"]}");

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());



app.Run();


