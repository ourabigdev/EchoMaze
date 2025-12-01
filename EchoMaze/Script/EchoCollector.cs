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

    public override void Start()
    {
        Console.WriteLine("Level script started.");
    }
    public override void Cancel()
    {
        base.Cancel();
        Console.WriteLine("Level script shutdown");
    }

    void IContactHandler.OnStartedTouching<TManifold>(Contacts<TManifold> contacts)
    {
        CharacterMovement.TriggerWin();
    }

    void IContactHandler.OnStoppedTouching<TManifold>(Contacts<TManifold> contacts)
    {
        //DebugText.Print("Echo Exited!", new Int2(100, 100));
    }
}
