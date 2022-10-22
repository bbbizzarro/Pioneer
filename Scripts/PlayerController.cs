using Godot;
using System;

public class PlayerController : Character {
    bool _isInputPaused;
    public Inventory Inventory {private set; get;}
    Sensor _Sensor;
    Interactor _interactor;

    public override void _Ready() {
        base._Ready();
        Inventory = new Inventory(8);
        GetReferences();
        MakeConnections();
    }

    private void MakeConnections() {
        Inventory.UpdatedEvent += HandleInventoryUpdate;
    }

    private void GetReferences() {
        _Sensor = (Sensor)GetNode("Sensor");
        _Sensor.ItemPickupEvent += HandleItemPickup;
        _interactor = (Interactor)GetNode("Interactor");
        _interactor.Initialize(Inventory);
    }

    private void SetHeldToActive() {
        Inventory.ItemStack _itemStack = Inventory.GetActive();
        if (!_itemStack.IsEmpty()) {
            _spriteGroup.Held.SetHeld(_itemStack.ID);
        }
        else {
            _spriteGroup.Held.ClearHeld();
        }
    }

    public override void _Process(float delta) {
        base._Process(delta);
        ParseInput();
        _interactor.UpdateNearest();
    }

    private void HandleInventoryUpdate() {
        SetHeldToActive();
    }

    public void HandleItemPickup(string id, int count) {
        Inventory.Add(id, count);
        Inventory.Update();
    }

    private void ParseInput() {
        if (Input.IsActionJustPressed("Inventory")) _isInputPaused = !_isInputPaused;
        if (_isInputPaused) {
            HandleMovement(Vector2.Zero);
            return;
        }
        HandleMovement(Input.GetVector("Left", "Right", "Up", "Down"));
        if (Input.IsActionPressed("Attack")) HandleAttack();
        FaceToward(GetGlobalMousePosition());
        PositionHeld(GetGlobalMousePosition());
        HandleHotbarInput();
        HandleInteractionInput();
    }

    private void HandleInteractionInput() {
        if (Input.IsActionJustPressed("Interact")) {
            _interactor.Interact();
        }
        //if (Input.IsActionJustPressed("Attack")) {
        //    _interactor.Interact();
        //}
    }

    private void HandleHotbarInput() {
        if (Input.IsActionJustPressed("Hotbar1")) SetActiveSlot(0);
        if (Input.IsActionJustPressed("Hotbar2")) SetActiveSlot(1);
        if (Input.IsActionJustPressed("Hotbar3")) SetActiveSlot(2);
        if (Input.IsActionJustPressed("Hotbar4")) SetActiveSlot(3);
    }

    private void SetActiveSlot(int slot) {
        Inventory.SetActiveSlot(slot);
        Inventory.Update();
    }
}
