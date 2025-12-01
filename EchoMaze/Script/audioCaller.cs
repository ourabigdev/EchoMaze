using System;
using Stride.Audio;
using Stride.Engine;

namespace EchoMaze.Script;

public class audioCaller : SyncScript
{
    public float LightDuration = 0.4f;   // How long the light stays ON
    public float Cooldown = 1.2f;        // Time the light stays OFF
    private string AudioName = "light";   // Name of sound slot in AudioEmitter

    private Entity lightEntity;
    private LightComponent lightComp;

    private AudioEmitterComponent emitter;
    private AudioEmitterSoundController controller;
    private Entity audioEntity;

    private float timer = 0f;
    private bool isLightOn = false;

    public override void Start()
    {
        // ligth setup
        lightEntity = Entity.FindChild("audioLight");
        if (lightEntity != null)
            lightComp = lightEntity.Get<LightComponent>();
        else
            Console.WriteLine("audioCaller WARNING: Entity 'audioLight' not found.");

        if (lightComp != null)
            lightComp.Enabled = false; 

        timer = Cooldown; 

        audioEntity = Entity.FindChild("audioCallerSound");


        emitter = audioEntity.Get<AudioEmitterComponent>();
        if (emitter == null)
        {
            Console.WriteLine("audioCaller ERROR: No AudioEmitterComponent on entity.");
            return;
        }

        controller = emitter[AudioName];
        if (controller == null)
        {
            Console.WriteLine($"audioCaller ERROR: No audio entry named '{AudioName}'.");
            return;
        }

        controller.IsLooping = true;   
        controller.Pitch = 1.0f;       
        controller.Volume = 4f;      
        controller.Play();            
    }

    public override void Update()
    {
        float dt = (float)Game.UpdateTime.Elapsed.TotalSeconds;
        timer -= dt;

        if (!isLightOn)
        {
            if (timer <= 0f)
            {
                if (lightComp != null)
                    lightComp.Enabled = true;

                isLightOn = true;
                timer = LightDuration;
            }

            return;
        }

        if (isLightOn)
        {
            if (timer <= 0f)
            {
                if (lightComp != null)
                    lightComp.Enabled = false;

                isLightOn = false;
                timer = Cooldown;
            }
        }
    }
}
