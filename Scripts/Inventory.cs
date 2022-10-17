using System.Collections.Generic;
using System.Linq;
using System;
using Godot;

public delegate void InventoryEventHandler();
public class Inventory {

    public event InventoryEventHandler UpdatedEvent;
    int _capacity;
    int _count;
    ItemStack[] _items;

    public Inventory(int capacity) {
        _capacity = capacity; 
        _items = new ItemStack[capacity];
        for (int i = 0; i < capacity; ++i) {
            _items[i] = ItemStack.EmptyStack();
        }
    }

    public void Update() {
        UpdatedEvent?.Invoke();
    }

    public ItemStack Get(string name) {
        foreach (ItemStack itemStack in _items) {
            if (itemStack.ID == name) return itemStack;
        }
        return ItemStack.EmptyStack();
    }

    public ItemStack Get(int position) {
        if (position < 0 || position >= _capacity) {
            return ItemStack.EmptyStack();
        }
        return _items[position];
    }

    private Tuple<int, ItemStack> GetPosition(string name) {
        for (int i = 0; i < _capacity; ++i) {
            if (_items[i].ID == name) 
                return new Tuple<int, ItemStack>(i, _items[i]);
        }
        return new Tuple<int, ItemStack>(-1, ItemStack.EmptyStack());
    }

    public bool HasItem(string name, int count) {
        ItemStack itemStack = Get(name);
        return !itemStack.IsEmpty() && itemStack.Count >= count;
    }

    public int NumberOf(string name) {
        return Get(name).Count;
    }

    public IEnumerable<string> GetItemIDs() {
        var idList = new List<string>();
        foreach (var stack in _items) {
            if (!stack.IsEmpty()) {
                idList.Add(stack.ID);
            }
        }
        return idList;
    }

    public IEnumerable<ItemStack> GetItems() {
        var items = new List<ItemStack>();
        foreach (var stack in _items) {
            if (!stack.IsEmpty()) {
                items.Add(stack);
            }
        }
        return items;
    }

    public IEnumerable<ItemStack> GetSlots() {
        return _items;
    }

    public bool Add(string item, int count) {
        // Do we already have a slot with the item?
        ItemStack itemStack = Get(item);
        if (!itemStack.IsEmpty()) {
            itemStack.Count += count;
            return true;
        }
        // Or can we reserve a new slot?
        if (_count >= _capacity) return false;
        ItemStack openSlot = GetOpenSlot();
        if (openSlot != null) {
            openSlot.Set(item, count);
            _count += 1;
            return true;
        }
        return false;
    }

    public void Swap(int positionA, int positionB) {
        if (positionA >= 0 && positionB >= 0 && positionA < _capacity && positionB < _capacity) {
            var itemA = _items[positionA];
            _items[positionA] = _items[positionB];
            _items[positionB] = itemA;
            GD.Print("Swap success.");
        }
    }

    public void Move(string item, int position) {
        var pair = GetPosition(item);
        int itemPos = pair.Item1;
        ItemStack itemStack = pair.Item2;
        if (itemStack.IsEmpty() || position >= _capacity) return;
        ItemStack temp = _items[position];
        _items[position] = itemStack;
        _items[itemPos] = temp;
    }

    private ItemStack GetOpenSlot() {
        foreach (var i in _items) {
            if (i.IsEmpty()) {
                return i;
            }
        }
        return null;
    }

    public bool Remove(string ID, int count) {
        ItemStack slot = Get(ID);
        if (!slot.IsEmpty() && slot.Count >= count) {
            slot.Count -= count;
            if (slot.Count <= 0) {
                slot.SetEmpty();
                _count -= 1;
            }
            return true;
        }
        return false;
    }

    public class ItemStack {
        static string NullID = "NULL";
        public string ID;
        public int Count;

        public ItemStack(string id, int count) {
            ID = id; Count = count;
        }

        public void Set(string id, int count) {
            ID = id; Count = count;
        }

        public bool IsEmpty() {
            return Count == 0 || ID == NullID;
        }

        public void SetEmpty() {
            ID = NullID;
            Count = 0;
        }

        public static ItemStack EmptyStack() {
            return new ItemStack(NullID, 0);
        }
    }
}