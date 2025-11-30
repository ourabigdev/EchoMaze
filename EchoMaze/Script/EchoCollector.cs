using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.BepuPhysics;
using Stride.BepuPhysics.Definitions.Contacts;
using Stride.Engine.Events;

namespace EchoMaze.Script;

public class EchoCollector : StartupScript, IContactHandler
{
    public bool NoContactResponse => true;


    // Declared public member fields and properties will show in the game studio
    public override void Start()
    {
        // Initialization of the script.
        Console.WriteLine("Level script started.");
    }
    public override void Cancel()
    {
        base.Cancel();
        Console.WriteLine("Level script shutdown");
    }

    void IContactHandler.OnStartedTouching<TManifold>(Contacts<TManifold> contacts)
    {
        var menuScene = Content.Load<Scene>("MainMenu");
        CharacterMovement.StopSound();
        SceneSystem.SceneInstance.RootScene = null;
        SceneSystem.SceneInstance.RootScene = menuScene;
    }

    void IContactHandler.OnStoppedTouching<TManifold>(Contacts<TManifold> contacts)
    {
        //DebugText.Print("Echo Exited!", new Int2(100, 100));
    }
}
