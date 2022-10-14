using Godot;
using System;
using System.Collections.Generic;

public class SpriteDB : Node {
    [Export] private string DBPath;
    [Export] private Texture DefaultSprite;
    public static SpriteDB Instance;
    Directory _textureDirectory;

    public override void _Ready() {
        if (Instance != null) QueueFree();
        else {
            Instance = this;
            _textureDirectory = new Directory();
            _textureDirectory.Open(DBPath);
        }
    }

    public Texture GetSprite(string name) {
        if (_textureDirectory.FileExists(name + ".tres")) {
            return (Texture)ResourceLoader.Load(DBPath + name + ".tres");
        }
        else {
            return DefaultSprite;
        }
    }

}
