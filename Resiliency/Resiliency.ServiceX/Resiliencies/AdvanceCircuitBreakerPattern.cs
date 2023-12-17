namespace Resiliency.ServiceX.Resiliencies;

public class AdvanceCircuitBreakerPattern
{
    public IAsyncPolicy<HttpResponseMessage> GetAdvanceCircuitBreakerPolicy()
    {
        // 0.5 --> 10 saniye içerisinde başarısız istek sayısı %50'den (0.5) fazlaysa, eğer 0.1 verirsek %10'u temsil eder.
        // yani 10 saniye içinde 100 istekten 10 tanesi başarısız ise. circuit devreye girer. 0.2 derseniz 80 başarılı 20 başarısız ise.
        // minimum çıktı olarak da 5 veriyorum. Yani 10 saniye içerisinden 100 istekten 10 tanesinde devreye girsin ama minimum başarısız sayısı de 5 olsun.
        // diyelim ki 10 saniyede 10 tane istek yaptınız 0.1 olduğu için 1 istek başarısız oldugu için devreye girecek ya ben buraya 5 verirsem artık 5 e kadar bekleyecek.
        // yani 10 saniyede en az 5 tane başarısız olacak devreye girecek. oran dışında minimum başarısız sayısı belirtiyorum.

        // minimumThroughput: minimum başarısız sayısı (oran dışında). 


        // 30 saniye içerisinde minimum 30 istek kafadan meydana gelecek onun dışında birde 0.5 (%50) oranında başarısız hata olacak.
        // (örn: 30 saniye içerisinde 100 isteğin 30'unun hata alması yetmez. çünkü 0.5 ile %50 başarısız oran istedik. 51 isteğin başarısız olması lazım)
        // TimeSpan.FromSeconds(30): sıfırlanma durumu. Her devre açık durumuna geçtiğinde 100 saniye bekleyecek.
        return HttpPolicyExtensions.
            HandleTransientHttpError()
            .AdvancedCircuitBreakerAsync(
            0.1, //başarılı başarısız tüm istekler dahildir. içindeki başarısızlık oranı 
            TimeSpan.FromSeconds(30),
            4,
            TimeSpan.FromSeconds(30), //sıfırlanma durumu
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
