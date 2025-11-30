using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;
using Stride.UI;
using Stride.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EchoMaze.Script;

public class SceneLoader : SyncScript
{
    private Scene Load;
    private Entity uiEntity;
    private Entity uiTutorial;
    private UIComponent TutorialComponent;
    private UIComponent uiComponent;

    // Persist receivers as fields so they keep registered state across frames.
    private EventReceiver winReceiver;
    private EventReceiver<string> winReceiverData;

    public override void Start()
    {
        Load = Entity.Scene;
        winReceiver = new EventReceiver(EchoCollector.winEvent);
        winReceiverData = new EventReceiver<string>(EchoCollector.winEventData);

        uiTutorial = Entity.FindChild("Tutorial");
        TutorialComponent = uiTutorial.Get<UIComponent>();


        LoadMenu();
    }

    public override void Update()
    {
        // Reuse the persistent receivers to poll for events.
        if (winReceiver.TryReceive())
        {
            LoadMenu();
        }

    }

    private void ButtonClickedEvent(object sender, Stride.UI.Events.RoutedEventArgs e)
    {
        LoadTutorial();
    }

    private void TutorialButtonClickedEvent(object sender, Stride.UI.Events.RoutedEventArgs e)
    {
        LoadLevel();
    }

    private void LoadMenu()
    {
        // Detach and clear old scenes
        foreach (var child in Load.Children.ToList())
            child.Parent = null;
        Load.Children.Clear();

        TutorialComponent.Enabled = false;

        // Load new menu
        var menuScene = Content.Load<Scene>("MainMenu");
        Load.Children.Add(menuScene);

        // Grab UI from new menu
        uiEntity = Entity.FindChild("Menu");
        if (uiEntity == null) return;

        uiComponent = uiEntity.Get<UIComponent>();
        if (uiComponent == null) return;

        // Grab button and assign event
        var button = uiComponent.Page.RootElement.FindVisualChildOfType<Button>("Start");
        if (button != null)
        {
            button.Click -= ButtonClickedEvent; // prevent duplicate
            button.Click += ButtonClickedEvent;
        }

        uiComponent.Enabled = true;

        // Unlock mouse for menu
        Input.UnlockMousePosition();
    }
    private void LoadTutorial()
    {
        

        TutorialComponent.Enabled = true;

        var button = TutorialComponent.Page.RootElement.FindVisualChildOfType<Button>("Start");
        if (button != null)
        {
            button.Click -= TutorialButtonClickedEvent;
            button.Click += TutorialButtonClickedEvent;
        }

        uiComponent.Enabled = false;
    }

    private void LoadLevel()
    {
        foreach (var child in Load.Children.ToList())
            child.Parent = null;
        Load.Children.Clear();

        TutorialComponent.Enabled = false;

        var levelScene = Content.Load<Scene>("MainScene");
        Load.Children.Add(levelScene);
    }

}
