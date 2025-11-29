using Stride.Core.Mathematics;
using Stride.Engine;
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

public class SceneLoader : StartupScript
{
    private Scene LevelScene;
    private Scene Load;
    private Scene MenuScene;
    private Entity uiEntity;
    private UIComponent uiComponent;

    public override void Start()
    {
        Load = Entity.Scene;
        LevelScene = Content.Load<Scene>("MainScene");
        MenuScene = Content.Load<Scene>("MainMenu");

        uiEntity = Entity.FindChild("Menu");
        uiComponent = uiEntity.Get<UIComponent>();
        var page = uiComponent.Page;
        var button = page.RootElement.FindVisualChildOfType<Button>("Start");

        Load.Children.Add(MenuScene);
        button.Click += ButtonClickedEvent;
    }

    private void ButtonClickedEvent(object sender, Stride.UI.Events.RoutedEventArgs e)
    {
        MenuScene.Parent = null;
        uiComponent.Enabled = false;
        Load.Children.Add(LevelScene);

    }
}
