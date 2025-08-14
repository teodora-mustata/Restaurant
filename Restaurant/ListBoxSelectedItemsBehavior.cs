using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Restaurant.Model;

namespace Restaurant
{
    public static class ListBoxSelectedItemsBehavior
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectedItems",
                typeof(IList),
                typeof(ListBoxSelectedItemsBehavior),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        public static void SetBindableSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(BindableSelectedItemsProperty, value);
        }

        public static IList GetBindableSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(BindableSelectedItemsProperty);
        }

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                listBox.SelectionChanged -= ListBox_SelectionChanged;
                listBox.SelectionChanged += ListBox_SelectionChanged;

                if (e.OldValue is INotifyCollectionChanged oldCollection)
                    oldCollection.CollectionChanged -= (s, ev) => UpdateListBoxSelection(listBox);

                if (e.NewValue is INotifyCollectionChanged newCollection)
                    newCollection.CollectionChanged += (s, ev) => UpdateListBoxSelection(listBox);

                UpdateListBoxSelection(listBox);
            }
        }

        private static bool _isUpdatingSelection = false;

        private static void UpdateListBoxSelection(ListBox listBox)
        {
            if (_isUpdatingSelection) return;

            _isUpdatingSelection = true;

            var selectedItems = GetBindableSelectedItems(listBox);
            if (selectedItems == null)
            {
                _isUpdatingSelection = false;
                return;
            }

            var itemsToSelect = selectedItems.Cast<object>().ToList();

            listBox.SelectedItems.Clear();

            foreach (var item in itemsToSelect)
            {
                listBox.SelectedItems.Add(item); 
            }


            _isUpdatingSelection = false;
        }

        //private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var listBox = sender as ListBox;
        //    if (listBox == null) return;

        //    var bindableSelectedItems = GetBindableSelectedItems(listBox);
        //    if (bindableSelectedItems == null) return;

        //    bindableSelectedItems.Clear();


        //    foreach (var item in listBox.SelectedItems)
        //    {
        //        bindableSelectedItems.Add(item);
        //    }

        //    UpdateListBoxSelection(listBox);
        //}

        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;

            var bindableSelectedItems = GetBindableSelectedItems(listBox);
            if (bindableSelectedItems == null) return;

            _isUpdatingSelection = true;

            foreach (var removedItem in e.RemovedItems)
            {
                bindableSelectedItems.Remove(removedItem);
            }

            foreach (var addedItem in e.AddedItems)
            {
                if (!bindableSelectedItems.Contains(addedItem))
                    bindableSelectedItems.Add(addedItem);
            }

            _isUpdatingSelection = false;
        }


        //private static bool _isUpdatingSelection = false;

        //private static void UpdateListBoxSelection(ListBox listBox)
        //{
        //    if (_isUpdatingSelection) return;

        //    var selectedItems = GetBindableSelectedItems(listBox);
        //    if (selectedItems == null) return;
        //    _isUpdatingSelection = true;
        //    listBox.SelectedItems.Clear();

        //    var itemsToSelect = selectedItems.Cast<object>().ToList();
        //    foreach (var item in itemsToSelect)
        //    {
        //        listBox.SelectedItems.Add(item);
        //    }

        //    _isUpdatingSelection = false;
        //}


        //private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var listBox = sender as ListBox;
        //    if (listBox == null) return;

        //    var bindableSelectedItems = GetBindableSelectedItems(listBox);
        //    if (bindableSelectedItems == null) return;

        //    bindableSelectedItems.Clear();
        //    foreach (var item in listBox.SelectedItems)
        //    {
        //        bindableSelectedItems.Add(item);
        //    }
        //}

        //private static bool _isUpdatingSelection = false;

        //private static void UpdateListBoxSelection(ListBox listBox)
        //{
        //    // Dacă deja se face o actualizare, ieșim
        //    if (_isUpdatingSelection) return;

        //    _isUpdatingSelection = true;

        //    // Obținem elementele selectate din Binding
        //    var selectedItems = GetBindableSelectedItems(listBox);
        //    if (selectedItems == null)
        //    {
        //        Debug.WriteLine("Nu sunt elemente selectate.");
        //        _isUpdatingSelection = false;
        //        return;
        //    }

        //    // Log pentru a vedea ce elemente avem în Binding
        //    Debug.WriteLine("Elemente selectate pentru actualizare: " + string.Join(", ", selectedItems.Cast<object>().Select(item => item.ToString())));

        //    // Listă temporară pentru selecția de adăugat
        //    var itemsToAdd = new List<object>();

        //    // Împingem elementele selectate în lista temporară
        //    foreach (var item in selectedItems)
        //    {
        //        if (!listBox.SelectedItems.Contains(item))
        //        {
        //            itemsToAdd.Add(item);
        //        }
        //    }

        //    // Adăugăm elementele la SelectedItems din listă
        //    foreach (var item in itemsToAdd)
        //    {
        //        listBox.SelectedItems.Add(item);
        //    }

        //    // Log pentru a verifica selecția efectivă
        //    Debug.WriteLine("Selecție actualizată: " + string.Join(", ", listBox.SelectedItems.Cast<object>().Select(i => i.ToString())));

        //    _isUpdatingSelection = false;
        //}

        //private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var listBox = sender as ListBox;
        //    if (listBox == null) return;

        //    var bindableSelectedItems = GetBindableSelectedItems(listBox);
        //    if (bindableSelectedItems == null) return;

        //    Debug.WriteLine("Selecție schimbată:");

        //    // Curăță Binding-ul înainte de a actualiza selecția
        //    bindableSelectedItems.Clear();

        //    // Adaugă elementele noi în Binding
        //    foreach (var item in listBox.SelectedItems)
        //    {
        //        bindableSelectedItems.Add(item);
        //    }

        //    Debug.WriteLine("Selecție schimbată: " + string.Join(", ", bindableSelectedItems.Cast<object>().Select(item => item.ToString())));

        //    // Actualizează selecția efectivă
        //    UpdateListBoxSelection(listBox);
        //}

    }
}
