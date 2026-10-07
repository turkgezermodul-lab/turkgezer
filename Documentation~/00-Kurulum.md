# Kurulum ve ilk görev

Bu kılavuz 1.2.0 **kod ve hafif teknik örnek dağıtımı** içindir. Özgün ISS, astronot, deney modeli ve ses kütüphanesi bu açık kaynak dağıtımında yoktur. Kendi model ve içeriklerinizi ekleyebilirsiniz.

## Kurulum

1. Unity 2022.3 LTS ile boş 3D Built-in proje açın.
2. UPM: Package Manager > + > Add package from tarball ile `.tgz` dosyasını seçin; ardından Hafif Teknik Baslangic Ornekleri örneğini Import edin. Kaynak klasörle Add package from disk seçeneğinde `package.json` kullanılabilir.
3. Alternatif: Assets > Import Package > Custom Package ile `.unitypackage` dosyasını ekleyin. UPM ve UnityPackage aynı projeye birlikte kurulmaz.
4. Unity UI 1.0.0 ve bildirilen Unity built-in modüllerinin bulunduğunu kontrol edin. UnityPackage otomatik bağımlılık çözmez. Input System/XRI/OpenXR/AR Foundation/Vuforia yalnız kullanılacak platform için ayrıca kurulur.
5. Active Input Handling = Input Manager (Old) veya Both yapın; istenirse Unity'yi yeniden başlatın.
6. Project aramasında `ModulMenu` bulun. Örnek sahneleri Build Settings'e ekleyen Tools > TurkGezer komutu mevcut sahne listenizi değiştirebilir; önce yedek alın.

UPM örneği `Assets/Samples/.../1.2.0/Baslangic`, UnityPackage örneği `Assets/TurkGezerPaket/Ornekler` altında bulunur. Aynı örneği ikinci kez oluşturmayın. Kit oluşturucu yalnız `Assets/TurkGezerOrnekler` altında küçük teknik örnekler üretir.

## Kendi göreviniz

1. `Assets/Uygulamam/Tasarim`, `Prefabs`, `Sahneler`, `Scripts` klasörlerini açın.
2. Create > TurkGezer > Uzay Misyonu ile MissionDefinition oluşturun. Görev kimlikleri, hedef sayıları ve ön koşulları tanımlayın.
3. Sahneye MissionRunner ekleyin; `definition` alanına tanımı bağlayın.
4. Collider içeren bir nesneye ModuleInteractable ekleyin. `mission`, `taskId` ve tekil `uniqueToken` alanlarını bağlayın.
5. `onInteracted` olayına kendi bilgi, ses veya animasyon işleminizi bağlayın. UI butonu aynı nesnenin `Interact()` metodunu çağırabilir.
6. Deney için ExperimentDefinition ve ExperimentStation kullanın. Gerçek içerik/model/sesinizi ve bilimsel kaynağınızı siz eklersiniz.

Görev kimlikleri aynı olmalıdır. Ön koşul tamamlanmadan ilerleme sayılmaz. Sıfırlamada Runner'ın `ResetMission()` ve nesnelerin `ResetInteraction()` işlemlerini birlikte ele alın. Kalıcı kayıt otomatik sağlanmaz.

## Kontrol

Tools > TurkGezer > Cekirdek Testlerini Calistir ve Paket Bagimsizligini Denetle komutlarını kullanın. Bu testler gerçek cihaz testinin veya öğrenci araştırmasının yerine geçmez.
