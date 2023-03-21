using Godot;
using System;
using System.Collections.Generic;

public class SceneDB : Node {
    public static SceneDB Instance;

    Dictionary<string, PackedScene> _db;

    public override void _Ready() {
        if (Instance != null) {
            QueueFree();
        }
        else {
            Instance = this;
            IndexScenes("res://Scenes");
        }
    }

    public PackedScene GetScene(string ID) {
        if (_db.ContainsKey(ID)) {
            return _db[ID];
        }
        else {
            return null;
        }
    }

    public Node Create(string ID, Node parent) {
        PackedScene packedScene = GetScene(ID);
        Node scene = packedScene.Instance();
        parent.AddChild(scene);
        return scene;
    }

    //public Node InstanceScene(string ID) {
    //    if (_db.ContainsKey(ID)) {
    //        return _db[ID].Instance();
    //    }
    //    else {
    //        return null;
    //    }
    //}

    private void IndexScenes(string path) {
        _db = new Dictionary<string, PackedScene>();

        var directory = new Directory();
        directory.Open(path);
        directory.ListDirBegin();

        string file = directory.GetNext();
        while (file != "") {
            // Avoid hidden files and grab PNG files only
            if (!file.BeginsWith(".") && file.EndsWith(".tscn")) {
                string name = file.Substring(0, file.Length - 5);
                string pathToFile = path + "/" + file;
                _db.Add(name, (PackedScene)ResourceLoader.Load(pathToFile));
			}
            file = directory.GetNext();
		}

        directory.ListDirEnd();
	}

}
