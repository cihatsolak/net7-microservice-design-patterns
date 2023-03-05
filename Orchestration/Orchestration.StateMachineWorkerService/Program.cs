IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.AddMassTransit(busRegistrationConfigurator =>
        {
            busRegistrationConfigurator.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
            .EntityFrameworkRepository(configure =>
            {
                configure.AddDbContext<SagaDbContext, OrderStateDbContext>((serviceProvider, dbContextOptionsBuilder) =>
                {
                    dbContextOptionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString(nameof(OrderStateDbContext)), sqlServerOptions =>
                    {
                        sqlServerOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name); //dokümantasyona göre gerekli
                    });
                });
            });

            busRegistrationConfigurator.UsingRabbitMq((busRegistrationContext, rabbitMqBusFactoryConfigurator) =>
            {
                rabbitMqBusFactoryConfigurator.Host(builder.Configuration["RabbitMqSetting:HostAddress"], "/", hostConfigurator =>
                {
                    hostConfigurator.Username(builder.Configuration["RabbitMqSetting:Username"]);
                    hostConfigurator.Password(builder.Configuration["RabbitMqSetting:Password"]);
                });

                rabbitMqBusFactoryConfigurator.ReceiveEndpoint(RabbitQueueName.OrderSaga, endpoint =>
                {
                    endpoint.ConfigureSaga<OrderStateInstance>(busRegistrationContext);
                });
            });
        });

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
