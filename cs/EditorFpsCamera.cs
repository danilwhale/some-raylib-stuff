// works like in https://learnopengl.com/Getting-started/Camera tutorial
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace SomeRaylibStuff;

public class EditorFpsCamera
{
    public Vector3 Rotation;
    public Camera3D Camera; // YOU NEED TO ASSIGN CAMERA YOURSELF
    public float Speed;
    public float MouseSensitivity;

    public EditorFpsCamera(float speed, float mouseSensitivity)
    {
        Camera = new Camera3D();
        Speed = speed;
        MouseSensitivity = mouseSensitivity;
    }
    
    public void Update()
    {
        Vector3 axis = Vector3.Zero;
        
        // handle input, very basic
        if (IsKeyDown(KeyboardKey.KEY_W)) axis.Z = 1;
        if (IsKeyDown(KeyboardKey.KEY_S)) axis.Z = -1;
        if (IsKeyDown(KeyboardKey.KEY_A)) axis.X = -1;
        if (IsKeyDown(KeyboardKey.KEY_D)) axis.X = 1;
        if (IsKeyDown(KeyboardKey.KEY_E)) axis.Y = 1;
        if (IsKeyDown(KeyboardKey.KEY_Q)) axis.Y = -1;

        // multiply direction (axis) by speed and delta time (then speed wont be different on different framerates)
        axis *= Speed * GetFrameTime();
        
        Vector2 mouseAxis = -GetMouseDelta();
        mouseAxis *= MouseSensitivity;
        
        // check if mouse axis is non-zero and save some cpu cycles if its zero
        if (mouseAxis != Vector2.Zero)
        {
            // rotate camera using mouse
            Rotation += new Vector3(mouseAxis.Y, -mouseAxis.X, 0);
            // clamp rotation X axis (up, down) between -89 and 89
            Rotation.X = Math.Clamp(Rotation.X, -89, 89);

            // now we need to update vectors to match camera rotation
            UpdateVectors();
        }
        
        // and now we can move camera itself
        CameraMoveForward(ref Camera, axis.Z, false);
        CameraMoveRight(ref Camera, axis.X, false);
        CameraMoveUp(ref Camera, axis.Y);
    }

    // this method heavily uses https://learnopengl.com/Getting-started/Camera tutorial code
    private void UpdateVectors()
    {
        // calculate rotation sin and cos (convert to radians using 'x * DEG2RAD'!!!)
        var sincosX = MathF.SinCos(Rotation.X * DEG2RAD);
        var sincosY = MathF.SinCos(Rotation.Y * DEG2RAD);

        // some math from tutorial
        var forward = new Vector3(
            sincosY.Cos * sincosX.Cos,
            sincosX.Sin,
            sincosY.Sin * sincosX.Cos
        );
        forward = Vector3.Normalize(forward);

        // GetCameraRight will have rotated Z axis, use own impl
        var right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));
        
        // update raylib's camera vectors
        Camera.Up = Vector3.Normalize(Vector3.Cross(right, forward));
        Camera.Target = Camera.Position + forward;
    }
}