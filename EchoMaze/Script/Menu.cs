// Menu.cs (replace your current Menu script with this)
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.UI;
using Stride.UI.Controls;
using System;

namespace EchoMaze.Script
{
    public class Menu : StartupScript
    {
        private Entity uiEntity;
        private Entity uiTutorial;
        private UIComponent TutorialComponent;
        private UIComponent uiComponent;

        // keep references so we can unhook safely
        private Button menuStartButton;
        private Button tutorialPlayButton;

        // Use distinct names for buttons to avoid confusion
        private const string MENU_START_NAME = "Start";          // button inside Menu UI component
        private const string TUTORIAL_PLAY_NAME = "Play";       // button inside Tutorial UI component (rename in UI to Play if you can)

        public override void Start()
        {
            // Find Menu UI entity and component
            uiEntity = Entity.FindChild("Menu");
            if (uiEntity != null)
                uiComponent = uiEntity.Get<UIComponent>();

            // Find Tutorial UI entity and component
            uiTutorial = Entity.FindChild("Tutorial");
            if (uiTutorial != null)
                TutorialComponent = uiTutorial.Get<UIComponent>();

            // Ensure tutorial is hidden at start
            if (TutorialComponent != null)
                TutorialComponent.Enabled = false;

            // Wire menu UI
            LoadMenu();
        }

        public override void Cancel()
        {
            // Unhook to avoid duplicate subscriptions when scene reloads/unloads
            if (menuStartButton != null)
            {
                menuStartButton.Click -= OnMenuStartClicked;
                menuStartButton = null;
            }
            if (tutorialPlayButton != null)
            {
                tutorialPlayButton.Click -= OnTutorialPlayClicked;
                tutorialPlayButton = null;
            }
        }

        // called when pressing Start in Menu
        private void OnMenuStartClicked(object sender, Stride.UI.Events.RoutedEventArgs e)
        {
            ShowTutorial();
        }

        // called when pressing Play in Tutorial (loads level)
        private void OnTutorialPlayClicked(object sender, Stride.UI.Events.RoutedEventArgs e)
        {
            // Load level and lock the mouse
            var levelScene = Content.Load<Scene>("MainScene");
            SceneSystem.SceneInstance.RootScene = null;
            SceneSystem.SceneInstance.RootScene = levelScene;
            Game.IsMouseVisible = false;
            Input.LockMousePosition();
        }

        private void LoadMenu()
        {
            // Make sure we have the menu UI component
            if (uiComponent == null) return;

            // Ensure tutorial is disabled (extra safety)
            if (TutorialComponent != null) TutorialComponent.Enabled = false;

            // Enable menu UI
            uiComponent.Enabled = true;

            // Unhook previous menu button if any
            if (menuStartButton != null)
            {
                menuStartButton.Click -= OnMenuStartClicked;
                menuStartButton = null;
            }

            // Find and wire Start button inside this menu UI ONLY (search inside menu UI component)
            var startBtn = uiComponent.Page.RootElement.FindVisualChildOfType<Button>(MENU_START_NAME);
            if (startBtn != null)
            {
                startBtn.Click -= OnMenuStartClicked; // safe unsubscribe
                startBtn.Click += OnMenuStartClicked;
                menuStartButton = startBtn;
            }

            // Make sure tutorial play button is unhooked (we will re-hook when tutorial opens)
            if (tutorialPlayButton != null)
            {
                tutorialPlayButton.Click -= OnTutorialPlayClicked;
                tutorialPlayButton = null;
            }

            // Unlock mouse and show cursor for menu
            Input.UnlockMousePosition();
            Game.IsMouseVisible = true;
        }

        private void ShowTutorial()
        {
            if (TutorialComponent == null) return;

            // Hide menu
            if (uiComponent != null) uiComponent.Enabled = false;

            // Enable tutorial UI
            TutorialComponent.Enabled = true;

            // Unhook old tutorial button if any
            if (tutorialPlayButton != null)
            {
                tutorialPlayButton.Click -= OnTutorialPlayClicked;
                tutorialPlayButton = null;
            }

            // Find the Play button inside the Tutorial UI component (search inside tutorial UI)
            var playBtn = TutorialComponent.Page.RootElement.FindVisualChildOfType<Button>(TUTORIAL_PLAY_NAME);
            if (playBtn != null)
            {
                playBtn.Click -= OnTutorialPlayClicked; // safe unsubscribe then subscribe
                playBtn.Click += OnTutorialPlayClicked;
                tutorialPlayButton = playBtn;
            }
        }
    }
}
