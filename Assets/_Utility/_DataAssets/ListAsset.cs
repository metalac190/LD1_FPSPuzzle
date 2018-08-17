using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Asset is the base functionality for implementing a generic component list. Inherit this from a separate
/// Scriptable Object in order to create a list of the specified type.
/// </summary>

namespace DataAssets
{
    public abstract class ListAsset<T> : ScriptableObject
    {
        public List<T> items = new List<T>();   // our generic list we can store things into

        //If the desired component isn't already in our list, add it
        public void Add(T t)
        {
            if (!items.Contains(t))
                items.Add(t);
        }

        //If the desired component is in our list, remove it
        public void Remove(T t)
        {
            if (items.Contains(t))
                items.Remove(t);
        }
    }
}

