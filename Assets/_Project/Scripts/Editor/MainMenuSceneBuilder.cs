using System.Collections.Generic;
using Codebound.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Codebound.EditorTools
{
    public static class MainMenuSceneBuilder
    {
        const string ScenesFolder = "Assets/_Project/Scenes";
        const string ScenePath = ScenesFolder + "/MainMenu.unity";

        static readonly Color PanelColor = new Color(0.106f, 0.106f, 0.184f);
        static readonly Color ButtonNormal = new Color(0.23f, 0.23f, 0.42f);
        static readonly Color ButtonHighlight = new Color(0.38f, 0.38f, 0.66f);
        static readonly Color ButtonPressed = new Color(0.15f, 0.15f, 0.30f);
        static readonly Color ButtonDisabled = new Color(0.2f, 0.2f, 0.25f, 0.6f);
        static readonly Color HintColor = new Color(0.7f, 0.7f, 0.8f);
        static readonly Color NoticeColor = new Color(1f, 0.8f, 0.4f);

        [MenuItem("Codebound/Build Main Menu Scene")]
        public static void Build()
        {
            if (TMP_Settings.instance == null)
            {
                EditorUtility.DisplayDialog("TextMeshPro setup needed",
                    "Import the TMP Essentials first:\nWindow > TextMeshPro > Import TMP Essentials.\nThen run this menu item again.",
                    "OK");
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null &&
                !EditorUtility.DisplayDialog("Rebuild the Main Menu?",
                    "MainMenu.unity already exists and will be replaced. Any manual edits in it will be lost.",
                    "Replace", "Cancel"))
                return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            SetUpCamera();

            var canvas = CreateCanvas();
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            var title = CreatePanel(canvas, "TitlePanel");
            var settings = CreatePanel(canvas, "SettingsPanel");
            var credits = CreatePanel(canvas, "CreditsPanel");
            var confirmNewGame = CreatePanel(canvas, "ConfirmNewGamePanel");
            var confirmQuit = CreatePanel(canvas, "ConfirmQuitPanel");
            var placeholder = CreatePanel(canvas, "PlaceholderPanel");

            CreateText(title.transform, "LogoText", "CODEBOUND", 120, new Vector2(0, 380), new Vector2(1400, 160));
            CreateText(title.transform, "SubtitleText", "The Programming Dungeon", 48, new Vector2(0, 270), new Vector2(900, 60));
            var column = CreateColumn(title.transform, "ButtonColumn", new Vector2(0, -60), new Vector2(500, 420), 16);
            var newGame = CreateLayoutButton(column, "NewGameButton", "New Game");
            var continueButton = CreateLayoutButton(column, "ContinueButton", "Continue");
            var settingsButton = CreateLayoutButton(column, "SettingsButton", "Settings");
            var creditsButton = CreateLayoutButton(column, "CreditsButton", "Credits");
            var quitButton = CreateLayoutButton(column, "QuitButton", "Quit");
            var noSaveHint = CreateText(title.transform, "NoSaveHint", "No saved game yet", 32,
                new Vector2(0, -320), new Vector2(700, 50), HintColor);
            var notice = CreateText(title.transform, "NoticeText", string.Empty, 30,
                new Vector2(0, -390), new Vector2(1400, 50), NoticeColor);

            CreateText(settings.transform, "SettingsTitle", "Settings", 90, new Vector2(0, 380), new Vector2(900, 120));
            var settingsColumn = CreateColumn(settings.transform, "SettingsColumn", new Vector2(0, -20), new Vector2(700, 500), 30);
            CreateLayoutLabel(settingsColumn, "MusicLabel", "Music");
            var musicSlider = CreateSlider(settingsColumn, "MusicSlider");
            CreateLayoutLabel(settingsColumn, "SfxLabel", "Effects");
            var sfxSlider = CreateSlider(settingsColumn, "SfxSlider");
            var settingsBack = CreateLayoutButton(settingsColumn, "BackButton", "Back");

            CreateText(credits.transform, "CreditsTitle", "Credits", 90, new Vector2(0, 380), new Vector2(900, 120));
            CreateText(credits.transform, "CreditsBody",
                "Codebound: The Programming Dungeon\n\nA university game development project.\nBuilt with Unity.\n\nAsset credits will be listed here.",
                36, new Vector2(0, 40), new Vector2(1200, 500));
            var creditsBack = CreateButton(credits.transform, "BackButton", "Back", new Vector2(0, -400), new Vector2(400, 70));

            CreateText(confirmNewGame.transform, "Message",
                "Starting a new game replaces your saved progress. Continue?", 48, new Vector2(0, 120), new Vector2(1200, 200));
            var newGameYes = CreateButton(confirmNewGame.transform, "YesButton", "Yes, start new", new Vector2(-240, -100), new Vector2(420, 70));
            var newGameNo = CreateButton(confirmNewGame.transform, "NoButton", "No, go back", new Vector2(240, -100), new Vector2(420, 70));

            CreateText(confirmQuit.transform, "Message", "Quit Codebound?", 60, new Vector2(0, 120), new Vector2(1200, 200));
            var quitYes = CreateButton(confirmQuit.transform, "YesButton", "Yes, quit", new Vector2(-240, -100), new Vector2(420, 70));
            var quitNo = CreateButton(confirmQuit.transform, "NoButton", "No, stay", new Vector2(240, -100), new Vector2(420, 70));

            var placeholderText = CreateText(placeholder.transform, "PlaceholderText", "Coming soon.", 48,
                new Vector2(0, 60), new Vector2(1200, 300));
            var placeholderBack = CreateButton(placeholder.transform, "BackButton", "Back", new Vector2(0, -250), new Vector2(400, 70));

            var controllerObject = new GameObject("MenuController");
            var controller = controllerObject.AddComponent<MainMenuController>();
            var so = new SerializedObject(controller);
            Assign(so, "_titlePanel", title);
            Assign(so, "_settingsPanel", settings);
            Assign(so, "_creditsPanel", credits);
            Assign(so, "_confirmNewGamePanel", confirmNewGame);
            Assign(so, "_confirmQuitPanel", confirmQuit);
            Assign(so, "_placeholderPanel", placeholder);
            Assign(so, "_newGameButton", newGame);
            Assign(so, "_continueButton", continueButton);
            Assign(so, "_settingsButton", settingsButton);
            Assign(so, "_creditsButton", creditsButton);
            Assign(so, "_quitButton", quitButton);
            Assign(so, "_noSaveHint", noSaveHint.gameObject);
            Assign(so, "_noticeText", notice);
            Assign(so, "_musicSlider", musicSlider);
            Assign(so, "_sfxSlider", sfxSlider);
            Assign(so, "_settingsBackButton", settingsBack);
            Assign(so, "_creditsBackButton", creditsBack);
            Assign(so, "_confirmNewGameYesButton", newGameYes);
            Assign(so, "_confirmNewGameNoButton", newGameNo);
            Assign(so, "_confirmQuitYesButton", quitYes);
            Assign(so, "_confirmQuitNoButton", quitNo);
            Assign(so, "_placeholderText", placeholderText);
            Assign(so, "_placeholderBackButton", placeholderBack);
            so.ApplyModifiedPropertiesWithoutUndo();

            if (!AssetDatabase.IsValidFolder(ScenesFolder))
                AssetDatabase.CreateFolder("Assets/_Project", "Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddToBuildSettings();

            Selection.activeGameObject = controllerObject;
            Debug.Log("MainMenu scene built and saved to " + ScenePath);
        }

        static void SetUpCamera()
        {
            var light = GameObject.Find("Directional Light");
            if (light != null) Object.DestroyImmediate(light);

            var camera = Camera.main;
            if (camera == null) return;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = PanelColor;
            camera.orthographic = true;
        }

        static Transform CreateCanvas()
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            return go.transform;
        }

        static GameObject CreatePanel(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            Stretch((RectTransform)go.transform);
            go.GetComponent<Image>().color = PanelColor;
            return go;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static TextMeshProUGUI CreateText(Transform parent, string name, string text, float size,
            Vector2 position, Vector2 dimensions, Color? color = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions;

            var label = go.AddComponent<TextMeshProUGUI>();
            label.font = TMP_Settings.defaultFontAsset;
            label.text = text;
            label.fontSize = size;
            label.alignment = TextAlignmentOptions.Center;
            label.color = color ?? Color.white;
            label.raycastTarget = false;
            return label;
        }

        static RectTransform CreateColumn(Transform parent, string name, Vector2 position, Vector2 dimensions, float spacing)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions;

            var layout = go.GetComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return rect;
        }

        static Button CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 dimensions)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions;

            var image = go.GetComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            var button = go.GetComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = ButtonNormal;
            colors.highlightedColor = ButtonHighlight;
            colors.selectedColor = ButtonHighlight;
            colors.pressedColor = ButtonPressed;
            colors.disabledColor = ButtonDisabled;
            colors.colorMultiplier = 1f;
            button.colors = colors;

            var text = CreateText(go.transform, "Label", label, 40, Vector2.zero, dimensions);
            Stretch(text.rectTransform);
            return button;
        }

        static Button CreateLayoutButton(Transform parent, string name, string label)
        {
            var button = CreateButton(parent, name, label, Vector2.zero, new Vector2(500, 70));
            button.gameObject.AddComponent<LayoutElement>().preferredHeight = 70;
            return button;
        }

        static void CreateLayoutLabel(Transform parent, string name, string text)
        {
            var label = CreateText(parent, name, text, 40, Vector2.zero, new Vector2(700, 50));
            label.gameObject.AddComponent<LayoutElement>().preferredHeight = 50;
        }

        static Slider CreateSlider(Transform parent, string name)
        {
            var resources = new DefaultControls.Resources
            {
                standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
                background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
                knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd")
            };
            var go = DefaultControls.CreateSlider(resources);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.AddComponent<LayoutElement>().preferredHeight = 40;

            var slider = go.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.8f;
            return slider;
        }

        static void Assign(SerializedObject so, string field, Object value)
        {
            var property = so.FindProperty(field);
            if (property == null)
            {
                Debug.LogError("MainMenuSceneBuilder: field '" + field + "' not found on MainMenuController.");
                return;
            }
            property.objectReferenceValue = value;
        }

        static void AddToBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene> { new EditorBuildSettingsScene(ScenePath, true) };
            foreach (var existing in EditorBuildSettings.scenes)
                if (existing.path != ScenePath) scenes.Add(existing);
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
