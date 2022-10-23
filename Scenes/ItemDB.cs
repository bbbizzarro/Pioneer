using Godot;
using System;
using System.Collections.Generic;

public class ItemDB : Node {
    public static ItemDB Instance;
    Dictionary<string, Item> _db = new Dictionary<string, Item>() {};

    public override void _Ready() {
        if (Instance != null) QueueFree();
        else {
            Instance = this;
            CreateDebugDB();
        }
    }

    public Item GetItem(string itemID) {
        if (_db.ContainsKey(itemID)) return _db[itemID];
        else return Item.Default;
    }

    private void CreateDebugDB() {
        var itemList = new List<Item>() {
            MakeItem("Log", ItemType.Good),
            MakeItem("Coin", ItemType.Good),
            MakeItem("Axe", ItemType.Tool)
        };
        foreach (var item in itemList) {
            _db.Add(item.ID, item);
        }
    }

    private Item MakeItem(string id, ItemType type) {
        return new Item(id, type);
    }
}

public enum ItemType {
    Default,
    Tool,
    Good,
    Buildable
}

public struct Item {
    public static readonly Item Default = new Item("NULL", ItemType.Default);
    public string ID;
    public ItemType Type;
    public Item(string id, ItemType type) {
        ID = id; Type = type;
    }
}
