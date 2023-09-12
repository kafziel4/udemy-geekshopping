using GeekShopping.OrderAPI.MessageConsumer;
using GeekShopping.OrderAPI.Model.Context;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repository;
using GeekShopping.OrderAPI.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<MySqlContext>(options =>
    options.UseMySql(connection, new MySqlServerVersion(ServerVersion.AutoDetect(connection))));

var dbOptionsBuilder = new DbContextOptionsBuilder<MySqlContext>();
dbOptionsBuilder.UseMySql(connection, new MySqlServerVersion(ServerVersion.AutoDetect(connection)));

builder.Services.AddSingleton<IOrderRepository>(new OrderRepository(dbOptionsBuilder.Options));

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQSettings"));
builder.Services.AddHostedService<RabbitMQCheckoutConsumer>();
builder.Services.AddHostedService<RabbitMQPaymentConsumer>();
builder.Services.AddSingleton<IRabbitMQMessageSender, RabbitMQMessageSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
