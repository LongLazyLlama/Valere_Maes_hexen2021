using DAE.Commons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace CardSystem
{
    public class SelectionEventArgs<TSelectableItem> : EventArgs
    {
        public TSelectableItem SelectionItem { get; }

        public SelectionEventArgs(TSelectableItem selectionItem)
        {
            SelectionItem = selectionItem;
        }
    }

    public class SelectionManager<TSelectableItem>
    {
        public event EventHandler<SelectionEventArgs<TSelectableItem>> Selected;
        public event EventHandler<SelectionEventArgs<TSelectableItem>> Deselected;

        //Voor een lijst van unieke elementen.
        private HashSet<TSelectableItem> _selectedItems = new HashSet<TSelectableItem>();

        public HashSet<TSelectableItem> SelectedItems => _selectedItems;

        public TSelectableItem SelectedItem => _selectedItems.First();

        public bool HasSelection => _selectedItems.Count > 0;

        public bool IsSelected(TSelectableItem selectableItem)
            => _selectedItems.Contains(selectableItem);

        public bool Select(TSelectableItem selectableItem)
        {
            Debug.Log($"Selected {selectableItem}");
            if (_selectedItems.Add(selectableItem))
            {
                OnSelected(new SelectionEventArgs<TSelectableItem>(selectableItem));
                return true;
            }
            return false;
        }

        public bool Deselect(TSelectableItem selectableItem)
        {
            //Debug.Log($"Deselected {selectableItem}");
            if (_selectedItems.Remove(selectableItem))
            {
                OnDeselected(new SelectionEventArgs<TSelectableItem>(selectableItem));
                return true;
            }
            return false;
        }

        public bool Toggle(TSelectableItem selectableItem)
        {
            if (IsSelected(selectableItem))
                return !Deselect(selectableItem);
            else
                return Select(selectableItem);
        }

        public void DeselectAll()
        {
            foreach (var selectedItem in _selectedItems.ToList())
            {
                Deselect(selectedItem);
            }
        }

        protected virtual void OnSelected(SelectionEventArgs<TSelectableItem> e)
        {
            var handler = Selected;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDeselected(SelectionEventArgs<TSelectableItem> e)
        {
            var handler = Deselected;
            handler?.Invoke(this, e);
        }
    }
}
