using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

using WorldCitiesAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog support
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.MSSqlServer(connectionString:
    ctx.Configuration.GetConnectionString("DefaultConnection"),
    restrictedToMinimumLevel: LogEventLevel.Information,
    sinkOptions: new MSSqlServerSinkOptions
    {
        TableName = "LogEvents",
        AutoCreateSqlTable = true
    }
    )
    .WriteTo.Console()
    );

// Add services to the container.
builder.Services.AddCors();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();