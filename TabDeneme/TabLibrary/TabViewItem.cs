using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TabLibrary
{
    [Preserve(AllMembers = true)]
    [ContentProperty(nameof(TabContent))]
    public partial class TabViewItem : ContentView
    {

        public static readonly BindableProperty TabContentProperty = BindableProperty.Create(nameof(TabContent), typeof(View), typeof(TabViewItem), null);
        public static readonly BindableProperty FontAttributeProperty = BindableProperty.Create(nameof(FontAttribute), typeof(FontAttributes), typeof(TabViewItem), FontAttributes.None);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(TabViewItem), 14.0);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(TabViewItem), Color.Default);
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(TabViewItem), string.Empty);
        public static readonly BindableProperty TabBackgroundColorProperty = BindableProperty.Create(nameof(TabBackgroundColor), typeof(Color), typeof(TabViewItem), Color.Default);
        public static readonly BindableProperty SelectedTabBackgroundColorProperty = BindableProperty.Create(nameof(SelectedTabBackgroundColor), typeof(Color), typeof(TabViewItem), Color.Default);
        public static readonly BindableProperty TabIconProperty = BindableProperty.Create(nameof(TabIcon), typeof(string), typeof(TabViewItem), string.Empty);
        public static readonly BindableProperty TabIconPlaceProperty = BindableProperty.Create(nameof(TabIconPlace), typeof(ImagePosition), typeof(TabViewItem), ImagePosition.Top);
        public static readonly BindableProperty TabIconHeightPlaceProperty = BindableProperty.Create(nameof(TabIconHeight), typeof(double), typeof(TabViewItem), 60.0);

        public TabViewItem() { }

        public View TabContent
        {
            get => (View)GetValue(TabContentProperty);
            set => SetValue(TabContentProperty, value);
        }      
        public FontAttributes FontAttribute
        {
            get => (FontAttributes)GetValue(FontAttributeProperty);
            set => SetValue(FontAttributeProperty, value);
        }
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public Color TabBackgroundColor
        {
            get => (Color)GetValue(TabBackgroundColorProperty);
            set => SetValue(TabBackgroundColorProperty, value);
        }   
        public Color SelectedTabBackgroundColor
        {
            get => (Color)GetValue(SelectedTabBackgroundColorProperty);
            set => SetValue(SelectedTabBackgroundColorProperty, value);
        }
        public string TabIcon
        {
            get => (string)GetValue(SelectedTabBackgroundColorProperty);
            set => SetValue(SelectedTabBackgroundColorProperty, value);
        }
        public ImagePosition TabIconPlace
        {
            get => (ImagePosition)GetValue(TabIconPlaceProperty);
            set => SetValue(TabIconPlaceProperty, value);
        }
        public double TabIconHeight
        {
            get => (double)GetValue(TabIconHeightPlaceProperty);
            set => SetValue(TabIconHeightPlaceProperty, value);
        }
        public enum ImagePosition
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {         
            base.OnPropertyChanged(propertyName);
          
            if (propertyName == "Content")
            {
                TabContent = Content;
            }       
        }
    }
}
