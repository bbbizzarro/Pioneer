using Godot;
using System;
using System.Collections.Generic;

public class SpriteDB : Node {
    [Export] private string DBPath;
    [Export] private Texture DefaultSprite;
    public static SpriteDB Instance;
    Directory _textureDirectory;

    static Dictionary<string, List<string>> categories;

    public override void _Ready() {
        if (Instance != null) QueueFree();
        else {
            Instance = this;
            _textureDirectory = new Directory();
            _textureDirectory.Open(DBPath);
            categories = new Dictionary<string, List<string>>() {
                {"Large", new List<string>(){"Tree"}},
                {"Small", new List<string>(){"Grass1", "Grass2"}}
            };
        }
    }

    public Texture GetRandomCategory(string category) {
        if (!categories.ContainsKey(category)) return null;
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        if (rng.Randf() < 0.2f) return null;
        else return GetSprite(categories[category][rng.RandiRange(0, categories[category].Count - 1)]);
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
