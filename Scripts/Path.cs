using Godot;
using System;

public class Path : Line2D {
	public Location Start { private set; get; }
	public Location End { private set; get; }

    float animationDuration = 0.3f;
    float time;
    bool isAnimating;
    bool isHidden;


    public override void _Process(float delta) {
        Animate(delta);
    }

    public void Connect(Location start, Location end) {
		Start = start; End = end;
        start.AddPath(this);
        end.AddPath(this);
		ClearPoints();
		AddPoint(start.GlobalPosition);
		AddPoint(start.GlobalPosition);
        HidePath();
    }

    private void Animate(float delta) {
        if (time >= animationDuration) {
            return;
        }
        time += delta;
        float t = time / animationDuration;
        Vector2 current = End.GlobalPosition * t + Start.GlobalPosition * (1f - t);
        SetPointPosition(1, current);
    }

    public void Reveal(Location location) {
        if (!isHidden) return;
        isHidden = false;
        if (End == location) {
            var temp = Start;
            Start = End;
            End = temp;
            SetPointPosition(0, Start.GlobalPosition);
            SetPointPosition(1, End.GlobalPosition);
        }
        animationDuration = 0.08f * (Start.GlobalPosition - End.GlobalPosition).Length() / Globals.PixelsPerUnit;
        time = 0;
        Show();
    }

    public void HidePath() {
        if (isHidden) return;
        isHidden = true;
        time = animationDuration;
        Hide();
    }
}
