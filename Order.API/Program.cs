var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(AppDbContext)));
});

builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.UsingRabbitMq((busRegistrationContext, rabbitMqBusFactoryConfigurator) =>
    {
        rabbitMqBusFactoryConfigurator.Host(builder.Configuration["RabbitMqSetting:HostAddress"], "/", hostConfigurator =>
        {
            hostConfigurator.Username(builder.Configuration["RabbitMqSetting:Username"]);
            hostConfigurator.Password(builder.Configuration["RabbitMqSetting:Password"]);
        });
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

