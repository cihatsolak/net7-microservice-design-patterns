var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ProductService>(opt =>
{
    opt.BaseAddress = new Uri("https://localhost:7202/api/products/");
}).AddPolicyHandler(GetCircuitBreakerPolicy());

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


#region Circuit Breaker Pattern
IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() //Basic Level
{
    //Ard arda 3 başarısız istek olduğunda 10 saniye bekle. tekrar istek yaptığında başarısız ise 10 saniye bekledikten sonra tekrar istek yap.
    //Ard arda 3 başarısız istek, 10 saniye bekle. 10 saniye bekeldikten sonra tekrar istek yapıp devreyi açık/yarı açık durumuna getirek.
    return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), 
        onBreak: (httpResponseMessage, timeSpan) =>
    {
        Debug.WriteLine("Circuit Breaker Status => On Break");
    }, onReset: () =>
    {
        Debug.WriteLine("Circuit Breaker Status => On Reset");
    }, onHalfOpen: () =>
    {
        Debug.WriteLine("Circuit Breaker Status => On Half Open");
    });
}

IAsyncPolicy<HttpResponseMessage> GetAdvanceCircuitBreakerPolicy()
{
    // 0.5 --> 30 saniye içerisinde başarısız istek sayısı %50'den (0.5) fazlaysa, eğer 0.1 verirsek %10'u temsil eder. 10 saniye içinde 100 istekten 10 tanesi başarısız ise. circuit devreye girer.
    // minimumThroughput: minimum başarısız sayısı (oran dışında). 


    // 30 saniye içerisinde minimum 30 istek kafadan meydana gelecek onun dışında birde 0.5 (%50) oranında başarısız hata olacak.
    // (örn: 30 saniye içerisinde 100 isteğin 30'unun hata alması yetmez. çünkü 0.5 ile %50 başarısız oran istedik. 51 isteğin başarısız olması lazım)
    // TimeSpan.FromSeconds(40): sıfırlanma durumu. Her devre açık durumuna geçtiğinde 40 saniye bekleyecek.
    return HttpPolicyExtensions.HandleTransientHttpError().AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(30), 30, TimeSpan.FromSeconds(40), 
        onBreak: (httpResponseMessage, timeSpan) =>
    {
        Debug.WriteLine("Circuit Breaker Status => On Break");
    }, onReset: () =>
    {
        Debug.WriteLine("Circuit Breaker Status => On Reset");
    }, onHalfOpen: () =>
    {
        Debug.WriteLine("Circuit Breaker Status => On Half Open");
    });
}
#endregion

#region Retry Pattern

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError().OrResult(httpResponseMessage => httpResponseMessage.StatusCode == HttpStatusCode.NotFound).WaitAndRetryAsync(5, retryAttempt =>
    {
        Debug.WriteLine($"Retry Count :{retryAttempt}");
        return TimeSpan.FromSeconds(10);
    }, onRetryAsync: OnRetryAsync);
}

Task OnRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
{
    Debug.WriteLine($"Request is made again:{arg2.TotalMilliseconds}");

    return Task.CompletedTask;
}

#endregion