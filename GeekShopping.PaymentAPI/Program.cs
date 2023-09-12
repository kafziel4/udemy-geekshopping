using GeekShopping.PaymentAPI.MessageConsumer;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentAPI.Settings;
using GeekShopping.PaymentProcessor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQSettings"));
builder.Services.AddHostedService<RabbitMQPaymentConsumer>();
builder.Services.AddSingleton<IRabbitMQMessageSender, RabbitMQMessageSender>();
builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
