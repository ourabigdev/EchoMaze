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

namespace EchoMaze.Script;

public class CharacterMovement : SyncScript
{
    // Declared public member fields and properties will show in the game studio
    public Vector3 MovemetMultiplayer = new Vector3(3, 0, 4);

    private CharacterComponent character;
    private AnimationComponent anim;
    private LightComponent lightBat;
    private Entity modelEntity;
    private Entity BatLight;
    private float LightDuration = 0.7f;
    private float timer = 0f;
    private bool lightActive = false;
    private Sound batSoundEffect;
    private SoundInstance soundEffect;

    private float LightCooldown = 4f;
    private float cooldownTimer = 0f;

    protected void PlayAnimation(string name)
    {
        if(!anim.IsPlaying(name))
        {
            anim.Play(name);
        }
    }

    public override void Start()
    {
        // Initialization of the script.
        character = Entity.Get<CharacterComponent>();
        modelEntity = Entity.FindChild("bat");
        BatLight = Entity.FindChild("BatLight");
        anim = modelEntity.Get<AnimationComponent>();
        lightBat = BatLight.Get<LightComponent>();
        batSoundEffect = Content.Load<Sound>("sound and music/batPower");
        soundEffect = batSoundEffect.CreateInstance();
        soundEffect.IsLooping = false;
        soundEffect.Volume = 0.15f;
        lightBat.Enabled = false;
        PlayAnimation("flap");
    }

    public override void Update()
    {
        // Do stuff every new frame
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
