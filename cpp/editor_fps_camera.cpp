// works like in https://learnopengl.com/Getting-started/Camera tutorial
#include "raylib.h"
#include "raymath.h"

class EditorFpsCamera {
public:
    EditorFpsCamera(float speed, float mouseSensitivity);
    void Update();

    float Speed;
    float MouseSensitivity;
    Camera3D Camera{}; // YOU NEED TO ASSIGN CAMERA YOURSELF
    Vector3 Rotation{ Vector3(0.0f, -90.0f, 0.0f) };
private:
    void UpdateVectors();
};

EditorFpsCamera::EditorFpsCamera(float speed, float mouseSensitivity)
    : Speed(speed), MouseSensitivity(mouseSensitivity)
{

}

void EditorFpsCamera::Update()
{
    Vector3 axis = Vector3Zero();

    // handle input, very basic
    if (IsKeyDown(KeyboardKey.KEY_W)) axis.z = 1;
    if (IsKeyDown(KeyboardKey.KEY_S)) axis.z = -1;
    if (IsKeyDown(KeyboardKey.KEY_A)) axis.x = -1;
    if (IsKeyDown(KeyboardKey.KEY_D)) axis.x = 1;
    if (IsKeyDown(KeyboardKey.KEY_E)) axis.y = 1;
    if (IsKeyDown(KeyboardKey.KEY_Q)) axis.y = -1;

    // multiply direction (axis) by speed and delta time (then speed wont be different on different framerates)
    axis = Vector3Scale(axis, Speed * GetFrameTime());

    Vector2 mouseAxis = Vector2Negate(GetMouseDelta());
    mouseAxis = Vector2Scale(mouseAxis, MouseSensitivity);

    // check if mouse axis is non-zero and save some cpu cycles if its zero
    if (mouseAxis.x != 0 && mouseAxis.y != 0)
    {
        // rotate camera using mouse
        Rotation.x += mouseAxis.y;
        Rotation.y -= mouseAxis.x;
        
        // clamp rotation X axis (up, down) between -89 and 89
        Rotation.x = Clamp(Rotation.x, -89, 89);

        // now we need to update vectors to match camera rotation
        UpdateVectors();
    }

    // and now we can move camera itself
    CameraMoveForward(&Camera, axis.z, false);
    CameraMoveRight(&Camera, axis.x, false);
    CameraMoveUp(&Camera, axis.y);
}

// this method heavily uses https://learnopengl.com/Getting-started/Camera tutorial code
void EditorFpsCamera::UpdateVectors()
{
    // calculate rotation sin and cos (convert to radians using 'x * DEG2RAD'!!!)
    float sinX = sin(Rotation.X * DEG2RAD);
    float cosX = cos(Rotation.X * DEG2RAD);
    float sinY = sin(Rotation.Y * DEG2RAD);
    float cosY = cos(Rotation.Y * DEG2RAD);

    // some math from tutorial
    Vector3 forward;
    forward.x = cosY * cosX;
    forward.y = sinX;
    forward.z = sinY * cosX;
    forward = Vector3Normalize(forward);

    // GetCameraRight will have rotated Z axis, use own impl
    Vector3 right = Vector3Normalize(Vector3CrossProduct(forward, { 0.0f, 1.0f, 0.0f }));

    // update raylib's camera vectors
    Camera.up = Vector3Normalize(Vector3CrossProduct(right, forward))
    Camera.target = Camera.position + forward;
}