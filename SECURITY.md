# Güvenli API kullanımı

- API anahtarı, parola, lisans anahtarı, token ve özel sunucu kimlik bilgilerini Git'e eklemeyin. `.gitignore` tek başına sızıntı denetimi değildir.
- `ApiConfiguration.chatEndpoint` ve `chatModel` başlangıçta boştur. Üretim için kendi HTTPS proxy sunucunuzu yerel olarak yapılandırın.
- Sağlayıcı anahtarını proxy sunucusunda tutun. Kaynak koda, ScriptableObject dosyasına, Unity istemcisine veya Android/iOS derlemesine anahtar yazmayın.
- Editor'e özgü doğrudan bağlantı seçeneği varsayılan olarak kapalıdır. Kullanılacaksa yalnız yerel ortam değişkeniyle çalışır; Player derlemesi bu anahtarı okuyacak kodu içermez.
- Vuforia ve platform kimlik bilgilerini kendi uygulama projenizde tutun; bu paket bunları sağlamaz.
- Soru-cevap istekleri yapılandırdığınız dış servise gönderilir. Öğrenci kimliği ve kişisel bilgilerini göndermeyin; kullanım amacına uygun veri koruma ve erişim kontrollerini uygulama geliştiricisi sağlamalıdır.
- ISS sağlayıcılarının herkese açık URL'leri gizli anahtar değildir. Genel bir API adresini yayımlamak hesap kimlik bilgilerini yayımlamak anlamına gelmez.

Bir anahtar yanlışlıkla yayımlanırsa dosyadan kaldırmak yeterli kabul edilmemelidir: ilgili sağlayıcıda anahtarı iptal edip değiştirin, depo geçmişini ayrıca ele alın. Güvenlik bildiriminize anahtarın kendisini veya kişisel verileri eklemeyin.
