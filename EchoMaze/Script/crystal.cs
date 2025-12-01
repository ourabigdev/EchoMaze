using Stride.Audio;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoMaze.Script;

public class crystal : StartupScript
{
    // Declared public member fields and properties will show in the game studio
    private AudioEmitterComponent emitter;
    private AudioEmitterSoundController controller;

    public override void Start()
    {
        // Initialization of the script.
        emitter = Entity.Get<AudioEmitterComponent>();
        controller = emitter["crystal"];

        controller.IsLooping = true;   
        controller.Pitch = 1.0f;        
        controller.Volume = 0.2f;      
        controller.Play();            
    }
}
