using Godot;
using System;

public class Pivot : Sprite {
    public override void _Ready() {
        Offset = new Vector2(0, -Texture.GetSize().y/2f);
    }
}
