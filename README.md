# TürkGezer: Millî Uzay Misyonu Odaklı 3B Astronomi ve Uzay Uygulamaları Geliştirme Modülü

TürkGezer, Unity 2022.3 LTS için yeniden kullanılabilir C# görev, deney, etkileşim, menü, ISS konum ve isteğe bağlı yapay zekâ soru-cevap bileşenleri sunan bir yazılım modülüdür. Aynı çekirdek, bilgisayar ve mobil uygulamalarda; uygun SDK bağlantıları sağlandığında VR ve AR uygulamalarında kullanılabilir.

## Bu açık kaynak dağıtımının kapsamı

Bu depo **1.2.0 sürümünün kod ve hafif teknik örnek dağıtımıdır**. Özgün ISS, astronot, 13 deney modeli, doku, animasyon ve ses kütüphanesi; yeniden dağıtım hakları doğrulanmadığı için bu depoya ve bu depodan hazırlanan paketlere dahil edilmez. Bu içerikler yerel çalışma projesinden silinmemiştir. Teknik örneklerde Unity'nin basit geometrileri kullanılır; bunlar özgün tasarımların yerine geçmez.

Depo bitmiş bilgisayar/mobil oyunu veya hazır Quest/AR uygulaması değil, bu uygulamalarda kullanılabilecek ortak altyapıdır. Kendi kullanım iznine sahip olduğunuz model ve içerikleri modüle bağlayabilirsiniz.

## Hızlı başlangıç

1. Unity 2022.3 LTS ile boş bir 3D Built-in proje açın.
2. UPM `.tgz` paketini **Window > Package Manager > + > Add package from tarball** yoluyla ekleyin. Alternatif `.unitypackage` dosyası **Assets > Import Package > Custom Package** ile eklenir. İki biçimi aynı projeye birlikte kurmayın.
3. UPM kullanıyorsanız **Samples > Hafif Teknik Baslangic Ornekleri > Import** seçin.
4. **Active Input Handling = Input Manager (Old) veya Both** ayarlayın. İstenirse Unity'yi yeniden başlatın.
5. Project penceresinde `ModulMenu` arayın ve sahneyi açın. UPM örnekleri `Assets/Samples/.../1.2.0/Baslangic`, UnityPackage örnekleri `Assets/TurkGezerPaket/Ornekler` altında bulunur.
6. Gerekirse **Tools > TurkGezer > Ornek Sahneleri Build Settings'e Ekle** komutunu çalıştırın. Bu işlem mevcut sahne listenizi etkileyebilir; önce yedeğini alın.

Kaynak kodla kurulum için bu depo klasörünü **Add package from disk** ile `package.json` üzerinden ekleyebilirsiniz.

## Modüler yapı

| Bölüm | İşlev |
| --- | --- |
| `Runtime/Scripts/UzayMisyonu` | Görev tanımı, ön koşullar, ilerleme ve tamamlanma |
| `Runtime/Scripts/Deneyler` | Deney tanımları, bilgi ve anlatım bağlantıları |
| `Runtime/Scripts/Etkilesim` | Ortak nesne etkileşimleri |
| `Runtime/Scripts/Platform` | Klavye-fare, dokunmatik, SDK'dan bağımsız VR/AR köprüleri |
| `Runtime/Scripts/ISS` | Uzay istasyonu örnek bileşenleri |
| `Runtime/Scripts/API` | Genel ISS verisi ve yapılandırılabilir soru-cevap servisi |
| `Runtime/Scripts/Menu` | Sahne geçişleri, ses ayarları ve örnek arayüz |
| `Runtime/Scripts/TeknikTest` | Çalışma zamanı performans kayıt bileşeni |
| `Runtime/Adapters` | İsteğe bağlı platform/SDK adaptörleri |
| `Samples~/Baslangic` | Küçük teknik prefab, tanım ve sahne örnekleri |
| `Editor` | Kit oluşturma, test, paketleme ve dışa aktarma araçları |

## Platform kılavuzları

- [Kurulum ve ilk görev](Documentation~/00-Kurulum.md)
- [Bilgisayar oyunu](Documentation~/01-Bilgisayar-Oyunu.md)
- [Mobil](Documentation~/02-Mobil.md)
- [Sanal gerçeklik (VR)](Documentation~/03-VR.md)
- [Artırılmış gerçeklik (AR)](Documentation~/04-AR.md)
- [API ve genişletme](Documentation~/Kullanim.md)

UnityPackage dağıtımında aynı kılavuzlar `Dokumantasyon` altındadır.

## Güvenlik

Bu dağıtım API anahtarı, Vuforia lisans anahtarı, parola, erişim belirteci veya özel sohbet sunucusu adresi içermez. Soru-cevap sunucu/model alanları boş bırakılır; sunucuyu geliştirici yapılandırır. Üretimde sağlayıcı anahtarını Unity istemcisine/APK'ya koymayın; kendi HTTPS sunucunuzda tutun. Ayrıntılar: [SECURITY.md](SECURITY.md).

ISS sağlayıcılarının herkese açık adresleri kimlik doğrulama sırrı değildir; ISS bileşeninin çalışması için kaynakta bulunabilir. İnternet ve dış servisler olmadan görev/deney çekirdeği kullanılabilir.

## Test ve sınırlar

**Tools > TurkGezer > Cekirdek Testlerini Calistir** ile görev ve veri ayrıştırma testlerini; **Paket Bagimsizligini Denetle** ile referans/izolasyon kontrolünü çalıştırın. Yerel tam 1.2.0 sürümünde yapılmış testler, bu değiştirilmiş açık kaynak dağıtımının veya gerçek cihazların otomatik doğrulaması sayılmaz. Bu dağıtımın kendi kontrol sonucu ayrıca kaydedilir.

7 Ekim 2026 tarihinde açık kaynak kaynak kodu temiz bir Unity 2022.3.62f1 projesinde derlenmiştir: 21 çekirdek doğrulama başarılıdır; 7 teknik sahne ve 7 prefabda eksik script veya nesne referansı bulunmamıştır. Kapsam ve sınırlar [doğrulama notunda](Documentation~/Dogrulama.md) açıklanmıştır.

VR örneği hazır gözlük sağlayıcısı/rig kurmaz. AR örneğindeki önizleme gerçek kamera takibi veya QR okuyucu değildir. Gerçek mobil cihaz, Quest, Vuforia/AR Foundation takibi ve kullanıcı uygulamaları ayrıca kurulup test edilmelidir. Materyaller Built-in içindir; URP/HDRP için uyarlama gerekir. Görev ilerlemesi otomatik olarak diske kaydedilmez.

## Lisans ve katkı

Yeni modül kodu ve basit geometrili teknik örnekler [MIT lisanslıdır](LICENSE.md). Üçüncü taraf SDK'lar kendi koşullarına tabidir; [üçüncü taraf açıklamalarını](Third%20Party%20Notices.md) inceleyin. Yeniden dağıtım hakkı belirsiz içerikleri, öğrenci verilerini, özel sunucu adreslerini ve anahtarları katkı olarak göndermeyin.

Yeni görev, içerik veya platform bağlantısı eklerken ortak çekirdeği uygulamanızın sabit oyun akışından ayrı tutun. Hata bildirirken Unity sürümü, kurulum biçimi ve kişisel/gizli bilgi içermeyen tekrar adımlarını paylaşın.
