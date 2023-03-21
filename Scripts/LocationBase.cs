using Godot;
using System;

public class LocationBase : Prop {

    public override void Reveal() {
        animationPlayer.Play("BaseReveal");
        Show();
    }
}
