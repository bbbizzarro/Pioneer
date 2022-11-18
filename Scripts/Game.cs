using Godot;
using System;
using System.Collections.Generic;

public class Game : Node {

    PlayerController _player;
    UI _ui;
    List<Region> _regions;
    Region _curr;

    public override void _Ready() {
        _player = (PlayerController)GetNode("World/Player");
        _player.Inventory.Add("Axe", 1);
        _ui = (UI)GetNode("UI");
        _ui.SetInventory(_player.Inventory);
        CreateRegion();
    }

    public void SwitchRegion(int regionIndex) {
        if (regionIndex < 0 || regionIndex > _regions.Count) {
            return;
        }
        //if (_curr != null) _curr.Save();
    }

    public override void _Process(float delta) {
        base._Process(delta);
        if (Input.IsActionJustPressed("PauseMenu")) {
            GetTree().Paused = !GetTree().Paused;
        }
    }

    public Region CreateRegion() {
        Region testRegion = new Region();
        testRegion.Load(GetNode("World"), (TileMap)GetNode("TileMaps/Floor"));
        _player.GlobalPosition = testRegion.PlayerPosition;
        return testRegion;
    }

}

public class Region {
    public int Width = 5;
    public int Height = 5;
    public Vector2 PlayerPosition;
    int[,] _map;
    List<EntityData> _entities = new List<EntityData>();
    List<ExitPoint> _exitPoints = new List<ExitPoint>();

    public Region() {
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        // Generate Entities
        _entities = new List<EntityData>();
        _map = new int[Width, Height];
        for (int x = 0; x < Width; ++x) {
            for (int y = 0; y < Height; ++y) {
                if (rng.Randf() < 0.25f) {
                    _entities.Add(new EntityData("Tree", 
                        Globals.PixelsPerUnit * new Vector2(x, y)));
                }
                else {
                    PlayerPosition = Globals.PixelsPerUnit * new Vector2(x, y);
                }
                _map[x, y] = 0;
            }
        }
    }

    public void AddExitPoint(Vector2 position, Region region) {
    }

    public void Load(Node root, TileMap floor) {
        foreach (EntityData entityData in _entities) {
            entityData.Load(root);
        }
        for (int x = -1; x <= Width; ++x) {
            for (int y = -1; y <= Height; ++y) {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                    floor.SetCell(x, y, _map[x,y]);
                else    
                    floor.SetCell(x, y, 1);
            }
        }
    }

    public void Save(TileMap floor) {
        floor.Clear();
        foreach (EntityData entityData in _entities) {
            entityData.SaveAndRemove();
        }
    }

}

public class EntityData {
    public string SceneName;
    public Vector2 Position;

    Node activeEntity;

    public EntityData(string sceneName, Vector2 position) {
        SceneName = sceneName; Position = position;
    }

    public Node2D Load(Node root) {
        var scene = (Node2D)SceneDB.Instance.GetScene(SceneName).Instance();
        root.AddChild(scene);
        scene.GlobalPosition = Position;
        activeEntity = scene;
        return scene;
    }

    public void SaveAndRemove() {
        if (activeEntity != null) {
            activeEntity.QueueFree();
            activeEntity = null;
        }
    }
}