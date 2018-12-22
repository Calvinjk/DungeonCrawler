using UnityEngine;
using System.Collections;
using System;

/*
---Heap Properties--- 
Parent of a node 		= (n - 1) / 2
Left child of a node 	= 2n + 1
Right child of a node	= 2n + 2
*/
public class Heap<T> where T: IHeapItem<T>{

	T[] items;				// An array of all of the items in this container
	int currentItemCount;	// How many items live in this container

	// Constructor based on a max size
	public Heap(int maxHeapSize){
		items = new T[maxHeapSize];
	}

	// Add a new item to the heap
	public void Add(T item){
		// First add it to the end of the array
		item.HeapIndex = currentItemCount;
		items [currentItemCount] = item;
		// Now use SortUp to place it in the correct position
		SortUp (item);
		// We added an item, so increment the count
		++currentItemCount;
	}

	public T RemoveFirst(){
		// Save the root of the tree structure as it will be the item we want
		T firstItem = items [0];
		--currentItemCount;

		// Overwrite the old root with the last thing in the array
		items [0] = items[currentItemCount];
		items [0].HeapIndex = 0;
		// Now use SortDown to place it in the correct position
		SortDown (items [0]);
		return firstItem;
	}

	// When updating an item for pathfinding, we only ever decrease the "weight", so only sortup is necessary
	public void UpdateItem(T item){
		SortUp (item);
	}

	// Public method to get the size of the heap
	public int Count{
		get { 
			return currentItemCount;
		}
	}

	// Public method to check if an item is in the heap
	public bool Contains(T item){
		return Equals (items [item.HeapIndex], item);
	}

	void SortDown(T item){
		while (true) {
			// First find the children
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			// First check if it even has a left child
			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				// If it has two children, determine which it should swap with
				if (childIndexRight < currentItemCount) {
					if (items [childIndexLeft].CompareTo (items [childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				// Now that we have the better of the two children, should we swap at all?
				if (item.CompareTo (items [swapIndex]) < 0) {
					Swap (item, items [swapIndex]);
				} else {
					return;
				}
			} else {
				return;
			}
		}
	}

	void SortUp(T item){
		// Find the current parent of this item
		int parentIndex = (item.HeapIndex - 1) / 2;

		// Loop and swap positions until it is in the correct spot
		while (true) {
			T parentItem = items [parentIndex];
			// CompareTo: Higher priority (1), Equal priority (0), Lower priority (-1)
			if (item.CompareTo(parentItem) > 0) {
				Swap (item, parentItem);
			} else {
				break;
			}
		}
	}

	void Swap(T a, T b){
		items [a.HeapIndex] = b;
		items [b.HeapIndex] = a;
		// Must create a temporary variable for swapping in order to not lose data
		int aIndex = a.HeapIndex;
		a.HeapIndex = b.HeapIndex;
		b.HeapIndex = aIndex;
	}
}

public interface IHeapItem<T> : IComparable<T>{
	int HeapIndex {
		get;
		set;
	}
}