using System;
using System.IO;
using System.Linq;
using TurkGezer.Api;
using TurkGezer.Diagnostics;
using TurkGezer.Experiments;
using TurkGezer.Interaction;
using TurkGezer.Menu;
using TurkGezer.Missions;
using TurkGezer.Platform;
using TurkGezer.Samples;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TurkGezer.Editor
{
    public static class TurkGezerKitBuilder
    {
        public const string Root = "Assets/TurkGezerOrnekler";
        private static Font font;
        [MenuItem("Tools/TurkGezer/Baslangic Kiti Olustur")]
        public static void CreateStarterKit()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Root + "/Sahneler/Menu/ModulMenu.unity") != null)
            {
                Debug.Log("Baslangic kiti zaten var; mevcut dosyalarin uzerine yazilmadi.");
                return;
            }
            Build(Root);
            AddSampleScenesToBuild();
        }

        public static void Build(string root)
        {
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                foreach (string path in new[] { "Tasarim/Materyaller", "Tasarim/Misyonlar", "Tasarim/Deneyler", "Tasarim/API",
                    "Prefabs/ISS/IcMekan", "Prefabs/ISS/DisMekan", "Prefabs/Deneyler", "Prefabs/Platform", "Prefabs/API",
                    "Sahneler/Menu", "Sahneler/ISS/IcMekan", "Sahneler/ISS/DisMekan", "Sahneler/Mobil", "Sahneler/VR", "Sahneler/AR", "Sahneler/API" })
                    Directory.CreateDirectory(root + "/" + path);
                AssetDatabase.Refresh();
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                var mission = Asset<MissionDefinition>(root + "/Tasarim/Misyonlar/OrnekMisyon.asset");
                mission.missionId = "ornek-arastirma";
                mission.title = "Ornek Arastirma Gorevleri";
                mission.description = "Yeniden kullanim gosteren basit egitsel senaryo; gercek uzay operasyonu degildir.";
                mission.tasks = new System.Collections.Generic.List<MissionTask>
                {
                    new MissionTask { id = "inspect", title = "Numune kutusunu incele" },
                    new MissionTask { id = "experiment", title = "Deney bilgisini incele", prerequisites = new[] { "inspect" } },
                    new MissionTask { id = "report", title = "Veri panelini kontrol et", prerequisites = new[] { "experiment" } }
                };
                var experiment = Asset<ExperimentDefinition>(root + "/Tasarim/Deneyler/OrnekDeney.asset");
                experiment.experimentId = "mikroyercekim";
                experiment.title = "Mikroyercekimde Deneyler";
                experiment.explanation = "ISS ve icindeki cisimler Dunya cevresinde birlikte serbest dusme hareketi yapar. Bu ortam mikroyercekim olarak adlandirilir. Bu kart ornek iceriktir; yeni deneylerinizi kendi bilimsel kaynaklarinizla tanimlayin.";
                experiment.sourceUrl = "https://www.nasa.gov/microgravity/";
                var config = Asset<ApiConfiguration>(root + "/Tasarim/API/ApiAyarlar.asset");
                config.chatEndpoint = "";
                config.chatModel = "";
                config.chatUsesProxy = true;
                config.allowEditorDirectChat = false;
                foreach (var asset in new UnityEngine.Object[] { mission, experiment, config }) EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                var blue = Material(root, "Istasyon", new Color(.1f, .2f, .4f));
                var orange = Material(root, "Deney", new Color(.9f, .4f, .1f));
                var apiObject = new GameObject("TurkGezer_API");
                apiObject.AddComponent<IssLocationService>().configuration = config;
                apiObject.AddComponent<ChatService>().configuration = config;
                PrefabUtility.SaveAsPrefabAsset(apiObject, root + "/Prefabs/API/ApiSistemi.prefab");
                UnityEngine.Object.DestroyImmediate(apiObject);
                var station = new GameObject("ISS_IcMekan_Ornek");
                Primitive("Zemin", PrimitiveType.Cube, station.transform, new Vector3(0, -.6f, 0), new Vector3(8, .2f, 12), blue);
                Primitive("SolDuvar", PrimitiveType.Cube, station.transform, new Vector3(-4, 1, 0), new Vector3(.2f, 3, 12), blue);
                Primitive("SagDuvar", PrimitiveType.Cube, station.transform, new Vector3(4, 1, 0), new Vector3(.2f, 3, 12), blue);
                PrefabUtility.SaveAsPrefabAsset(station, root + "/Prefabs/ISS/IcMekan/IstasyonKoridoru.prefab");
                UnityEngine.Object.DestroyImmediate(station);
                var satellite = new GameObject("ISS_DisMekan_Ornek");
                Primitive("Govde", PrimitiveType.Cylinder, satellite.transform, Vector3.zero, new Vector3(2, 3, 2), blue);
                Primitive("GunesPaneli", PrimitiveType.Cube, satellite.transform, new Vector3(0, 0, 3), new Vector3(10, .1f, 2), blue);
                PrefabUtility.SaveAsPrefabAsset(satellite, root + "/Prefabs/ISS/DisMekan/IstasyonDisModeli.prefab");
                UnityEngine.Object.DestroyImmediate(satellite);
                var expObject = Primitive("DeneyIstasyonu", PrimitiveType.Cube, null, Vector3.zero, Vector3.one, orange);
                expObject.AddComponent<ExperimentStation>().experiment = experiment;
                expObject.GetComponent<ExperimentStation>().label = "Deney Bilgisi";
                PrefabUtility.SaveAsPrefabAsset(expObject, root + "/Prefabs/Deneyler/DeneyIstasyonu.prefab");
                UnityEngine.Object.DestroyImmediate(expObject);
                CreateScene(root, "ISS/IcMekan/ISS_Ici", 0, mission, experiment, config, orange);
                CreateScene(root, "ISS/DisMekan/ISS_Disi", 1, mission, experiment, config, orange);
                CreateScene(root, "Mobil/MobilOrnek", 2, mission, experiment, config, orange);
                CreateScene(root, "VR/VR_BaglantiOrnegi", 3, mission, experiment, config, orange);
                CreateScene(root, "AR/AR_BaglantiOrnegi", 4, mission, experiment, config, orange);
                CreateScene(root, "API/API_Ornek", 5, mission, experiment, config, orange);
                CreateMenu(root);
                AssetDatabase.SaveAssets();
                Debug.Log("[TurkGezer] Baslangic kiti olusturuldu: " + root);
            }
            finally { if (setup.Length > 0) EditorSceneManager.RestoreSceneManagerSetup(setup); }
        }

        private static void CreateScene(string root, string name, int platform, MissionDefinition mission,
            ExperimentDefinition experiment, ApiConfiguration config, UnityEngine.Material material)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            mission = AssetDatabase.LoadAssetAtPath<MissionDefinition>(root + "/Tasarim/Misyonlar/OrnekMisyon.asset");
            experiment = AssetDatabase.LoadAssetAtPath<ExperimentDefinition>(root + "/Tasarim/Deneyler/OrnekDeney.asset");
            config = AssetDatabase.LoadAssetAtPath<ApiConfiguration>(root + "/Tasarim/API/ApiAyarlar.asset");
            material = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(root + "/Tasarim/Materyaller/Deney.mat");
            var light = new GameObject("AnaIsik").AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(40, -30, 0);
            var services = new GameObject("TurkGezer_Sistemler");
            var runner = services.AddComponent<MissionRunner>();
            runner.definition = mission;
            services.AddComponent<PerformanceRecorder>().scenarioId = name;
            var navigator = services.AddComponent<SceneNavigator>();
            var canvas = Canvas();
            var hud = canvas.gameObject.AddComponent<SampleHud>();
            hud.mission = runner;
            hud.progressText = Text(canvas.transform, "Gorevler", new Vector2(20, -25), new Vector2(650, 140), "Gorevler");
            hud.informationText = Text(canvas.transform, "Bilgi", new Vector2(20, -180), new Vector2(850, 140), "WASD: hareket | Fare: bakis | E: etkilesim | SPACE/CTRL: yukari/asagi");
            var player = new GameObject("ModulOyuncusu");
            player.transform.position = new Vector3(0, 1, -4);
            player.AddComponent<PlayerIdentity>();
            var collider = player.AddComponent<CapsuleCollider>();
            collider.height = 1.8f;
            var camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.gameObject.tag = "MainCamera";
            camera.transform.SetParent(player.transform, false);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.01f, .02f, .06f);
            camera.gameObject.AddComponent<AudioListener>();
            ModuleInputSource input;
            if (platform == 2) input = player.AddComponent<MobileInputSource>();
            else input = player.AddComponent<LegacyDesktopInput>();
            var motor = player.AddComponent<FreeFlightMotor>();
            motor.input = input;
            motor.view = camera.transform;
            motor.captureDesktopCursor = platform == 0 || platform == 1;
            var ray = player.AddComponent<RayInteractor>();
            ray.input = input;
            ray.rayOrigin = camera.transform;
            ray.distance = 10;
            var center = Text(canvas.transform, "Nisangah", Vector2.zero, new Vector2(30, 30), "+");
            center.rectTransform.anchorMin = center.rectTransform.anchorMax = new Vector2(.5f, .5f);
            center.alignment = TextAnchor.MiddleCenter;
            if (platform == 0 || platform == 2)
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(root + "/Prefabs/ISS/IcMekan/IstasyonKoridoru.prefab"));
            else if (platform == 1 || platform == 3)
            {
                var exterior = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(root + "/Prefabs/ISS/DisMekan/IstasyonDisModeli.prefab"));
                exterior.transform.position = new Vector3(0, 2, 12);
            }
            var content = new GameObject("OrtakGorevIcerigi");
            var cargo = Primitive("NumuneKutusu", PrimitiveType.Cube, content.transform, new Vector3(-2, 1, 1), Vector3.one, material);
            var cargoInteraction = cargo.AddComponent<ModuleInteractable>();
            cargoInteraction.label = "Numune";
            cargoInteraction.mission = runner; cargoInteraction.taskId = "inspect";
            var expObject = Primitive("Deney", PrimitiveType.Sphere, content.transform, new Vector3(0, 1, 2), Vector3.one, material);
            var station = expObject.AddComponent<ExperimentStation>();
            station.experiment = experiment;
            station.mission = runner; station.taskId = "experiment";
            UnityEventTools.AddPersistentListener(station.onInformation, hud.ShowInformation);
            var antenna = Primitive("VeriPaneli", PrimitiveType.Cube, content.transform, new Vector3(2, 1, 1), Vector3.one, material);
            var antennaInteraction = antenna.AddComponent<ModuleInteractable>();
            antennaInteraction.mission = runner; antennaInteraction.taskId = "report";
            var iss = services.AddComponent<IssLocationService>();
            iss.configuration = config;
            iss.startAutomatically = platform == 5;
            hud.positionText = Text(canvas.transform, "ISSKonum", new Vector2(880, -25), new Vector2(380, 200), "ISS API istege bagli. API orneginde otomatik baslar.");
            UnityEventTools.AddPersistentListener(iss.onDisplay, hud.ShowPosition);
            UnityEventTools.AddPersistentListener(iss.onError, hud.ShowPosition);
            var chat = services.AddComponent<ChatService>();
            chat.configuration = config; hud.chat = chat;
            UnityEventTools.AddPersistentListener(chat.onResponse, hud.ShowInformation);
            UnityEventTools.AddPersistentListener(chat.onError, hud.ShowInformation);
            Button(canvas.transform, "MenuyeDon", new Vector2(1030, -640), "Menu", null, false);
            var menuButton = canvas.transform.Find("MenuyeDon").GetComponent<Button>();
            UnityEventTools.AddStringPersistentListener(menuButton.onClick, navigator.Load, "ModulMenu");
            if (platform == 2)
            {
                var mobile = (MobileInputSource)input;
                Joystick(canvas.transform, "Hareket", new Vector2(100, -530), mobile, false);
                Joystick(canvas.transform, "Bakis", new Vector2(940, -530), mobile, true);
                var action = Button(canvas.transform, "Etkilesim", new Vector2(1070, -530), "Etkilesim", null, false);
                UnityEventTools.AddPersistentListener(action.onClick, ray.Interact);
                hud.informationText.text = "Sol joystick: hareket | Sag joystick: bakis | Etkilesim butonu: hedefle etkilesim";
            }
            if (platform == 3)
            {
                motor.rotateView = false;
                motor.enabled = false;
                var xrType = Type.GetType("TurkGezer.Platform.XRDeviceInputSource, TurkGezer.XR");
                if (xrType != null)
                {
                    input.enabled = false;
                    var xr = (ModuleInputSource)player.AddComponent(xrType);
                    var serializedXR = new SerializedObject(xr);
                    serializedXR.FindProperty("trackedHead").objectReferenceValue = camera.transform;
                    serializedXR.ApplyModifiedPropertiesWithoutUndo();
                    motor.input = xr; motor.enabled = true;
                    ray.input = xr;
                }
                hud.informationText.text = "XR BAGLANTI ORNEGI: XR Plug-in Management ve OpenXR/Meta saglayicisi gerekir. Bu sahne tek basina gozluk takibi baslatmaz. XRI selectEntered -> XRInteractionBridge.Select.";
                foreach (var target in new ModuleInteractable[] { cargoInteraction, station, antennaInteraction })
                    target.gameObject.AddComponent<XRInteractionBridge>().target = target;
            }
            if (platform == 4)
            {
                motor.enabled = false;
                var tracker = services.AddComponent<ARTrackingBridge>();
                tracker.contentRoot = content;
                var show = Button(canvas.transform, "TakipOnizleme", new Vector2(25, -400), "Takip onizlemesi", null, false);
                UnityEventTools.AddPersistentListener(show.onClick, tracker.TrackingFound);
                var exp = Button(canvas.transform, "DeneyKartiniAc", new Vector2(260, -400), "Deney bilgisi", null, false);
                station.mission = null;
                UnityEventTools.AddPersistentListener(exp.onClick, station.Interact);
                hud.informationText.text = "AR BAGLANTI ORNEGI: Vuforia/AR Foundation ve kamera/marker ayarlari gerekir. Found/Lost olaylarini ARTrackingBridge'e baglayin. Onizleme butonu gercek AR takibi degildir.";
            }
            if (platform == 5)
            {
                motor.enabled = false;
                var inputObject = new GameObject("SoruGirisi", typeof(RectTransform), typeof(Image), typeof(InputField));
                inputObject.transform.SetParent(canvas.transform, false);
                var rect = inputObject.GetComponent<RectTransform>(); Position(rect, new Vector2(25, -360), new Vector2(600, 50));
                var question = inputObject.GetComponent<InputField>();
                var label = Text(inputObject.transform, "Text", new Vector2(10, -5), new Vector2(580, 40), "");
                label.color = Color.black;
                question.textComponent = label;
                hud.questionInput = question;
                var ask = Button(canvas.transform, "Sor", new Vector2(650, -360), "Sor", null, false);
                UnityEventTools.AddPersistentListener(ask.onClick, hud.SendQuestion);
                hud.informationText.text = "API ornegi: ISS konumu acik; soru-cevap icin ApiAyarlar assetinde kendi HTTPS proxy adresinizi girin. API anahtari paketlenmez.";
            }
            if (platform == 0 || platform == 2 || platform == 3)
                PrefabUtility.SaveAsPrefabAsset(player, root + "/Prefabs/Platform/" + (platform == 2 ? "MobilOyuncu" : platform == 3 ? "XRBaglanti" : "BilgisayarOyuncu") + ".prefab");
            EditorSceneManager.SaveScene(scene, root + "/Sahneler/" + name + ".unity");
        }

        private static void CreateMenu(string root)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var canvas = Canvas();
            Text(canvas.transform, "Baslik", new Vector2(40, -30), new Vector2(1000, 80), "TURKGEZER MODULU - BASLANGIC KITI");
            Text(canvas.transform, "Aciklama", new Vector2(40, -100), new Vector2(1100, 80), "Ayni gorev cekirdegi, farkli platform baglantilari. VR ve AR icin ilgili saglayici/SDK kurulumunu tamamlayin.");
            var navigator = canvas.gameObject.AddComponent<SceneNavigator>();
            var names = new[] { "ISS/IcMekan/ISS_Ici", "ISS/DisMekan/ISS_Disi", "Mobil/MobilOrnek", "VR/VR_BaglantiOrnegi", "AR/AR_BaglantiOrnegi", "API/API_Ornek" };
            for (int i = 0; i < names.Length; i++)
            {
                var button = Button(canvas.transform, "Ornek" + i, new Vector2(50, -200 - i * 65), names[i], null, false);
                UnityEventTools.AddStringPersistentListener(button.onClick, navigator.Load, Path.GetFileName(names[i]));
            }
            EditorSceneManager.SaveScene(scene, root + "/Sahneler/Menu/ModulMenu.unity");
        }
        public static void CreateSettingsScene(string path, string returnScene)
        {
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                var canvas = Canvas();
                Text(canvas.transform, "Baslik", new Vector2(60, -70), new Vector2(900, 80), "TURKGEZER - SES AYARLARI");
                var settings = canvas.gameObject.AddComponent<TurkGezer.Menu.AudioSettings>();
                var sliderObject = new GameObject("Ses", typeof(RectTransform), typeof(Slider));
                sliderObject.transform.SetParent(canvas.transform, false);
                Position(sliderObject.GetComponent<RectTransform>(), new Vector2(70, -210), new Vector2(500, 40));
                var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
                background.transform.SetParent(sliderObject.transform, false);
                Position(background.GetComponent<RectTransform>(), Vector2.zero, new Vector2(500, 40));
                var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
                handle.transform.SetParent(sliderObject.transform, false);
                handle.GetComponent<RectTransform>().sizeDelta = new Vector2(35, 40);
                var slider = sliderObject.GetComponent<Slider>();
                slider.handleRect = handle.GetComponent<RectTransform>();
                slider.targetGraphic = handle.GetComponent<Image>();
                settings.volumeSlider = slider;
                UnityEventTools.AddPersistentListener(slider.onValueChanged, settings.SetVolume);
                var navigation = canvas.gameObject.AddComponent<SceneNavigator>();
                var back = Button(canvas.transform, "Geri", new Vector2(70, -330), "Menuye don", null, false);
                UnityEventTools.AddStringPersistentListener(back.onClick, navigation.Load, returnScene);
                EditorSceneManager.SaveScene(scene, path);
            }
            finally { if (setup.Length > 0) EditorSceneManager.RestoreSceneManagerSetup(setup); }
        }
        private static void Joystick(Transform parent, string name, Vector2 position, MobileInputSource input, bool look)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            Position(obj.GetComponent<RectTransform>(), position, new Vector2(150, 150));
            obj.GetComponent<Image>().color = new Color(.2f, .3f, .4f, .8f);
            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(obj.transform, false);
            handle.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 45);
            var joystick = obj.AddComponent<TouchJoystick>();
            joystick.input = input; joystick.controlsLook = look; joystick.handle = handle.GetComponent<RectTransform>();
        }
        private static Canvas Canvas()
        {
            var obj = new GameObject("ModulCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = obj.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = obj.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            var system = new GameObject("EventSystem", typeof(EventSystem));
            system.AddComponent<StandaloneInputModule>();
            return canvas;
        }
        private static Text Text(Transform parent, string name, Vector2 position, Vector2 size, string value)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Text)); obj.transform.SetParent(parent, false);
            var text = obj.GetComponent<Text>(); text.font = font; text.fontSize = 20; text.text = value;
            text.color = Color.white; text.raycastTarget = false;
            Position(text.rectTransform, position, size); return text;
        }
        private static Button Button(Transform parent, string name, Vector2 position, string label, UnityEngine.Events.UnityAction action, bool runtime)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button)); obj.transform.SetParent(parent, false);
            Position(obj.GetComponent<RectTransform>(), position, new Vector2(220, 50));
            obj.GetComponent<Image>().color = new Color(.1f, .3f, .5f);
            Text(obj.transform, "Label", new Vector2(8, -8), new Vector2(205, 40), label);
            var button = obj.GetComponent<Button>();
            if (runtime && action != null) button.onClick.AddListener(action);
            return button;
        }
        private static void Position(RectTransform rect, Vector2 position, Vector2 size)
        { rect.anchorMin = rect.anchorMax = new Vector2(0, 1); rect.pivot = new Vector2(0, 1); rect.anchoredPosition = position; rect.sizeDelta = size; }
        private static GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, UnityEngine.Material material)
        {
            var obj = GameObject.CreatePrimitive(type); obj.name = name; obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position; obj.transform.localScale = scale; obj.GetComponent<Renderer>().sharedMaterial = material;
            return obj;
        }
        private static UnityEngine.Material Material(string root, string name, Color color)
        {
            string path = root + "/Tasarim/Materyaller/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(path);
            if (material == null) { material = new UnityEngine.Material(Shader.Find("Standard")); AssetDatabase.CreateAsset(material, path); }
            material.color = color; EditorUtility.SetDirty(material); return material;
        }
        private static T Asset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null) { asset = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset, path); }
            return asset;
        }
        [MenuItem("Tools/TurkGezer/Ornek Sahneleri Build Settings'e Ekle")]
        public static void AddSampleScenesToBuild()
        {
            var existing = EditorBuildSettings.scenes.ToList();
            foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.StartsWith(Root + "/") && !path.Contains("TurkGezerPaket/Ornekler/") &&
                    !path.StartsWith("Assets/TurkGezerKutuphane/") && !path.Contains("TurkGezerPaket/Kutuphane/") &&
                    !(path.StartsWith("Assets/Samples/") && (path.Contains("Baslangic/") || path.Contains("TasarimKutuphane/")))) continue;
                if (!existing.Any(item => item.path == path)) existing.Add(new EditorBuildSettingsScene(path, true));
            }
            EditorBuildSettings.scenes = existing.ToArray();
        }
    }
}
