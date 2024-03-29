var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("StockDb");
});

builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.AddConsumer<OrchestrationOrderCreatedEventConsumer>();
    busRegistrationConfigurator.AddConsumer<OrchestrationStockRollBackMessageConsumer>();

    busRegistrationConfigurator.UsingRabbitMq((busRegistrationContext, rabbitMqBusFactoryConfigurator) =>
    {
        rabbitMqBusFactoryConfigurator.Host(builder.Configuration["RabbitMqSetting:HostAddress"], "/", hostConfigurator =>
        {
            hostConfigurator.Username(builder.Configuration["RabbitMqSetting:Username"]);
            hostConfigurator.Password(builder.Configuration["RabbitMqSetting:Password"]);
        });

        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(RabbitQueueName.StockOrderCreatedEventQueueName, configure =>
        {
            configure.ConfigureConsumer<OrchestrationOrderCreatedEventConsumer>(busRegistrationContext);
        });

        rabbitMqBusFactoryConfigurator.ReceiveEndpoint(RabbitQueueName.StockRollBackMessageQueueName, configure =>
        {
            configure.ConfigureConsumer<OrchestrationStockRollBackMessageConsumer>(busRegistrationContext);
        });
    });
});

var app = builder.Build();

await AppDbContextSeed.ExecuteAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();