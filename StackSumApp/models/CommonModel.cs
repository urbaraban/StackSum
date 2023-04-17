using Microsoft.Xaml.Behaviors.Core;
using StackSumApp.Lib;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace StackSumApp.models
{
    internal class CommonModel : ObservableCollection<StackSum>, INotifyPropertyChanged
    {
        public int SelectIndex => this.IndexOf(this._selectStack) + 1;

        public StackSum SelectStack 
        {
            get => _selectStack;
            set
            {
                _selectStack = value;
                OnPropertyChanged(nameof(SelectStack));
            }
        }
        private StackSum? _selectStack;

        public CommonModel()
        {
            base.Add(new StackSum());
        }

        protected override void InsertItem(int index, StackSum item)
        {
            base.InsertItem(index, item);
            SelectStack = item;
        }

        public void KeyPressVoid(Key key)
        {
            if (key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ParseAlreadyStackCommand.Execute(this);
            }
            if (key == Key.V && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                AddStackCommand.Execute(this);
                ParseAlreadyStackCommand.Execute(this);
                OnPropertyChanged(nameof(Count));
            }
        }

        public ICommand ParseAlreadyStackCommand => new ActionCommand(() => { GetClipboard(); });

        public void GetClipboard()
        {
            if (SelectStack == null)
            {
                if (this.Count == 0)
                {
                    AddStackCommand.Execute(this);
                }
                else
                {
                    SelectStack = this[0];
                }
            }

            string temp = Clipboard.GetText();
            string[] lines = temp.Split(new[] { "\r\n", "\r", "\n", "\t" }, StringSplitOptions.None);

            foreach (string element in lines)
            {
                string element1 = Regex.Replace(element, "[^0-9,.]", string.Empty);
                if (element1 != string.Empty)
                {
                    float result = 0;
                    bool ret = float.TryParse(element1, out result);
                    if (ret == true)
                    {
                        this.SelectStack.Add(new StackItem($"Item {this.SelectStack.Count}", result));
                    }
                }
            }
            OnPropertyChanged();
        }


        public ICommand NextStackCommand => new ActionCommand(() => {
            if (this.Count > 0)
            {
                int next_index = (this.IndexOf(this.SelectStack) + 1) % this.Count;
                SelectStack = this[next_index];
                OnPropertyChanged(nameof(SelectIndex));
                OnPropertyChanged();
            }
        });

        public ICommand PrevStackCommand => new ActionCommand(() => {
            if (this.Count > 0)
            {
                int index = this.IndexOf(this.SelectStack);
                int prev_index = (index - 1 + this.Count) % this.Count;
                SelectStack = this[prev_index];
                OnPropertyChanged(nameof(SelectIndex));
                OnPropertyChanged();
            }
        });

        public ICommand AddStackCommand => new ActionCommand(() => {
            this.Add(new StackSum());
            this.SelectStack = this.Last();
            OnPropertyChanged(nameof(Count));
        });

        public ICommand RemoveStackCommand => new ActionCommand(() => {
            this.Remove(this.SelectStack);
            OnPropertyChanged(nameof(Count));
        });

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
