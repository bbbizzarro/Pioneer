using Godot;
using System;

public class CameraController : Camera2D {

    public override void _Ready() {
    }

    public override void _Process(float delta) {
        float offsetScale = 0.05f;
        var amountX = GetGlobalMousePosition().x * 2f / GetViewportRect().Size.y;
        var amountY = GetGlobalMousePosition().y * 2f / GetViewportRect().Size.y;
        GlobalPosition = new Vector2(amountX * offsetScale * 1920f/ 2f, 
                                     amountY * offsetScale * 1080f / 2f);
        if (Input.IsActionJustPressed("Select")) {
            GD.Print(GlobalPosition);
        }
    }
}
