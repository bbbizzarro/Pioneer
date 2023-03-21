using Godot;
using System;

public class Prop : Node2D {

    protected AnimationPlayer animationPlayer;
    protected Sprite sprite;

    public override void _Ready() {
        animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        sprite = (Sprite)GetNode("Pivot/Sprite");
    }

    public virtual void SetSprite(string id) {
        Texture texture = SpriteDB.Instance.GetSprite(id);
        sprite.Texture = texture;
        sprite.Position = new Vector2(0, -sprite.Texture.GetHeight()/2f);
    }

    public virtual void SetSpriteByCategory(string category) {
        sprite.Texture = SpriteDB.Instance.GetRandomCategory(category);
        if (sprite.Texture != null)
            sprite.Position = new Vector2(0, -sprite.Texture.GetHeight()/2f);
        var rng = new RandomNumberGenerator();
        rng.Randomize();
        if (rng.Randf() > 0.5f) {
            sprite.FlipH = true;
        }
    }

    public virtual void Reveal() {
        Show();
        animationPlayer.Play("PropReveal");
    }

    public void HideProp() {
        Hide();
    }
}
