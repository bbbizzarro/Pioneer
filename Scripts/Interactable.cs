using Godot;
using System;

public delegate void InteractableEventHandler();
public delegate void StringIDEventHandler(string id);
public delegate void InventoryHandler(Inventory inventory);

public class Interactable : Area2D {
    Sprite previewSprite;

    public event InteractableEventHandler EnterEvent;
    public event InteractableEventHandler ExitEvent;
    public event InventoryHandler InteractEvent;

    public override void _Ready() {
        if (HasNode("PreviewSprite")) {
            previewSprite = (Sprite)GetNode("PreviewSprite");
		}
        previewSprite.Hide();
    }

    public virtual void Interact() {
        GD.Print("InteractioN!");
        QueueFree();
	}

    public void ShowPreview() {
        previewSprite.Show();
	}
    public void HidePreview() {
        previewSprite.Hide();
	}


    public void OnInteract(Inventory inventory) {
        InteractEvent?.Invoke(inventory);
    }

    public void OnEnter() {
        EnterEvent?.Invoke();
        previewSprite.Show();
    }

    public void OnExit() {
        ExitEvent?.Invoke();
        previewSprite.Hide();
    }
}
