using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CustomRadioButtonsGroup
{
    public class SelectionChangedEventArgs : EventArgs
    {
        public object SelectedItem { get; }
        public object SelectedValue { get; }
        public int SelectedIndex { get; }
        public SelectionChangedEventArgs(object selectedItem, object selectedValue, int selectedIndex)
        {
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
            SelectedValue = selectedValue;
        }
    }

    public class ItemsAddedEventArgs : EventArgs
    {
        public IEnumerable<object> Items { get; }
        public ItemsAddedEventArgs(IEnumerable<object> items)
        {
            Items = items;
        }
    }

    public enum Directions
    { RTL, LTR }

    [XamlCompilation(XamlCompilationOptions.Compile), Obsolete]
    public class RadioButtonsGroupUnicoded : ContentView
    {
        StackLayout parentStack;
        List<Label> lbRadios;
        readonly string chckd = "◉";
        readonly string unchckd = "◯";
        readonly string rbClassId = "r";

        public RadioButtonsGroupUnicoded()
        {
            parentStack = new StackLayout();
            Content = parentStack;
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(RadioButtonsGroup), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnItemsSourceChanged);
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(RadioButtonsGroup), defaultValue: -1, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(RadioButtonsGroup), defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(RadioButtonsGroup), defaultValue: StackOrientation.Vertical);
        public static readonly BindableProperty FrontColorProperty = BindableProperty.Create(nameof(FrontColor), typeof(Color), typeof(RadioButtonsGroup), defaultValue: Color.Black);
        public static readonly BindableProperty DirectionProperty = BindableProperty.Create(nameof(Direction), typeof(Directions), typeof(RadioButtonsGroup), defaultValue: Directions.LTR);
        public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(RadioButtonsGroup));
        public static readonly BindableProperty SelectedValuePathProperty = BindableProperty.Create(nameof(SelectedValuePath), typeof(string), typeof(RadioButtonsGroup));
        public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create(nameof(SelectedValue), typeof(object), typeof(RadioButtonsGroup));

        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public StackOrientation Orientation
        {
            get { return (StackOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public Color FrontColor
        {
            get { return (Color)GetValue(FrontColorProperty); }
            set { SetValue(FrontColorProperty, value); }
        }

        public Directions Direction
        {
            get { return (Directions)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public delegate void ItemsAddedHandler(object sender, ItemsAddedEventArgs e);
        public event ItemsAddedHandler OnItemsAdded;

        public delegate void SelectionChangedHandler(object sender, SelectionChangedEventArgs e);
        public event SelectionChangedHandler OnSelectionChanged;


        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null)
                return;
            var @this = bindable as RadioButtonsGroupUnicoded;

            // unsubscribe from the old value

            var oldNPC = oldValue as INotifyPropertyChanged;
            if (oldNPC != null)
            {
                oldNPC.PropertyChanged -= @this.OnItemsSourcePropertyChanged;
            }

            var oldNCC = oldValue as INotifyCollectionChanged;
            if (oldNCC != null)
            {
                oldNCC.CollectionChanged -= @this.OnItemsSourceCollectionChanged;
            }

            // subscribe to the new value

            var newNPC = newValue as INotifyPropertyChanged;
            if (newNPC != null)
            {
                newNPC.PropertyChanged += @this.OnItemsSourcePropertyChanged;
            }

            var newNCC = newValue as INotifyCollectionChanged;
            if (newNCC != null)
            {
                newNCC.CollectionChanged += @this.OnItemsSourceCollectionChanged;
            }

            // inform the instance to do something

            @this.RebuildOnItemsSource();
        }

        private void RebuildOnItemsSource()
        {
            parentStack.Orientation = Orientation;
            parentStack.HorizontalOptions = Orientation == StackOrientation.Vertical ? LayoutOptions.Center : HorizontalOptions;
            parentStack.VerticalOptions = VerticalOptions;
            //parentStack.BackgroundColor = Color.Gray;
            //if (Direction == Directions.RTL)
            //    HorizontalOptions = LayoutOptions.End;

            var items = ItemsSource;
            if (Orientation == StackOrientation.Horizontal && Direction == Directions.RTL)
            {
                items = items.Reverse();
            }

            foreach (var item in items)
            {
                StackLayout radio = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    BindingContext = item,
                    //VerticalOptions = VerticalOptions,
                    HorizontalOptions = /*LayoutOptions.Fill*/Orientation == StackOrientation.Horizontal ? LayoutOptions.CenterAndExpand : LayoutOptions.Fill,
                    //BackgroundColor = Color.Orange
                };
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += RadioChecked;
                radio.GestureRecognizers.Add(tap);

                var displayText = DisplayMemberPath == null ? item.ToString() : item.GetType().GetProperty(DisplayMemberPath).GetValue(item, null).ToString();

                Label radioText = new Label { Text = displayText, VerticalOptions = LayoutOptions.Center, TextColor = FrontColor };
                Label radioCircle = new Label { ClassId = rbClassId, Text = unchckd, TextColor = FrontColor, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center };
                InitDirection(radio, radioText, radioCircle);
            }
            lbRadios = parentStack.Children.Where(x => x is StackLayout).SelectMany(x => ((StackLayout)x).Children.Where(l => l.ClassId == rbClassId).Cast<Label>()).ToList();
            if (Orientation == StackOrientation.Horizontal && Direction == Directions.RTL)
                lbRadios.Reverse();
            if (SelectedIndex >= 0)
            {
                try
                {
                    lbRadios[SelectedIndex].Text = chckd;
                }
                catch (ArgumentOutOfRangeException)
                {
                    SetValue(SelectedIndexProperty, 0);
                    lbRadios[SelectedIndex].Text = chckd;
                }
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnItemsAdded?.Invoke(this, new ItemsAddedEventArgs(ItemsSource));
        }

        private void OnItemsSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnItemsAdded?.Invoke(this, new ItemsAddedEventArgs(ItemsSource));
        }

        private void InitDirection(StackLayout radio, Label radioText, Label radioCircle)
        {
            if (Direction == Directions.RTL)
            {
                radioText.HorizontalOptions = LayoutOptions.EndAndExpand;
                radioCircle.HorizontalOptions = LayoutOptions.StartAndExpand;
                radio.Children.Add(radioCircle);
                radio.Children.Add(radioText);
            }
            else
            {
                //radioText.HorizontalOptions = LayoutOptions.StartAndExpand;
                radioCircle.HorizontalOptions = LayoutOptions.EndAndExpand;
                radio.Children.Add(radioText);
                radio.Children.Add(radioCircle);
            }
            parentStack.Children.Add(radio);
        }

        private void RadioChecked(object sender, EventArgs e)
        {
            StackLayout stRadio = (StackLayout)sender;
            var lb = stRadio.Children.First(x => x.ClassId == rbClassId) as Label;
            if (lb.Text == unchckd)
            {
                if (SelectedIndex >= 0)
                    lbRadios.Single(x => x.Text == chckd).Text = unchckd;
                lb.Text = chckd;
                SelectedItem = stRadio.BindingContext;
                SelectedValue = SelectedValuePath == null ? null : SelectedItem.GetType().GetProperty(SelectedValuePath).GetValue(SelectedItem, null);
                SelectedIndex = ItemsSource.ToList().IndexOf(SelectedItem);
                OnSelectionChanged?.Invoke(this, new SelectionChangedEventArgs(SelectedItem, SelectedValue, SelectedIndex));
            }
        }

    }
}