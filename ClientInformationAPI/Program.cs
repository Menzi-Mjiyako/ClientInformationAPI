using ClientInformation.Data.DataAccess;
using ClientInformation.Data.DataAccess.Interfaces;
using ClientInformation.Data.Migrations;
using ClientInformation.Data.Repositories;
using ClientInformation.Data.Repositories.Interfaces;
using ClientInformationAPI.Config;
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add FluentMigrator services
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(runnerBuilder => runnerBuilder
        .AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(CreateBaseTables).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

// Add AutoMapper services
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

builder.Services.AddScoped<IDataProvider, DataProvider>();
builder.Services.AddScoped<IClientInformationRepository, ClientInformationRepository>();

var app = builder.Build();

// Migrate the database
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.UseCors("AllowAngularApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
