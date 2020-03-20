
using HDLibrary.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace ClipSum
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<List<ComboData>> memoryDatas = new List<List<ComboData>>();
        private static List<ComboData> comboDatas = new List<ComboData>();
        private static int index;
        private static bool stable;

        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)listView.Items).CollectionChanged +=
                listBox_CollectionChanged;
        }

        private void listBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (stable) Calc(true);
        }

        private void Calc(bool refresh)
        {
            if (refresh)
            {
                stable = false;
                if (index < 0) index = 0;
                listView.ItemsSource = null;
                listView.Items.Clear();
                listView.ItemsSource = memoryDatas[index];
                listView.UpdateLayout();
                stable = true;
            }

            indexLabel.Content = (index + 1).ToString() + "/" + memoryDatas.Count;

            float sum = 0;
            foreach (ComboData comboData in listView.Items)
                sum += (float)comboData.Value * comboData.Multiplier;
            SumLabel.Text = sum.ToString();
        }

        [Serializable]
        public class CustomHotKey : HotKey
        {
            public CustomHotKey(string name, Key key, ModifierKeys modifiers, bool enabled)
                : base(key, modifiers, enabled)
            {
                Name = name;
            }

            private string name;
            public string Name
            {
                get { return name; }
                set
                {
                    if (value != name)
                    {
                        name = value;
                        OnPropertyChanged(name);
                    }
                }
            }

            protected override void OnHotKeyPress()
            {
                //MessageBox.Show(string.Format("'{0}' has been pressed ({1})", Name, this));

                base.OnHotKeyPress();
            }


            protected CustomHotKey(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                : base(info, context)
            {
                Name = info.GetString("Name");
            }

            public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                base.GetObjectData(info, context);

                info.AddValue("Name", Name);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HotKeyHost hotKeyHost = new HotKeyHost((HwndSource)HwndSource.FromVisual(App.Current.MainWindow));
            hotKeyHost.AddHotKey(new CustomHotKey("SumClipboard", Key.F5, ModifierKeys.Control, true));
            hotKeyHost.AddHotKey(new CustomHotKey("AddClipboard", Key.F6, ModifierKeys.Control, true));
            hotKeyHost.HotKeyPressed += pressevent;
        }

        private void pressevent(object sender, HotKeyEventArgs e)
        {
            if (e.HotKey.Key == Key.F5)
                CalcClipboard(false);
            else if(e.HotKey.Key == Key.F6)
                CalcClipboard(true);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            CalcClipboard(false);
        }

        private class ComboData
        {
            public string Nik { get; set; }
            public string Display { get; set; }
            public float Value { get; set; }
            public int Multiplier { get; set; }
        }

        private void CalcClipboard(bool add)
        {
            string temp = Clipboard.GetText();
            string[] lines = temp.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
            );
            if (!add)
            {
                while (memoryDatas.Count >= 30) memoryDatas.RemoveAt(0);
                memoryDatas.Add(new List<ComboData>());
                index = memoryDatas.Count - 1;
            }

            ComboData comboData = new ComboData();
            foreach (string element in lines)
            {
               // if (!CheckWord(element))
                //{
                    string element1 = Regex.Replace(element, "[^0-9,]", string.Empty);
                    if (element1 != string.Empty)
                    {
                        comboData.Display = element;
                        comboData.Value = float.Parse(element1.Replace(" ", string.Empty).Replace('.', ','));
                        comboData.Multiplier = 1;
                        memoryDatas[index].Add(comboData);
                        comboData = new ComboData();

                    }
                //}
                //else comboData.Nik = element;
            }
            Calc(true);
        }

        private bool CheckWord(string str)
        {
            MatchCollection mathCollection = Regex.Matches(str, "[^0-9,]");
            if (mathCollection.Count > str.Length / 2)
                return true;
            else 
                return false;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            CalcClipboard(true);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            comboDatas.Remove((ComboData)btn.DataContext);
            Calc(true);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Calc(false);
        }

        private void minusIndexBtn_Click(object sender, RoutedEventArgs e)
        {
            if (index > 0)
            {
                index--;
                Calc(true);
            }
        }

        private void plusIndexBtn_Click(object sender, RoutedEventArgs e)
        {
            if (index < memoryDatas.Count - 1)
            {
                index++;
                Calc(true);
            }
        }

        private void dellListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (memoryDatas.Count > 0)
            {
                memoryDatas.RemoveAt(index);
                index--;
                Calc(true);
            }
        }
    }
}

