using Resiliency.ServiceX.Resiliencies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ProductService>(opt =>
{
    opt.BaseAddress = new Uri("https://localhost:7202/api/products/");
}).AddPolicyHandler(new BasicCircuitBreakerPattern().GetCircuitBreakerPolicy());

#region Retry Pattern

//builder.Services.AddHttpClient<ProductService>(opt =>
//{
//    opt.BaseAddress = new Uri("https://localhost:7202/api/products/");
//}).AddPolicyHandler(GetRetryPolicy());

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();