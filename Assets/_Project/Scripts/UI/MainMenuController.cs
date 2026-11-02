using System;
using System.IO;
using Codebound.App;
using Codebound.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Codebound.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] GameObject _titlePanel;
        [SerializeField] GameObject _settingsPanel;
        [SerializeField] GameObject _creditsPanel;
        [SerializeField] GameObject _confirmNewGamePanel;
        [SerializeField] GameObject _confirmQuitPanel;
        [SerializeField] GameObject _nameLookPanel;
        [SerializeField] GameObject _placeholderPanel;

        [Header("Title")]
        [SerializeField] Button _newGameButton;
        [SerializeField] Button _continueButton;
        [SerializeField] Button _settingsButton;
        [SerializeField] Button _creditsButton;
        [SerializeField] Button _quitButton;
        [SerializeField] GameObject _noSaveHint;
        [SerializeField] TMP_Text _noticeText;

        [Header("Settings")]
        [SerializeField] Slider _musicSlider;
        [SerializeField] Slider _sfxSlider;
        [SerializeField] Button _settingsBackButton;

        [Header("Credits")]
        [SerializeField] Button _creditsBackButton;

        [Header("Dialogs")]
        [SerializeField] Button _confirmNewGameYesButton;
        [SerializeField] Button _confirmNewGameNoButton;
        [SerializeField] Button _confirmQuitYesButton;
        [SerializeField] Button _confirmQuitNoButton;

        [Header("Name and Look")]
        [SerializeField] NameLookPanelController _nameLook;

        [Header("Audio")]
        [SerializeField] AudioSource _sfxSource;
        [SerializeField] AudioClip _clickClip;

        [Header("Placeholder")]
        [SerializeField] TMP_Text _placeholderText;
        [SerializeField] Button _placeholderBackButton;

        readonly MenuNavigator _navigator = new MenuNavigator();
        MenuScreen _shown = MenuScreen.Title;
        PlayerSave _save;
        bool _hasSave;

        void Start()
        {
            if (!AllReferencesAssigned()) return;

            _hasSave = GameServices.Save.TryLoad(out _save);

            var settings = GameServices.Settings.Settings;
            _musicSlider.SetValueWithoutNotify(settings.musicVolume);
            _sfxSlider.SetValueWithoutNotify(settings.sfxVolume);

            WireButtons();
            foreach (var button in FindObjectsOfType<Button>(true))
                button.onClick.AddListener(PlayClick);
            _nameLook.Started += OnNameLookStarted;
            _nameLook.Cancelled += BackToTitle;

            RefreshTitle();
            _navigator.ScreenChanged += OnScreenChanged;
            OnScreenChanged(_navigator.Current);
        }

        void OnDestroy()
        {
            _navigator.ScreenChanged -= OnScreenChanged;
            if (_nameLook == null) return;
            _nameLook.Started -= OnNameLookStarted;
            _nameLook.Cancelled -= BackToTitle;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) _navigator.HandleBack();
        }

        void WireButtons()
        {
            _newGameButton.onClick.AddListener(OnNewGame);
            _continueButton.onClick.AddListener(OnContinue);
            _settingsButton.onClick.AddListener(() => _navigator.Show(MenuScreen.Settings));
            _creditsButton.onClick.AddListener(() => _navigator.Show(MenuScreen.Credits));
            _quitButton.onClick.AddListener(() => _navigator.Show(MenuScreen.ConfirmQuit));

            _settingsBackButton.onClick.AddListener(BackToTitle);
            _creditsBackButton.onClick.AddListener(BackToTitle);
            _placeholderBackButton.onClick.AddListener(BackToTitle);
            _confirmNewGameNoButton.onClick.AddListener(BackToTitle);
            _confirmQuitNoButton.onClick.AddListener(BackToTitle);

            _confirmNewGameYesButton.onClick.AddListener(() => _navigator.Show(MenuScreen.NameLook));
            _confirmQuitYesButton.onClick.AddListener(Quit);

            _musicSlider.onValueChanged.AddListener(GameServices.Settings.SetMusicVolume);
            _sfxSlider.onValueChanged.AddListener(GameServices.Settings.SetSfxVolume);
        }

        void PlayClick()
        {
            _sfxSource.volume = GameServices.Settings.Settings.sfxVolume;
            _sfxSource.PlayOneShot(_clickClip);
        }

        void OnNewGame()
        {
            if (MainMenuRules.NewGameNeedsConfirm(_hasSave))
                _navigator.Show(MenuScreen.ConfirmNewGame);
            else
                _navigator.Show(MenuScreen.NameLook);
        }

        void OnContinue()
        {
            ShowPlaceholder($"Welcome back, {_save.playerName}. The Keep Map is coming soon.");
        }

        void OnNameLookStarted(PlayerSave save)
        {
            try
            {
                GameServices.Save.Save(save);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogError($"Could not write the save file: {e.Message}");
                ShowPlaceholder("Your game could not be saved. Please try again.");
                return;
            }

            _save = save;
            _hasSave = true;
            RefreshTitle();
            ShowPlaceholder($"Welcome, {save.playerName}. The prologue is coming soon.");
        }

        void ShowPlaceholder(string message)
        {
            _placeholderText.text = message;
            _navigator.Show(MenuScreen.Placeholder);
        }

        void BackToTitle() => _navigator.Show(MenuScreen.Title);

        void RefreshTitle()
        {
            _continueButton.interactable = MainMenuRules.IsContinueEnabled(_hasSave);
            _noSaveHint.SetActive(!_hasSave);
            _noticeText.text = GameServices.Save.BackedUpCorruptSave
                ? "Your saved game could not be read. A backup copy was kept."
                : string.Empty;
        }

        void OnScreenChanged(MenuScreen screen)
        {
            if (_shown == MenuScreen.Settings && screen != MenuScreen.Settings)
                GameServices.Settings.Save();
            _shown = screen;

            _titlePanel.SetActive(screen == MenuScreen.Title);
            _settingsPanel.SetActive(screen == MenuScreen.Settings);
            _creditsPanel.SetActive(screen == MenuScreen.Credits);
            _confirmNewGamePanel.SetActive(screen == MenuScreen.ConfirmNewGame);
            _confirmQuitPanel.SetActive(screen == MenuScreen.ConfirmQuit);
            _nameLookPanel.SetActive(screen == MenuScreen.NameLook);
            _placeholderPanel.SetActive(screen == MenuScreen.Placeholder);

            SelectFirst(screen);
        }

        void SelectFirst(MenuScreen screen)
        {
            if (EventSystem.current == null) return;

            GameObject first = screen switch
            {
                MenuScreen.Title => _newGameButton.gameObject,
                MenuScreen.Settings => _musicSlider.gameObject,
                MenuScreen.Credits => _creditsBackButton.gameObject,
                MenuScreen.ConfirmNewGame => _confirmNewGameNoButton.gameObject,
                MenuScreen.ConfirmQuit => _confirmQuitNoButton.gameObject,
                MenuScreen.NameLook => _nameLook.InitialFocus,
                _ => _placeholderBackButton.gameObject
            };
            EventSystem.current.SetSelectedGameObject(first);
        }

        void Quit()
        {
#if UNITY_EDITOR
            Debug.Log("Quit requested (ignored in the editor).");
#else
            Application.Quit();
#endif
        }

        bool AllReferencesAssigned()
        {
            var references = new (string name, UnityEngine.Object value)[]
            {
                (nameof(_titlePanel), _titlePanel), (nameof(_settingsPanel), _settingsPanel),
                (nameof(_creditsPanel), _creditsPanel), (nameof(_confirmNewGamePanel), _confirmNewGamePanel),
                (nameof(_confirmQuitPanel), _confirmQuitPanel), (nameof(_nameLookPanel), _nameLookPanel),
                (nameof(_placeholderPanel), _placeholderPanel),
                (nameof(_newGameButton), _newGameButton), (nameof(_continueButton), _continueButton),
                (nameof(_settingsButton), _settingsButton), (nameof(_creditsButton), _creditsButton),
                (nameof(_quitButton), _quitButton), (nameof(_noSaveHint), _noSaveHint),
                (nameof(_noticeText), _noticeText), (nameof(_musicSlider), _musicSlider),
                (nameof(_sfxSlider), _sfxSlider), (nameof(_settingsBackButton), _settingsBackButton),
                (nameof(_creditsBackButton), _creditsBackButton),
                (nameof(_confirmNewGameYesButton), _confirmNewGameYesButton),
                (nameof(_confirmNewGameNoButton), _confirmNewGameNoButton),
                (nameof(_confirmQuitYesButton), _confirmQuitYesButton),
                (nameof(_confirmQuitNoButton), _confirmQuitNoButton),
                (nameof(_nameLook), _nameLook),
                (nameof(_sfxSource), _sfxSource), (nameof(_clickClip), _clickClip),
                (nameof(_placeholderText), _placeholderText),
                (nameof(_placeholderBackButton), _placeholderBackButton)
            };

            var ok = true;
            foreach (var (name, value) in references)
            {
                if (value != null) continue;
                Debug.LogError($"MainMenuController: '{name}' is not assigned in the Inspector.", this);
                ok = false;
            }
            return ok;
        }
    }
}
