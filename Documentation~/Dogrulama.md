# Açık kaynak dağıtımının doğrulanması

Bu kayıt yalnız TürkGezer 1.2.0 açık kaynak kodu ve hafif teknik örneklerini kapsar. Lisansı doğrulanmamış tasarım kütüphanesi teste ve dağıtıma dahil değildir.

## Yerel UPM kaynak kurulumu

- Tarih: 7 Ekim 2026, 14:22 UTC.
- Ortam: Windows, Unity 2022.3.62f1, temiz 3D proje; batchmode ve nographics.
- Kurulum: `package.json` üzerinden yerel UPM paket referansı.
- Derleme: doğrulama metodu çalıştırılabilecek şekilde tamamlandı.
- Çekirdek: ModuleSelfTests içindeki 21 görev/veri ayrıştırma doğrulaması başarılı.
- İzolasyon: eski oyun kodu/sahnesi ve paket dışı bildirilmemiş GUID bağımlılığı bulunmadı.
- Hafif örnekler: 7 sahne ve 7 prefab açılarak denetlendi; eksik script veya nesne referansı bulunmadı.
- Paketleme: yalnız modül kökünden `.unitypackage` oluşturuldu; proje Assets klasörü veya bağımlılıklarının tamamı dışa aktarılmadı.

## Bu testin kanıtlamadığı konular

Başarılı derleme ve referans denetimi; görsel kalite, oynanabilirlik, bilimsel doğruluk, öğrenci öğrenmesi, FPS, geliştirme süresi veya gerçek cihaz uyumu ölçümü değildir. Android/iOS derlemesi, Quest, Vuforia/AR Foundation, kamera/QR takibi ve canlı API bağlantısı bu kontrolde çalıştırılmamıştır. Paket arşivinden kurulum ile UPM Git bağlantısı ayrıca doğrulanmalıdır.

Çekirdek testleri ve izolasyon denetimi Editor menüsünden tekrar çalıştırılabilir. Teknik örnekler, bitmiş eğitsel oyun veya hazır XR/AR uygulaması olarak değerlendirilmemelidir.
