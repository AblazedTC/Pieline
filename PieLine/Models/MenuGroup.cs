using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PieLine
{
    public class MenuGroup : INotifyPropertyChanged
    {
        private string _category = string.Empty;
        private ObservableCollection<MenuItem> _items = new();

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public ObservableCollection<MenuItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value ?? new ObservableCollection<MenuItem>());
        }

        public MenuGroup() { }

        public MenuGroup(string category, IEnumerable<MenuItem>? items = null)
        {
            _category = category ?? string.Empty;
            _items = items is not null
                ? new ObservableCollection<MenuItem>(items)
                : new ObservableCollection<MenuItem>();
        }

        public void AddItem(MenuItem item)
        {
            if (item is null) return;
            _items.Add(item);
            OnPropertyChanged(nameof(Items));
        }

        public bool RemoveItem(MenuItem item)
        {
            var removed = _items.Remove(item);
            if (removed) OnPropertyChanged(nameof(Items));
            return removed;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value!;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}