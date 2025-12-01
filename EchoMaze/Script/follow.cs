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
    private AudioEmitterComponent emitter;
    private AudioEmitterSoundController controller;

    public override void Start()
    {
        emitter = Entity.Get<AudioEmitterComponent>();
        controller = emitter["follow"];

        controller.IsLooping = true;   
        controller.Pitch = 1.0f;        
        controller.Volume = 0.2f;     
        controller.Play();            
    }
}
