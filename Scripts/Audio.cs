using Godot;
using System;
using System.Collections.Generic;

//WAV for short and repetitive sound effects, and Ogg Vorbis for music, speech, and long sound effects

public class Audio : Node {
    [Export] string SoundDirectoryPath = "";
    Directory soundDirectory;
    List<AudioStreamPlayer> audioStreamers = new List<AudioStreamPlayer>();
    public static Audio Instance;
    RandomNumberGenerator rng = new RandomNumberGenerator();
    Dictionary<string, AudioStream> library = new Dictionary<string, AudioStream>();

    public override void _Ready() {
        if (Instance != null) QueueFree();
        else {
            Instance = this;
            foreach (Node audioStreamer in GetChildren()) {
                audioStreamers.Add((AudioStreamPlayer)audioStreamer);
            }
            soundDirectory = new Directory();
            soundDirectory.Open(SoundDirectoryPath);
            LoadLibrary(SoundDirectoryPath);
        }
    }

    private void LoadLibrary(string path) {
        var directory = new Directory();
        directory.Open(path);
        directory.ListDirBegin();

        string file = directory.GetNext();
        while (file != "") {
            if (!file.BeginsWith(".") && file.EndsWith(".wav.import")) {
                string name = file.Substring(0, file.Length - ".wave.import".Length +1);
                string pathToFile = path + "/" + name + ".wav";
                library.Add(name, (AudioStream)ResourceLoader.Load(pathToFile));
			}
            file = directory.GetNext();
		}
        directory.ListDirEnd();
    }

    private AudioStream LoadAudio(string name) {
        if (library.ContainsKey(name)) {
            return library[name];
        }
        else {
            return null;
        }
    }


/*
    private AudioStream LoadAudio(string name) {
        if (soundDirectory.FileExists(name + ".wav.import")) {
            return (AudioStream)ResourceLoader.Load(SoundDirectoryPath + name + ".wav");
        }
        else {
            return null;
        }
    }
    */

/*
    public void Play(string id) {
        AudioStream audio = library.LoadSound(id);
        if (audio == null) return;
        foreach (var audioStreamer in audioStreamers) {
            if (!audioStreamer.Playing) {
                audioStreamer.Stream = audio;
                audioStreamer.PitchScale = rng.RandfRange(0.8f, 1.2f);
                audioStreamer.Play();
                return;
            }
        }
    }
    */

    public void Play(string id) {
        AudioStream audio = LoadAudio(id);
        if (audio == null) return;
        foreach (var audioStreamer in audioStreamers) {
            if (!audioStreamer.Playing) {
                audioStreamer.Stream = audio;
                audioStreamer.PitchScale = rng.RandfRange(0.8f, 1.2f);
                audioStreamer.Play();
                return;
            }
        }
    }

}
