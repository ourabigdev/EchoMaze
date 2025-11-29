using Stride.BepuPhysics;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using System;

namespace EchoMaze.Script;

public class CameraController : SyncScript
{
    // Declared public member fields and properties will show in the game studio
    public bool InvertY = false;
    public Vector2 MouseSpeed = new Vector2(0.8f, 0.7f);
    public float MaxLookUpAngle = -40;
    public float MaxLookDownAngle = 40;
    public float MinimumCameraDistance = 1.5f;

    private Vector3 CameraOffset = new Vector3(0, 0, -20);

    private Entity cameraPivot;
    private Entity thirdPersonPivot;

    private Vector2 maxCameraAngleRadians;
    private Vector3 camRotation;
    private bool isActive = false;
    private BepuSimulation simulation;
    private CharacterComponent character;

    public override void Start()
    {
        // Initialization of the script.
        Game.IsMouseVisible = false;
        isActive = true;

        thirdPersonPivot = Entity.FindChild("ThirdPersonPivot");
        cameraPivot = Entity.FindChild("Pivot");

        maxCameraAngleRadians = new Vector2(MathUtil.DegreesToRadians(MaxLookUpAngle), MathUtil.DegreesToRadians(MaxLookDownAngle));
        camRotation = Entity.Transform.RotationEulerXYZ;
        Input.MousePosition = new Vector2(0.5f, 0.5f);
        simulation = Entity.GetSimulation();
        character = Entity.Get<CharacterComponent>();
    }

    public override void Update()
    {
        // Do stuff every new frame

        if (Input.IsKeyPressed(Keys.Escape))
        {
            isActive = !isActive;
            Game.IsMouseVisible = !isActive;
            Input.UnlockMousePosition();
        }

        if (isActive)
        {
            Input.LockMousePosition();
            var mouseMovement = Input.MouseDelta * MouseSpeed;

            //update rotation withou mouse movement
            camRotation.Y += -mouseMovement.X;
            camRotation.X += InvertY ? -mouseMovement.Y : mouseMovement.Y;
            camRotation.X = MathUtil.Clamp(camRotation.X, maxCameraAngleRadians.X, maxCameraAngleRadians.Y);

            character.Orientation = Quaternion.RotationY(camRotation.Y);
            cameraPivot.Transform.Rotation = Quaternion.RotationX(camRotation.X);

            // --- Build desired camera world position correctly (relative to cameraPivot) ---
            // CameraOffset is in the pivot's local space (0,0,-10)
            var pivotWorldMatrix = cameraPivot.Transform.WorldMatrix;
            var rayStart = pivotWorldMatrix.TranslationVector;

            // Transform the local offset into world coordinates to get the desired camera world position
            var desiredWorld = Vector3.TransformCoordinate(CameraOffset, pivotWorldMatrix);

            // from pivot to desired camera position (world)
            var camToTarget = desiredWorld - rayStart;
            var distance = camToTarget.Length();

            // safety: avoid zero-length
            if (distance < 0.0001f)
            {
                // fallback: place at min distance behind pivot
                desiredWorld = rayStart + Vector3.TransformCoordinate(new Vector3(0, 0, -MinimumCameraDistance), pivotWorldMatrix);
                camToTarget = desiredWorld - rayStart;
                distance = camToTarget.Length();
            }

            var direction = Vector3.Normalize(camToTarget);

            // --- Move ray start slightly forward to avoid hitting the character's own collider ---
            const float selfHitOffset = 0.2f; // tweak: 0.1-0.4
            var rayStartSafe = rayStart + direction * selfHitOffset;
            var maxDistance = Math.Max(0.0f, distance - selfHitOffset);

            // --- Do BEPU raycast ---
            if (simulation.RayCast(rayStartSafe, direction, maxDistance, out HitInfo hit))
            {
                // Hit point is in world space. Compute camera world position just before the hit.
                var hitDistFromPivot = Vector3.Distance(rayStart, hit.Point);
                var desiredDist = Math.Max(MinimumCameraDistance, hitDistFromPivot - 0.1f);

                // new camera world position along direction (backwards from pivot)
                var newCameraWorld = rayStart + direction * desiredDist;

                // Convert world position into thirdPersonPivot's local space
                // If thirdPersonPivot has a parent, transform by inverse of parent's world matrix.
                var parentWorld = thirdPersonPivot.Transform.Parent?.WorldMatrix ?? Matrix.Identity;
                Matrix.Invert(ref parentWorld, out Matrix parentWorldInverse); // invert parent matrix
                var newLocal = Vector3.TransformCoordinate(newCameraWorld, parentWorldInverse);

                thirdPersonPivot.Transform.Position = newLocal;
                thirdPersonPivot.Transform.UpdateWorldMatrix();
            }
            else
            {
                // No hit — set to default offset (local)
                thirdPersonPivot.Transform.Position = CameraOffset;
                thirdPersonPivot.Transform.UpdateWorldMatrix();
            }


        }
    }
}
