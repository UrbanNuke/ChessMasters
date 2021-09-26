using UnityEngine.UIElements;
using System.Collections;
using Misc;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIService : MonoBehaviour
    {
        [SerializeField]
        private GameObject popupCheckText;

        private Animation _popupCheckAnimation;

        [SerializeField]
        private GameObject popupCheckmateText;

        [SerializeField]
        private UIDocument mainMenu;

        [SerializeField]
        private UIDocument gameUI;

        private VisualElement _firstScreen;
        private VisualElement _playScreen;
        private VisualElement _settingsScreen;

        private Button _playButton;
        private Button _playPlayerVsPlayerButton;
        private Button _playBackButton;
        private Button _settingsButton;
        private Button _settingsBackButton;
        private Button _quitButton;

        private Button _changeCameraButton;
        private EventDeliveryService _eventDeliveryService;

        [Inject]
        private void Construct(EventDeliveryService eventDeliveryService)
        {
            _eventDeliveryService = eventDeliveryService;
        }

        private void Awake()
        {
            SetMainMenuElements();
            _eventDeliveryService.OnPlayerCheck += ShowCheckText;
            _eventDeliveryService.OnPlayerCheckmate += ShowCheckmateText;
            _eventDeliveryService.OnUICameraButtonDisabled += SetCameraButtonState;
            _popupCheckAnimation = popupCheckText.GetComponent<Animation>();
        }

        private void ShowCheckText()
        {
            popupCheckText.SetActive(true);
            StartCoroutine(CheckTextCoroutine());
        }

        private void SetMainMenuElements()
        {
            VisualElement mainMenuRoot = mainMenu.rootVisualElement;

            _firstScreen = mainMenuRoot.Q<VisualElement>("main-menu__first-screen");
            _playScreen = mainMenuRoot.Q<VisualElement>("main-menu__play-screen");
            _settingsScreen = mainMenuRoot.Q<VisualElement>("main-menu__settings-screen");

            _playButton = mainMenuRoot.Q<Button>("main-menu__play-button");
            _playPlayerVsPlayerButton = mainMenuRoot.Q<Button>("main-menu__player-vs-player-button");
            _playBackButton = mainMenuRoot.Q<Button>("main-menu__play-back-button");
            _settingsButton = mainMenuRoot.Q<Button>("main-menu__settings-button");
            _settingsBackButton = mainMenuRoot.Q<Button>("main-menu__settings-back-button");
            _quitButton = mainMenuRoot.Q<Button>("main-menu__quit-button");

            _playPlayerVsPlayerButton.clickable.clickedWithEventInfo += StartGame;
            _playButton.clickable.clickedWithEventInfo += SwitchMenu;
            _playBackButton.clickable.clickedWithEventInfo += SwitchMenu;
            _settingsButton.clickable.clickedWithEventInfo += SwitchMenu;
            _settingsBackButton.clickable.clickedWithEventInfo += SwitchMenu;
            _quitButton.clicked += Quit;
        }

        private IEnumerator CheckTextCoroutine()
        {
            yield return new WaitUntil(() => !_popupCheckAnimation.isPlaying);
            popupCheckText.SetActive(false);
        }

        private void ShowCheckmateText()
        {
            popupCheckmateText.SetActive(true);
        }

        private void SwitchMenu(EventBase ev)
        {
            if (ev.target == _playButton)
            {
                _firstScreen.style.display = DisplayStyle.None;
                _playScreen.style.display = DisplayStyle.Flex;
            }

            if (ev.target == _settingsButton)
            {
                _firstScreen.style.display = DisplayStyle.None;
                _settingsScreen.style.display = DisplayStyle.Flex;
            }

            if (ev.target == _playBackButton || ev.target == _settingsBackButton)
            {
                _playScreen.style.display = DisplayStyle.None;
                _settingsScreen.style.display = DisplayStyle.None;
                _firstScreen.style.display = DisplayStyle.Flex;
            }
        }

        private void StartGame(EventBase ev)
        {
            mainMenu.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
            _changeCameraButton = gameUI.rootVisualElement.Q<Button>("game__camera-button");
            _changeCameraButton.clicked += _eventDeliveryService.UICameraButtonClicked;

            if (ev.target == _playPlayerVsPlayerButton)
                _eventDeliveryService.UIGameStart(GameMode.PlayerVsPlayer);
        }

        private void SetCameraButtonState(bool state)
        {
            _changeCameraButton.SetEnabled(state);
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}