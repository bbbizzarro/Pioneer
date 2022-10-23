using Godot;
using System;

public class Market : Node2D, IEntity {

    Sprite _previewSprite;
    Interactable _interactable;
    PackedScene _pickUpScene = (PackedScene)ResourceLoader.Load("res://Scenes/PickUp.tscn");
    event EntityEventHandler OnDestroyedEvent;

    public override void _Ready() {
        GetReferences();
        MakeConnections();
    }

    private void GetReferences() {
        _previewSprite = (Sprite)GetNode("PreviewSprite");
        _interactable = (Interactable)GetNode("Interactable");
    }

    private void MakeConnections() {
        _interactable.EnterEvent += ShowPreview;
        _interactable.ExitEvent += HidePreview;
        _interactable.InteractEvent += HandleSelling;
    }

    private void HandleSelling(Inventory inventory) {
        Inventory.ItemStack itemStack = inventory.GetActive();
        if (!itemStack.IsEmpty()) {
            inventory.Remove(itemStack.ID, 1);
            inventory.Update();
            LoadPickUp();
        }
    }

    private void LoadPickUp() {
        PickUp pickUp = (PickUp)_pickUpScene.Instance();
        pickUp.Initialize("Coin");
        pickUp.GlobalPosition = GlobalPosition;
        CallDeferred(nameof(ParentChild), pickUp);
        //GetParent().AddChild(pickUp);
    }

    private void ParentChild(Node node) {
        GetParent().AddChild(node);
    }


    private void ShowPreview() {
        _previewSprite.Visible = true;
    }

    private void HidePreview() {
        _previewSprite.Visible = false;
    }

    public void SubscribeToOnDestroyed(EntityEventHandler call) {
        OnDestroyedEvent += call;
    }
}
