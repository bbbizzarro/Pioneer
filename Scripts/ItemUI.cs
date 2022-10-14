using Godot;
using System;

public delegate void DragEventHandler(ItemUI itemUI);
public class ItemUI : Control {
    public event DragEventHandler DragEvent;
    public event DragEventHandler DropEvent;

    bool _mouseIsOver;
    bool _isDragging;
    bool _isActive;
    Vector2 offset;
    Control _itemImage;
    Label _itemCount;
    TextureRect _textureRect;
    string _currentSprite;

    public override void _Ready() {
        base._Ready();
        Connect("mouse_entered", this, nameof(HandleMouseEnter));
        Connect("mouse_exited", this, nameof(HandleMouseExit));
        offset = new Vector2(RectSize.x / 2, RectSize.y / 2);
        _textureRect = (TextureRect)GetNode("ItemUI/CenterContainer/ItemImage");
        _itemCount = (Label)GetNode("ItemUI/Label");
        _itemImage = (Control)GetNode("ItemUI");
        _itemImage.Visible = false;
    }

    public void Set(string name, int count) {
        _itemImage.Visible = true;
        _itemCount.Text = count.ToString();
        if (_currentSprite != name) {
            GD.Print("loading new sprite");
            _currentSprite = name;
            _textureRect.Texture = SpriteDB.Instance.GetSprite(name);
        }
    }

    public void UnSet() {
        _itemImage.Visible = false;
    }

    public override void _Process(float delta) {
        base._Process(delta);
        HandleActivationInput();
        if (!_isActive) return;

        if (_isDragging) HandleDragState();
        else HandleSelectableState();
    }

    private void FollowMouse() {
        RectGlobalPosition = GetGlobalMousePosition() - offset;
    }

    private void ResetPosition() {
        RectPosition = Vector2.Zero;
    }

    private void Drop() {
        GD.Print("Dropped.");
        _isDragging = false;
        ResetPosition();
        DropEvent?.Invoke(this);
    }

    private void Drag() {
        GD.Print("Drag begin.");
        _isDragging = true;
        DragEvent?.Invoke(this);
    }

    private void HandleDragState() {
        FollowMouse();
        if (Input.IsActionJustReleased("Select")) {
            Drop();
        }
    }

    private void HandleSelectableState() {
        if (IsSelected()) {
            Drag();
        }
    }

    private bool IsSelected() {
        return _mouseIsOver && Input.IsActionJustPressed("Select");
    }

    private void HandleMouseEnter() {
        _mouseIsOver = true;
    }

    private void HandleMouseExit() {
        _mouseIsOver = false;
    }

    private void HandleActivated() {
        _isActive = true;
    }

    private void HandleDeactivated() {
        _isActive = false;
        Drop();
    }

    private void HandleActivationInput() {
        if (Input.IsActionJustPressed("Inventory")) {
            if (_isActive) {
                HandleDeactivated();
            }
            else {
                HandleActivated();
            }
        }
    }

}
