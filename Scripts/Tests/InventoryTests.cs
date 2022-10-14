using Godot;
using System;
using System.Collections.Generic;

public class InventoryTests  {
    public void Test() {
        if (!BasicTest()) PrintFailedTest(nameof(BasicTest));
        else PrintPassedTest(nameof(BasicTest));
        if (!AtLimitTest()) PrintFailedTest(nameof(AtLimitTest));
        else PrintPassedTest(nameof(AtLimitTest));
        if (!StackTest()) PrintFailedTest(nameof(StackTest));
        else PrintPassedTest(nameof(StackTest));
        if (!MoveTest()) PrintFailedTest(nameof(MoveTest));
        else PrintPassedTest(nameof(MoveTest));
    }

    private void PrintPassedTest(string name) {
        GD.Print(String.Format("Passed {0}", name));
    }

    private void PrintFailedTest(string name) {
        GD.Print(String.Format("!!!!! Failed {0}", name));
    }

    private bool BasicTest() {
        Inventory inventory = new Inventory(10);
        inventory.Add("ItemA", 1);
        if (inventory.NumberOf("ItemA") != 1) return false;
        inventory.Remove("ItemA", 1);
        if (inventory.NumberOf("ItemA") != 0) return false;
        return true;
    }

    private bool AtLimitTest() {
        Inventory inventory = new Inventory(3);
        inventory.Add("ItemA", 1);
        inventory.Add("ItemB", 1);
        inventory.Add("ItemC", 1);
        return !inventory.Add("ItemD", 1);
    }

    private bool StackTest() {
        Inventory inventory = new Inventory(3);
        for (int i = 0; i < 10; ++i) {
            inventory.Add("ItemA", 1);
        }
        return inventory.NumberOf("ItemA") == 10;
    }

    private bool MoveTest() {
        Inventory inventory = new Inventory(3);
        inventory.Add("ItemA", 1);
        inventory.Add("ItemB", 10);
        inventory.Add("ItemC", 1);
        inventory.Remove("ItemB", 10);
        inventory.Move("ItemA", 2);
        var lst = new List<Inventory.ItemStack>(inventory.GetSlots());
        return lst[0].ID == "ItemC" && lst[1].IsEmpty() && lst[2].ID == "ItemA";
    }
}
