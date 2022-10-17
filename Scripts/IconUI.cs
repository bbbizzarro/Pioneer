using Godot;
using System;

public class IconUI : Control {

    Control _imageContainer;

    public override void _Ready() {
        base._Ready();
        GetReferences();
    }

    public void ScaleIcon(float amount) {
        _imageContainer.RectPosition = (1f - amount) * 0.5f * _imageContainer.RectSize;
        _imageContainer.RectScale = new Vector2(amount, amount);
    }
    
    private void GetReferences() {
        _imageContainer = (Control)GetNode("CenterContainer");
    }
}
