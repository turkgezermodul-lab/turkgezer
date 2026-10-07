# API, içerik ve genişletme

## Ortak çekirdek

Görev ve deney tanımları ScriptableObject varlıklarıdır. MissionDefinition/MissionRunner görev ön koşulları ve ilerlemeyi; ModuleInteractable ortak etkileşimi; ExperimentDefinition/ExperimentStation deney bilgi ve ses bağlantılarını yönetir. Klavye, mobil, XR ve AR girişlerini bu ortak yapıya bağlayın. Sahne ve içerikler uygulamanıza aittir; bu kaynak depo yalnız küçük teknik örnekler içerir.

## ISS ve soru-cevap

Create > TurkGezer > API Yapilandirmasi ile ApiConfiguration oluşturun. IssLocationService ve ChatService'e bu yapılandırmayı bağlayın. ISS sağlayıcıları genel konum hizmetleridir; internet ve sağlayıcı limitleri geçerlidir. Genel sağlayıcı URL'leri anahtar değildir.

ChatService'in `chatEndpoint` ve `chatModel` alanları dağıtımda boştur. Kendi HTTPS sunucunuzu yerel uygulama yapılandırmasında belirtin. Proxy sağlayıcı anahtarını sunucuda tutar. Yanıt şeması `choices[0].message.content` alanını kullanır; proxy'nizin uyumunu test edin. Hazır sunucu ve hesap erişimi bu pakette yoktur.

AI yanıtları doğrulanmış bilimsel kaynak yerine geçmez. Bilimsel içerikleri öğretmen/alan uzmanı ile inceleyin; modelin hatalı yanıtlarını, zaman aşımını ve çevrimdışı durumu uygulama arayüzünüzde ele alın. Öğrenci kimlik bilgilerini dış servise göndermeyin.

## Yeni içerik ve fiziksel materyaller

Yeni modelleri kendi Prefabs/Tasarim klasörünüze ekleyin ve modülün görev/deney tanımlarına bağlayın. ContentExporter kodu model geometrisinin STL dışa aktarımı ve tanımlardan kart üretimi için yardımcı araçlar sunar. STL çıktısı fiziksel baskıya uygunluğu veya deney kartının bilimsel doğruluğunu otomatik kanıtlamaz. Modelin yeniden dağıtım/baskı hakkını ve materyalin bilimsel içeriğini ayrıca kontrol edin.

## Teknik testler ve dağıtım

Tools > TurkGezer > Cekirdek Testlerini Calistir ile çekirdek kontrolünü yapın. PackageIsolationAudit dış GUID ve eski oyun bağımlılıklarını kontrol eder. PerformanceRecorder uygulama içinde ölçüm kaydı sağlar; cihaz/derleme ortamı, sahne, çözünürlük, ayarlar ve ölçüm süresini raporlayın. Geliştirme süresi otomatik olarak FPS kaydından çıkarılamaz.

ModuleReleaseBuilder bu açık kaynak varyantında yalnız hafif teknik örnekleri hazırlar ve lisansı doğrulanmamış TasarimKutuphane klasörünün eklenmesini reddeder. ModuleExporter modülü `.unitypackage` olarak dışa aktarır. UPM `.tgz` arşivi paket klasörünü `package/` kökü altında içerir. Kaynak kod ile sürüm paketlerini aynı içerik kapsamından üretin.
