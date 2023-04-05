using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StackSumApp.Lib
{
    internal class StackSum : ObservableCollection<StackItem>, INotifyPropertyChanged
    {
        public float CommonSum
        {
            get
            {
                float result = 0.0f;
                for (int i = 0; i < this.Count; i += 1)
                {
                    result += this[i].MValue;
                }
                return result;
            }
        }

        protected override void InsertItem(int index, StackItem item)
        {
            base.InsertItem(index, item);
            this[index].Remove += Item_Remove;
            this[index].PropertyChanged += StackSum_PropertyChanged;
            OnPropertyChanged(nameof(CommonSum));
        }

        private void StackSum_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CommonSum));
        }

        protected override void RemoveItem(int index)
        {
            this[index].Remove -= Item_Remove;
            this[index].PropertyChanged -= StackSum_PropertyChanged;
            base.RemoveItem(index);
            OnPropertyChanged(nameof(CommonSum));
        }

        private void Item_Remove(object? sender, EventArgs e)
        {
            if (sender is StackItem stackItem)
            {
                for (int i = this.Count - 1; i >= 0; i -= 1)
                {
                    if (this[i].guid == stackItem.guid)
                    {
                        this.RemoveAt(i);
                        OnPropertyChanged();
                    }
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    internal class StackItem : INotifyPropertyChanged
    {
        public event EventHandler? Remove;

        public Guid guid { get; } = Guid.NewGuid();
        public float MValue => Value * Multiplier;
        public string Display { get; set; }
        public float Value 
        {
            get => _value;
            set
            {
                this._value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        private float _value = 0;
        public int Multiplier { get; set; }

        public StackItem(string display, float value, int multiplier = 1)
        {
            this.Display = display;
            this.Value = value;
            this.Multiplier = multiplier;
        }

        public ICommand RemoveCommand => new ActionCommand(() => {
            Remove?.Invoke(this, EventArgs.Empty);
        });

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}