using Godot;
using System;

public class LocationButton : TextureButton {
    public override void _Ready() {
        Connect("mouse_entered", this, nameof(HandleMouseEnter));
        Connect("mouse_exited", this, nameof(HandleMouseExit));
        Connect("button_down", this, nameof(HandleButtonDown));
        Connect("button_up", this, nameof(HandleButtonUp));
        Connect("hide", this, nameof(HandleOnHide));
    }

    public void HandleMouseEnter() {
        RectScale = new Vector2(1.2f, 1.2f);
    }

    public void HandleMouseExit() {
        RectScale = Vector2.One;
    }

    public void HandleButtonDown() {
        RectScale = new Vector2(1.3f, 1.3f);
    }
    public void HandleButtonUp() {
        RectScale = new Vector2(1.2f, 1.2f);
    }

    public void HandleOnHide() {
        RectScale = Vector2.One;
    }
}
