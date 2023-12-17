namespace Resiliency.ServiceX.Resiliencies;

public class RetryPattern
{
    /// <summary>
    /// Eğer belirtilen hatlaarda başarısız olursa 5 kez tekrarlar, her bir tekrar arasında 10 saniye bekle.
    /// </summary>
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(httpResponseMessage => httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(5, OnRetryAttempt, OnRetryAsync);
    }

    private TimeSpan OnRetryAttempt(int retryAttempt)
    {
        Debug.WriteLine($"Retry Count :{retryAttempt}");

        return TimeSpan.FromSeconds(10);
    }

    /// <summary>
    /// Business çalıştırmak istediğinizde. | Tekrar istek yapılmadan farklı işlemler yapılmak istendiğinde.
    /// </summary>
    private Task OnRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
    {
        Debug.WriteLine($"Request is made again:{arg2.TotalMilliseconds}");

        return Task.CompletedTask;
    }
}
