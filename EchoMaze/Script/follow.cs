using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Audio;

namespace EchoMaze.Script;

public class follow : StartupScript
{
    // Declared public member fields and properties will show in the game studio
    private AudioEmitterComponent emitter;
    private AudioEmitterSoundController controller;

    public override void Start()
    {
        // Initialization of the script.
        emitter = Entity.Get<AudioEmitterComponent>();
        controller = emitter["follow"];

        controller.IsLooping = true;   // loop forever
        controller.Pitch = 1.0f;        // normal pitch
        controller.Volume = 0.2f;      // adjust as needed
        controller.Play();            // plays once at game start
    }
}
