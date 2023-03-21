using System.Collections;
using System.Collections.Generic;
using System;

public interface IHeap<T> {
    void Insert(T item);
    T Pop();
    T Peek();
    //public void Update(T item);
}

public class BinaryHeap<T> : IHeap<T>{
    int head;
    int capacity;
    T[] items;
    Dictionary<T, int> itemMap;
    IComparer<T> comparer;

	public BinaryHeap(int capacity, IComparer<T> comparer) {
        this.comparer = comparer;
        this.capacity = capacity;
        // Items[0] is a sentinel
        head = 0;
        items = new T[capacity + 1];
        itemMap = new Dictionary<T, int>();
	}

    public void Insert(T item) {
        head += 1;
        if (head >= capacity) {
            head -= 1;
            throw new Exception("Trying to insert into a full BinaryHeap");
		}
        items[head] = item;
        itemMap[item] = head;
        BubbleUp(head);
    }

    void BubbleUp(int k) { 
        /*
        left-child: 2k
        right-child: 2*k + 1
        parent: k / 2
        */
        if (k == 1) {
            return;
		}
        int p = k / 2;
        T item = items[k];
        T parent = items[p];
        if (comparer.Compare(parent, item) > 0) {
            Swap(k, k / 2);
            BubbleUp(p);
		}
        else {
            return;
		}
	}

    void BubbleDown(int k) {
        if (k >= head) {
            return;
		}
        int l = 2 * k;
        int r = 2 * k + 1;

        int s = k;
        if (l <= head && comparer.Compare(items[l], items[s]) < 0) {
            s = l;
		}
        if (r <= head && comparer.Compare(items[r], items[s]) < 0) {
            s = r;
		}
        if (s == k) {
            return;
		}
        Swap(s, k);
        BubbleDown(s);
	}

    void Swap(int i, int j) {
        T temp = items[i];
        items[i] = items[j];
        items[j] = temp;
        itemMap[items[i]] = i;
        itemMap[items[j]] = j;
	}


    public T Peek() {
        if (IsEmpty()) {
            throw new Exception("Trying to access an empty heap");
		}
        return items[1];
    }

    public T Pop() {
        if (IsEmpty()) {
            throw new Exception("Trying to pop an empty heap");
		}
        T returnItem = items[1];
        itemMap.Remove(items[1]);
        Swap(head, 1);
        head -= 1;
		BubbleDown(1);
        return returnItem;
    }

    public void Update(T item) { 
        if (!itemMap.ContainsKey(item)) {
            throw new Exception("No such item to update in heap");
		}
        BubbleUp(itemMap[item]);
        BubbleDown(itemMap[item]);
	}

    public bool IsEmpty() {
        return head <= 0;
	}

    public override string ToString() { 
        string sm ="";
        for (int i = 0; i < 10; ++i) { 
            sm += items[i].ToString() + " ";
		}
        return sm;
	}

    public void Clear() {
        head = 0;
	}
}
