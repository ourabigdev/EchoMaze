using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.BepuPhysics;
using Stride.Audio;
using Stride.Particles.Components;

namespace EchoMaze.Script;

public class CharacterMovement : SyncScript
{
    // Declared public member fields and properties will show in the game studio
    private Vector3 MovemetMultiplayer = new Vector3(3, 0, 4);
    private CharacterComponent character;

    private AnimationComponent anim;
    private Entity modelEntity;

    private Entity BatLight;
    private Sound batSoundEffect;
    private static SoundInstance soundEffect;
    private Sound CaveSoundEffect;
    private static SoundInstance caveSound;
    private Sound WingsSoundEffect;
    private static SoundInstance wingsSound;

    private LightComponent lightBat;
    private float LightCooldown = 4f;
    private float cooldownTimer = 0f;
    private float LightDuration = 2f;
    private float timer = 0f;
    private bool lightActive = false;

    private static bool hasWon = false;

    public ParticleSystemComponent particle;

    protected void PlayAnimation(string name)
    {
        if(!anim.IsPlaying(name))
        {
            anim.Play(name);
        }
    }

    public static void StopSound()
    {
        soundEffect.Stop();
        caveSound.Stop();
        wingsSound.Stop();
    }

    public static void TriggerWin()
    {
        hasWon = true;
    }

    public override void Start()
    {
        // Initialization of the script.
        character = Entity.Get<CharacterComponent>();
        modelEntity = Entity.FindChild("bat");
        anim = modelEntity.Get<AnimationComponent>();

        //light
        BatLight = Entity.FindChild("BatLight");
        lightBat = BatLight.Get<LightComponent>();

        //sounds
        //bat
        batSoundEffect = Content.Load<Sound>("sound and music/batPower");
        soundEffect = batSoundEffect.CreateInstance();
        soundEffect.IsLooping = false;
        soundEffect.Volume = 0.07f;
        //cave
        CaveSoundEffect = Content.Load<Sound>("sound and music/ambient-sound-1-17076");
        caveSound = CaveSoundEffect.CreateInstance();
        caveSound.IsLooping = true;
        caveSound.Volume = 0.025f;
        //wings
        WingsSoundEffect = Content.Load<Sound>("sound and music/wings");
        wingsSound = WingsSoundEffect.CreateInstance();
        wingsSound.IsLooping = true;
        wingsSound.Volume = .08f;


        //implement
        caveSound.Play();
        wingsSound.Play();
        lightBat.Enabled = false;
        PlayAnimation("flap");
        particle.ParticleSystem.Stop();
        particle.ParticleSystem.ResetSimulation();
        hasWon = false;

        Entity.Transform.Position = new Vector3(0, 0, 0);
    }

    public override void Update()
    {
        if (hasWon)
        {
            var menuScene = Content.Load<Scene>("MainMenu");
            StopSound();
            SceneSystem.SceneInstance.RootScene = null;
            SceneSystem.SceneInstance.RootScene = menuScene;
            Entity.Transform.Position = new Vector3(0, 0, 0);
            Timer.Instance.ResetTimer();
        }

        var velocity = new Vector3();

        bool forward = Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.Up) || Input.IsKeyDown(Keys.Z);
        bool backward = Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down);
        bool left = Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.Left) || Input.IsKeyDown(Keys.Q);
        bool right = Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right);
        bool light = Input.IsMouseButtonPressed(MouseButton.Left);

        if(cooldownTimer > 0f)
        {
            cooldownTimer -= (float)Game.UpdateTime.Elapsed.TotalSeconds;
            if(cooldownTimer < 0f)
            {
                cooldownTimer = 0f;
            }
        }

        if (light && cooldownTimer <= 0f)
        {
            particle.ParticleSystem.Stop();
            particle.ParticleSystem.ResetSimulation();
            particle.ParticleSystem.Play();
            lightBat.Enabled = true;
            lightActive = true;
            soundEffect.Play();
            timer = LightDuration;
            cooldownTimer = LightCooldown;
        }
        if (lightActive)
        {
            timer -= (float)Game.UpdateTime.Elapsed.TotalSeconds;
            if (timer <= 0f)
            {
                lightBat.Enabled = false;
                lightActive = false;
            }
        }

        if (forward)
            velocity.Z++;
        if(backward)
            velocity.Z--;
        if(left)
            velocity.X++;
        if(right)
            velocity.X--;

        velocity.Normalize();
        velocity *= MovemetMultiplayer;
        velocity = Vector3.Transform(velocity, Entity.Transform.Rotation);
        character.Velocity = velocity;
    }
}
