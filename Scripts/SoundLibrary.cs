using Godot;
using System;

public class SoundLibrary : ResourcePreloader {
    [Export] string path;

    public override void _Ready() {
        var directory = new Directory();
        directory.Open(path);
        directory.ListDirBegin();

        string file = directory.GetNext();
        while (file != "") {
            // Avoid hidden files and grab PNG files only
            if (!file.BeginsWith(".") && file.EndsWith(".wav")) {
                string name = file.Substring(0, file.Length - 4);
                string pathToFile = path + "/" + file;
                AddResource(name, ResourceLoader.Load(pathToFile));
			}
            file = directory.GetNext();
		}
        directory.ListDirEnd();
    }

    public AudioStream LoadSound(string name) {
        if (HasResource(name)) {
            return (AudioStream)GetResource(name);
        }
        else {
            return null;
        }
    }
}
