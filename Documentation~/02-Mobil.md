# Mobil uygulama

Önce [ortak kurulumu](00-Kurulum.md) tamamlayın. `MobilOrnek` sahnesi dokunmatik kontrol bağlantısı örneğidir; dağıtım hazır Android/iOS uygulaması veya özgün tasarım kütüphanesi içermez.

1. Mobil örneği kendi klasörünüze kopyalayın.
2. MobileInputSource'u FreeFlightMotor ve RayInteractor gibi kullanan bileşenlere bağlayın.
3. TouchJoystick ile `SetMove`/`SetLook`; düşey kontrollerle `SetVertical`; etkileşim butonuyla `PressInteract()` girişlerini kullanın. Parmak bırakma/sıfırlama durumlarını kontrol edin.
4. MissionRunner, ModuleInteractable ve ExperimentStation aynı ortak çekirdeği kullanır. Görev tanımı ve sizin modeliniz platform kontrolünden ayrı kalır.
5. Android/iOS derleme araçlarını Unity Hub üzerinden hedefinize uygun kurun. Kamera/izin gerekmezse istemeyin; AR kullanacaksanız izinleri ayrıca tasarlayın.
6. Gerçek cihazda dokunmatik kontrolleri, ekran oranını, ısınmayı, FPS ve belleği test edin. Editor önizlemesi gerçek cihaz doğrulaması değildir.

AI sağlayıcı anahtarını APK/IPA içine koymayın. Kendi HTTPS sunucunuzu kullanın. Büyük modeller için LOD ve optimizasyonu kendi uygulamanızda yapın.
