using Godot;
using System;

public class ItemUI : Control {
    protected static ItemUI CurrentDragSlot;

    // Node references
    // ---------------
    IconUI _iconContainer;
    Label _itemCount;
    TextureRect _textureRect;
    Inventory _inventory;

    // Item Data
    // ---------
    protected int Position;

    public override void _Ready() {
        base._Ready();
        GetReferences();
        ConnectSignals();
        Clear();
    }

    // Initialization
    // --------------
    public void Initialize(int position, Inventory inventory) {
        Position = position; _inventory = inventory;
        UpdateSlot();
    }

    public void UpdateSlot() {
        if (_inventory != null) {
            var item = _inventory.Get(Position);
            if (item.IsEmpty()) {
                SetSlot(null, 0);
            }
            else {
                SetSlot(SpriteDB.Instance.GetSprite(item.ID), item.Count);
            }
        }
    }

    private void Clear() {
        SetSlot(null, 0);
    }

    private void ConnectSignals() {
        Connect("mouse_entered", this, nameof(HandleMouseEnter));
        Connect("mouse_exited", this, nameof(HandleMouseExit));
    }

    private void GetReferences() {
        _textureRect = (TextureRect)GetNode("IconUI/CenterContainer/ItemImage");
        _itemCount = (Label)GetNode("IconUI/Label");
        _iconContainer = (IconUI)GetNode("IconUI");
    }

    protected void SetSlot(Texture texture, int count) {
        _textureRect.Texture = texture;
        if (count <= 1)
            _itemCount.Text = "";
        else
            _itemCount.Text = count.ToString();
    }

    protected string GetCount() {
        return _itemCount.Text;
    }

    protected Texture GetTexture() {
        return _textureRect.Texture;
    }

    // Drag/Drop Helper Functions
    // --------------------------

    // Create the texture that follows the mouse when dragging.
    private TextureRect CreatePreviewTexture() {
        var dragTexture = new TextureRect();
        dragTexture.Expand = true;
        dragTexture.Texture = _textureRect.Texture;
        dragTexture.RectSize = _textureRect.RectSize;
        return dragTexture;
    }

    // Create the node in which to center the preview texture.
    private Control CreatePreviewNode(TextureRect previewTexture) {
        var dragNode = new Control();
        dragNode.AddChild(previewTexture);
        previewTexture.RectPosition = -0.5f * previewTexture.RectSize;
        return dragNode;
    }

    // Cast data.
    private Godot.Collections.Dictionary ReadDropData(object data) {
        return (Godot.Collections.Dictionary)data;
    }

    // Send item ui data in a marshable form.
    private Godot.Collections.Dictionary CreateDragData() {
        var data = new Godot.Collections.Dictionary();
        data["Origin"] = this;
        return data;
    }

    private bool ValidateData(Godot.Collections.Dictionary data) {
        return data.Contains("Origin");
    }

    private void SwapSlotDataWith(ItemUI origin) {
        if (_inventory != null) {
            _inventory.Swap(Position, origin.Position);
            // Update both slots after swap
            UpdateSlot();
            origin.UpdateSlot();
        }
    }

    private bool CanDrag() {
        return _inventory != null && !_inventory.Get(Position).IsEmpty();
    }

    private void CreatePreview() {
        var previewTexture = CreatePreviewTexture();
        var previewNode = CreatePreviewNode(previewTexture);
        SetDragPreview(previewNode);
        //if (UI.Instance != null) {
        //    GD.Print("Setting instance");
        //    RemoveChild(previewNode);
        //    UI.Instance.AddChild(previewNode);
        //}
    }

    // Drag/Drop Functions
    // -------------------
    public override object GetDragData(Vector2 position) {
        if (!CanDrag()) return null;
        CurrentDragSlot = this;
        _iconContainer.Hide();
        CreatePreview();
        return CreateDragData();
    }

    public override bool CanDropData(Vector2 position, object data) {
        return _inventory != null && ValidateData(ReadDropData(data));
    }

    public override void DropData(Vector2 position, object data) {
        var dropData = ReadDropData(data);
        var origin = (ItemUI)dropData["Origin"];
        SwapSlotDataWith(origin);
    }

    public override void _Notification(int what) {
        base._Notification(what);
        // Show origin slot texture that was hidden during preview.
        if (what == NotificationDragEnd && CurrentDragSlot == this) {
            _iconContainer.Show();
            CurrentDragSlot = null;
        }
    }

    // Handle Input Events
    // -------------------
    private void HandleMouseEnter() {
        _iconContainer.ScaleIcon(1.2f);
    } 

    private void HandleMouseExit() {
        _iconContainer.ScaleIcon(1f);
    }

    private void HandleActivationInput() {
        if (Input.IsActionJustPressed("Inventory")) {
        }
    }

}