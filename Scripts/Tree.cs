using Godot;
using System;

public class Tree : Node2D {

    Sprite sprite;

    public override void _Ready() {
        sprite = (Sprite)GetNode("Sprite");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
