namespace Resiliency.ServiceX.Resiliencies;

public class BasicCircuitBreakerPattern
{
    /// <summary>
    /// Ard arda 3 başarısız istek olduğunda 10 saniye bekle. Tekrar istek yaptığında başarısız ise 10 saniye bekledikten sonra tekrar istek yap.
    /// Ard arda 3 başarısız istek, 10 saniye bekle. 10 saniye bekledikten sonra tekrar istek yapıp devreyi açık/yarı açık durumuna getirek.
    /// </summary>
    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        // Arka arkaya 3 istek başarısız olursa devre open'a geçip 10 saniye bekliyor.
        // 11'inci saniyede servise bir istek gönderiyor, servis hatalı durumdaysa tekrar açığa geçiyor ve 10 saniye bekliyor.
        // Eğer 10'uncu saniyede b servisi ayaktaysa devre tekrar kapalı konuma geçiyor. 

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(10),
                OnBreak,
                OnReset,
                OnHalfOpen);
    }

    private void OnBreak(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
    {
        Debug.WriteLine("Circuit Breaker Status => On Break");
    }

    private void OnReset()
    {
        Debug.WriteLine("Circuit Breaker Status => On Reset");
    }

    private void OnHalfOpen()
    {
        Debug.WriteLine("Circuit Breaker Status => On Half Open");
    }
}
