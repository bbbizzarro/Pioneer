using Godot;
using System;

public class AnimSprite : Sprite {

    float time;
    float speed;
    Animation current;

    public override void _Ready() {
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        speed = rng.RandfRange(0.9f, 1f) * 10f;
        current = new Idle(this, speed);
    }

    public override void _Process(float delta) {
        Advance(delta);
        current.Animate(time);
    }

    private void Advance(float delta) {
        time += delta;
	}

    public void SquashAndStretch(float time, float speed) {

        float theta = Mathf.Cos(speed * time);
        Vector2 xLims = new Vector2(1f, 1.02f);
        Vector2 yLims = new Vector2(1f, 0.96f);
        Vector2 hLims = new Vector2(0f, (yLims.x - yLims.y) / 2f);

        float x = xLims.x * (1f - theta) + xLims.y * theta;
        float y = yLims.x * (1f - theta) + yLims.y * theta;
        float h = hLims.x * (1f - theta) + hLims.y * theta;

        Scale = new Vector2(x, y);
        Position = new Vector2(0, h * Texture.GetSize().y);
	}

    public void Sinusoidal(float time, float speed) {
        
    }

    public class Animation {
        protected AnimSprite animSprite;
        public Animation(AnimSprite animSprite) {
            this.animSprite = animSprite;
        }

        public virtual void Animate(float time) {
        }
    }

    public class Idle : Animation {
        float speed;
        public Idle(AnimSprite animSprite, float speed) : base(animSprite) {
            this.speed = speed;
        }

        public override void Animate(float time) {
            animSprite.SquashAndStretch(time, speed);
        }
    }

    public class Run : Animation {
        float speed;
        public Run(AnimSprite animSprite, float speed) : base(animSprite) {
            this.speed = speed;
        }

        public override void Animate(float time) {
            animSprite.SquashAndStretch(time, speed);
        }
    }
}