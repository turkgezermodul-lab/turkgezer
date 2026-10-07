# Bilgisayar tabanlı oyun

Önce [ortak kurulumu](00-Kurulum.md) tamamlayın. Bu dağıtım gerçek ISS/astronot tasarımları veya tamamlanmış sekiz görevli oyun içermez; küçük teknik sahneler içerir.

1. `ISS_Ici` veya `ISS_Disi` teknik sahnesinin bir kopyasını kendi uygulama klasörünüze alın.
2. LegacyDesktopInput ile FreeFlightMotor'u bağlayın. WASD yön, fare bakış, Space/sol Control düşey hareket ve E etkileşim için kullanılır. Giriş sistemi Input Manager veya Both olmalıdır.
3. MissionDefinition ve MissionRunner ile kendi görevlerinizi tanımlayın; nesnelere ModuleInteractable bağlayın.
4. Kendi kullanım iznine sahip olduğunuz ISS, astronot ve deney modellerini sahneye yerleştirin. Görseli değiştirmek görev çekirdeğini değiştirmeyi gerektirmez.
5. Deney bilgisi için ExperimentStation; görev arayüzü için kendi paneliniz veya örnek HUD; sahne geçişi için SceneNavigator kullanın.
6. Yalnız gerekli sahneleri Build Settings'e ekleyip hedef işletim sistemi için derleyin. Editör ve derlenmiş uygulamayı ayrı test edin.

İsteğe bağlı Input System adaptörü `Runtime/Adapters/InputSystem` altında bulunur. Kullanılacak paket/sürüm ve action bağlantılarını kendi projenizde yapılandırın. UPM çekirdek kodunu kişisel oyununuzun sabit görev akışıyla değiştirmeyin.
