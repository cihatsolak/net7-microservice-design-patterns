using Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(AppDbContext)));
});

builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.AddConsumer<PaymentCompletedEventConsumer>();
    busRegistrationConfigurator.AddConsumer<PaymentFailedEventConsumer>();
    busRegistrationConfigurator.AddConsumer<StockNotReservedEventConsumer>();

    busRegistrationConfigurator.UsingRabbitMq((busRegistrationContext, rabbitMqBusFactoryConfigurator) =>
    {
        rabbitMqBusFactoryConfigurator.Host(builder.Configuration["RabbitMqSetting:HostAddress"], "/", hostConfigurator =>
        {
            hostConfigurator.Username(builder.Configuration["RabbitMqSetting:Username"]);
            hostConfigurator.Password(builder.Configuration["RabbitMqSetting:Password"]);
        });

        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(RabbitQueueName.OrderPaymentCompletedEventQueueName, endpoint =>
        {
            endpoint.ConfigureConsumer<PaymentCompletedEventConsumer>(busRegistrationContext);
        });

        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(RabbitQueueName.OrderPaymentFailedEventQueueName, endpoint =>
        {
            endpoint.ConfigureConsumer<PaymentFailedEventConsumer>(busRegistrationContext);
        });

        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(RabbitQueueName.OrderStockNotReservedEventQueueName, endpoint =>
        {
            endpoint.ConfigureConsumer<StockNotReservedEventConsumer>(busRegistrationContext);
        });
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

