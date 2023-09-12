using GeekShopping.Email.MessageConsumer;
using GeekShopping.Email.Model.Context;
using GeekShopping.Email.Repository;
using GeekShopping.Email.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<MySqlContext>(options =>
    options.UseMySql(connection, new MySqlServerVersion(ServerVersion.AutoDetect(connection))));

var dbOptionsBuilder = new DbContextOptionsBuilder<MySqlContext>();
dbOptionsBuilder.UseMySql(connection, new MySqlServerVersion(ServerVersion.AutoDetect(connection)));

builder.Services.AddSingleton<IEmailRepository>(new EmailRepository(dbOptionsBuilder.Options));

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQSettings"));
builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
