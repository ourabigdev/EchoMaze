using Stride.Audio;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.UI;
using Stride.UI.Controls;
using System;
using System.Diagnostics;

namespace EchoMaze.Script
{
    public class Menu : StartupScript
    {
        private Entity uiEntity;
        private Entity uiTutorial;
        private UIComponent TutorialComponent;
        private UIComponent uiComponent;

        private Button menuStartButton;
        private Button tutorialPlayButton;

        private Sound Music;
        private static SoundInstance musicSound;

        private const string MENU_START_NAME = "Start"; 
        private const string TUTORIAL_PLAY_NAME = "Play";       

        public override void Start()
        {
            // Find Menu UI entity and component
            uiEntity = Entity.FindChild("Menu");
            uiComponent = uiEntity?.Get<UIComponent>();

            // Re-find Tutorial UI entity and component each time
            uiTutorial = Entity.FindChild("Tutorial");
            TutorialComponent = uiTutorial?.Get<UIComponent>();

            // Ensure tutorial is hidden at start
            if (TutorialComponent != null)
                TutorialComponent.Enabled = false;

            // Always re-hook buttons
            LoadMenu();

            Console.WriteLine("Menu script started.");

            Music = Content.Load<Sound>("sound and music/music");
            musicSound = Music.CreateInstance();
            musicSound.IsLooping = true;
            musicSound.Volume = .2f;
            musicSound.Play();
        }

        public override void Cancel()
        {
            base.Cancel();

            if (menuStartButton != null)
                menuStartButton.Click -= OnMenuStartClicked;

            if (tutorialPlayButton != null)
                tutorialPlayButton.Click -= OnTutorialPlayClicked;

            Console.WriteLine("Menu script shutdown");
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
            musicSound.Stop();
        }

        private void LoadMenu()
        {
            if (uiComponent == null) return;

            // Enable menu
            uiComponent.Enabled = true;

            // Disable tutorial
            if (TutorialComponent != null)
                TutorialComponent.Enabled = false;

            // Re-find Start button
            var startBtn = uiComponent.Page.RootElement.FindVisualChildOfType<Button>(MENU_START_NAME);
            if (startBtn != null)
            {
                startBtn.Click += OnMenuStartClicked;
                menuStartButton = startBtn;
            }

            // Clear tutorial button reference (we will re-hook when showing tutorial)
            tutorialPlayButton = null;

            // Unlock mouse
            Input.UnlockMousePosition();
            Game.IsMouseVisible = true;
        }

        private void ShowTutorial()
        {
            if (TutorialComponent == null) return;

            // Hide menu
            if (uiComponent != null) uiComponent.Enabled = false;

            // Enable tutorial
            TutorialComponent.Enabled = true;

            // Re-find Play button
            var playBtn = TutorialComponent.Page.RootElement.FindVisualChildOfType<Button>(TUTORIAL_PLAY_NAME);
            if (playBtn != null)
            {
                playBtn.Click += OnTutorialPlayClicked;
                tutorialPlayButton = playBtn;
            }
        }
    }
}
