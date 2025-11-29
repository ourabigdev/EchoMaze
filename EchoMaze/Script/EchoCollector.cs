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
    public static EventKey winEvent = new EventKey("Global", "Winning");
    public static EventKey<string> winEventData = new EventKey<string>("Global", "Winning Data");


    // Declared public member fields and properties will show in the game studio
    public override void Start()
    {
        // Initialization of the script.
    }

    void IContactHandler.OnStartedTouching<TManifold>(Contacts<TManifold> contacts)
    {
        winEvent.Broadcast();
        //DebugText.Print("Entered", new Int2(100, 100));
    }

    void IContactHandler.OnStoppedTouching<TManifold>(Contacts<TManifold> contacts)
    {
        //DebugText.Print("Echo Exited!", new Int2(100, 100));
    }
}
