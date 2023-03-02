using Choreography.PaymentService.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.AddConsumer<StockReservedEventConsumer>();

    busRegistrationConfigurator.UsingRabbitMq((busRegistrationContext, rabbitMqBusFactoryConfigurator) =>
    {
        rabbitMqBusFactoryConfigurator.Host(builder.Configuration["RabbitMqSetting:HostAddress"], "/", hostConfigurator =>
        {
            hostConfigurator.Username(builder.Configuration["RabbitMqSetting:Username"]);
            hostConfigurator.Password(builder.Configuration["RabbitMqSetting:Password"]);
        });

        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(RabbitQueueName.StockReservedEventQueueName, endpoint =>
        {
            endpoint.ConfigureConsumer<StockReservedEventConsumer>(busRegistrationContext);
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

await app.RunAsync();
