using CarouselView.FormsPlugin.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TabLibrary
{
    [Preserve(AllMembers = true)]
    [ContentProperty(nameof(TabItems))]
    public partial class TabView : ContentView
    {
        public TrulyObservableCollection<TabViewItem> TabItems { get; set; }

        public static readonly BindableProperty SwipeEnableProperty = BindableProperty.Create(nameof(SwipeEnable), typeof(bool), typeof(TabView), false);
        public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create(nameof(SelectedTab), typeof(int), typeof(TabView), 0);
        public static readonly BindableProperty TabbarHeightProperty = BindableProperty.Create(nameof(TabbarHeight), typeof(double), typeof(TabView), 40.0);
        public static readonly BindableProperty TabBarPositionProperty = BindableProperty.Create(nameof(TabBarPosition), typeof(TabPosition), typeof(TabView), TabPosition.Top);
        public bool SwipeEnable
        {
            get => (bool)GetValue(SwipeEnableProperty);
            set => SetValue(SwipeEnableProperty, value);
        }

        public int SelectedTab
        {
            get => (int)GetValue(SelectedTabProperty);
            set => SetValue(SelectedTabProperty, value);
        }     

        public double TabbarHeight
        {
            get => (double)GetValue(TabbarHeightProperty);
            set => SetValue(TabbarHeightProperty, value);
        }

        public TabPosition TabBarPosition
        {
            get => (TabPosition)GetValue(TabBarPositionProperty);
            set => SetValue(TabBarPositionProperty, value);
        }

        private readonly ObservableCollection<TabsItem.TabsView> TabsContentList;
        private readonly ScrollView scrollbar;
        private readonly StackLayout tablayout;
        private readonly Grid mainview;
        private readonly CarouselViewControl tabscontent;
        private int lastindex = 0;
        public TabView()
        {
            TabItems = new TrulyObservableCollection<TabViewItem>();
            BatchBegin();
            mainview = new Grid { RowSpacing = 0, ColumnSpacing = 0 };

            this.Content = mainview;
            TabsContentList = new ObservableCollection<TabsItem.TabsView>();
            scrollbar = new ScrollView 
            { 
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never, 
                Orientation = ScrollOrientation.Horizontal, 
                BackgroundColor = Color.Transparent, 
                HeightRequest = TabbarHeight 
            };
            tablayout = new StackLayout 
            { 
                Orientation = StackOrientation.Horizontal, 
                Spacing = 0, 
                BackgroundColor = Color.Transparent, 
                HorizontalOptions = LayoutOptions.FillAndExpand 
            };
            tabscontent = new CarouselViewControl
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
               
                IsSwipeEnabled = true,
                ShowArrows = false,
                ShowIndicators = false,               
                ItemTemplate = new DataTemplate(() =>
                 {
                    var tabViewItemContent = new ContentView
                     {
                         BackgroundColor = Color.Black,
                         VerticalOptions = LayoutOptions.FillAndExpand,
                         HorizontalOptions = LayoutOptions.FillAndExpand
                     };
                     tabViewItemContent.SetBinding(ContentProperty, "Content");
                     return tabViewItemContent;
                  
                 }),
                ItemsSource = TabsContentList
            };
            tabscontent.PositionSelected += Tabscontent_PositionSelected;

            double scrlwidth = 0;
            foreach (var item in TabItems)
            {
                Frame frm = new Frame { BackgroundColor = item.SelectedTabBackgroundColor, WidthRequest = item.WidthRequest, Padding = new Thickness(0, 0, 0, 0) };
                Button btn = new Button { Text = item.Title, CornerRadius = 0, WidthRequest = item.WidthRequest, TextColor = item.TextColor, BackgroundColor = item.TabBackgroundColor };                
                frm.Content = btn;
                tablayout.Children.Add(frm);
                btn.Clicked += Btn_Clicked;
                scrlwidth += item.WidthRequest;

            }
            scrollbar.WidthRequest = scrlwidth;
            tablayout.WidthRequest = scrlwidth;
            scrollbar.Content = tablayout;
            switch (TabBarPosition)
            {
                case TabPosition.Left:
                    mainview.ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = new GridLength(TabbarHeight) }, new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } };
                    scrollbar.Orientation = ScrollOrientation.Vertical;
                    tablayout.Orientation = StackOrientation.Vertical;
                    mainview.Children.Add(scrollbar, 0, 0);
                    mainview.Children.Add(tabscontent, 1, 0);;

                    break;
                case TabPosition.Bottom:
                    mainview.RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, new RowDefinition { Height = new GridLength(TabbarHeight) } };
                    mainview.Children.Add(scrollbar, 0, 1);
                    mainview.Children.Add(tabscontent , 0, 0);
                    break;
                case TabPosition.Right:
                    mainview.ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, new ColumnDefinition { Width = new GridLength(TabbarHeight) } };
                    scrollbar.Orientation = ScrollOrientation.Vertical;
                    tablayout.Orientation = StackOrientation.Vertical;
                    mainview.Children.Add(scrollbar, 1, 0);
                    mainview.Children.Add(tabscontent, 0, 0);
                    break;
                case TabPosition.Top:
                    mainview.RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = new GridLength(TabbarHeight) }, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } };
                    mainview.Children.Add(scrollbar, 0, 0);
                    mainview.Children.Add(tabscontent, 0, 1);
                    break;
                default:
                    mainview.RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, new RowDefinition { Height = new GridLength(TabbarHeight) } };
                    break;
            }
            TabItems.CollectionChanged += OnTabItemsCollectionChanged;
            BatchCommit();
        }

        private async void Tabscontent_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            foreach (var item in tablayout.Children)
            {
                (item as Frame).Padding = new Thickness(0);
            }
            (tablayout.Children[e.NewValue] as Frame).Padding = new Thickness(0, 0, 0, 3);
            var newindex = e.NewValue;            
            var phonewidth = Application.Current.MainPage.Width;
            var move = (tablayout.Children[e.NewValue] as Frame).X;
            try
            {
                move -= ((tablayout.Children[e.NewValue - 1] as Frame).WidthRequest / 2);
            }
            catch (Exception)
            {

            }
            if (move + phonewidth > tablayout.WidthRequest)
                move = tablayout.WidthRequest - phonewidth;
            if (move < 0)
                move = 0;
            if (tabscontent.Position == TabsContentList.Count - 1)
            {
                move = tablayout.WidthRequest - phonewidth;
            }
            if (tabscontent.Position == 0)
            {
                move = 0;
            }
            await scrollbar.ScrollToAsync(move, 0, true);
            SelectedTab = tabscontent.Position;
            lastindex = newindex;
        }
        private void Btn_Clicked(object sender, EventArgs e)
        {                     
            tabscontent.Position = tablayout.Children.IndexOf((sender as Button).Parent as Frame);
        }
        void OnTabItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //   tabscontent.ItemsSource = TabItems.Where(t => t.TabContent != null).ToList();
            var tabindex = SelectedTab;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    TabsContentList.Add(new TabsItem.TabsView { Content = (item as TabViewItem).TabContent });
                }
                double scrlwidth = 0;
                tablayout.Children.Clear();
                foreach (var item in TabItems)
                {
                    Frame frm = new Frame { BackgroundColor = Color.Transparent, WidthRequest = item.WidthRequest, Padding = new Thickness(0, 0, 0, 0), BindingContext = item };
                    Button btn = new Button { Text = item.Title, CornerRadius = 0, WidthRequest = item.WidthRequest, TextColor = item.TextColor, BackgroundColor = item.TabBackgroundColor };
                    frm.Content = btn;
                    tablayout.Children.Add(frm);
                    btn.Clicked += Btn_Clicked;
                    scrlwidth += item.WidthRequest;

                }
                foreach (var item in tablayout.Children)
                {
                    (item as Frame).Padding = new Thickness(0);
                }

                try
                {
                    var phonewidth = Application.Current.MainPage.Width;
                    SelectedTab = tabindex;
                    var move = (tablayout.Children[tabscontent.Position] as Frame).X;
                    try
                    {
                        move -= ((tablayout.Children[tabscontent.Position - 1] as Frame).WidthRequest / 2);
                    }
                    catch (Exception)
                    {

                    }
                    if (move + phonewidth > tablayout.WidthRequest)
                        move = tablayout.WidthRequest - phonewidth;
                    if (move < 0)
                        move = 0;
                    if (tabscontent.Position == TabsContentList.Count - 1)
                    {
                        move = tablayout.WidthRequest - phonewidth;
                    }
                    if (tabscontent.Position == 0)
                    {
                        move = 0;
                    }
                    scrollbar.ScrollToAsync(move, 0, true);
                    (tablayout.Children[tabscontent.Position] as Frame).Padding = new Thickness(0, 0, 0, 3);
                }
                catch (Exception ex)
                {
                    (tablayout.Children[tabscontent.Position] as Frame).Padding = new Thickness(0, 0, 0, 3);

                }
                scrollbar.WidthRequest = scrlwidth;
                tablayout.WidthRequest = scrlwidth;

            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    
                    TabsContentList.Remove(new TabsItem.TabsView { Content = (item as TabViewItem).TabContent });
                }
                tabscontent.ItemsSource = TabsContentList;
                double scrlwidth = 0;
                tablayout.Children.Clear();
                foreach (var item in TabItems)
                {
                    Frame frm = new Frame { BackgroundColor = item.SelectedTabBackgroundColor, WidthRequest = item.WidthRequest, Padding = new Thickness(0, 0, 0, 0) };
                    Button btn = new Button { Text = item.Title, CornerRadius = 0, WidthRequest = item.WidthRequest, TextColor = item.TextColor, BackgroundColor = item.TabBackgroundColor };
                    frm.Content = btn;
                    tablayout.Children.Add(frm);
                    btn.Clicked += Btn_Clicked;
                    scrlwidth += item.WidthRequest;

                }
                foreach (var item in tablayout.Children)
                {
                    (item as Frame).Padding = new Thickness(0);
                }

                try
                {
                    var phonewidth = Application.Current.MainPage.Width;
                    SelectedTab = tabindex;
                    var move = (tablayout.Children[tabscontent.Position] as Frame).X;
                    try
                    {
                        move -= ((tablayout.Children[tabscontent.Position - 1] as Frame).WidthRequest / 2);
                    }
                    catch (Exception)
                    {

                    }
                    if (move + phonewidth > tablayout.WidthRequest)
                        move = tablayout.WidthRequest - phonewidth;
                    if (move < 0)
                        move = 0;
                    if (tabscontent.Position == TabsContentList.Count - 1)
                    {
                        move = tablayout.WidthRequest - phonewidth;
                    }
                    if (tabscontent.Position == 0)
                    {
                        move = 0;
                    }
                    scrollbar.ScrollToAsync(move, 0, true);
                    (tablayout.Children[tabscontent.Position] as Frame).Padding = new Thickness(0, 0, 0, 3);
                }
                catch (Exception ex)
                {
                    (tablayout.Children[tabscontent.Position] as Frame).Padding = new Thickness(0, 0, 0, 3);

                }
                scrollbar.WidthRequest = scrlwidth;
                tablayout.WidthRequest = scrlwidth;
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                TabsContentList.Clear();
                foreach (var item in TabItems.Where(i => i.TabContent != null))
                {
                    TabsContentList.Add(new TabsItem.TabsView { Content = (item as TabViewItem).TabContent });

                }
                tabscontent.ItemsSource = TabsContentList;
                double scrlwidth = 0;
                for (int i =0;i< TabItems.Count;i++)
                {
                    (tablayout.Children[i] as Frame).BackgroundColor = TabItems[i].SelectedTabBackgroundColor;
                    (tablayout.Children[i] as Frame).WidthRequest = TabItems[i].WidthRequest;
                    (tablayout.Children[i] as Frame).Padding = new Thickness(0, 0, 0, 0);
                    (tablayout.Children[i] as Frame).BackgroundColor = TabItems[i].SelectedTabBackgroundColor;
                    ((tablayout.Children[i] as Frame).Content as Button).Text = TabItems[i].Title;
                    ((tablayout.Children[i] as Frame).Content as Button).WidthRequest = TabItems[i].WidthRequest;
                    ((tablayout.Children[i] as Frame).Content as Button).TextColor = TabItems[i].TextColor;
                    ((tablayout.Children[i] as Frame).Content as Button).BackgroundColor = TabItems[i].TabBackgroundColor;                    
                    scrlwidth += TabItems[i].WidthRequest;

                }              
                try
                {
                    var phonewidth = Application.Current.MainPage.Width;
                    SelectedTab = tabindex;
                    var move = (tablayout.Children[tabscontent.Position] as Frame).X;
                    try
                    {
                        move -= ((tablayout.Children[tabscontent.Position - 1] as Frame).WidthRequest / 2);
                    }
                    catch (Exception)
                    {

                    }
                    if (move + phonewidth > tablayout.WidthRequest)
                        move = tablayout.WidthRequest - phonewidth;
                    if (move < 0)
                        move = 0;
                    if (tabscontent.Position == TabsContentList.Count - 1)
                    {
                        move = tablayout.WidthRequest - phonewidth;
                    }
                    if (tabscontent.Position == 0)
                    {
                        move = 0;
                    }
                    scrollbar.ScrollToAsync(move, 0, true);
                    (tablayout.Children[tabscontent.Position] as Frame).Padding = new Thickness(0, 0, 0, 3);
                }
                catch (Exception ex)
                {
                    (tablayout.Children[tabscontent.Position] as Frame).Padding = new Thickness(0, 0, 0, 3);

                }
                scrollbar.WidthRequest = scrlwidth;
                tablayout.WidthRequest = scrlwidth;
            }
       
        }

        public void RemoveTab(TabViewItem tabitem)
        {
            TabItems.Remove(tabitem);
            tabscontent.ItemsSource = TabItems.Where(t => t.TabContent != null).ToList() ;
            tabscontent.Position = 1;
            double scrlwidth = 0;
            tablayout.Children.Clear();
            foreach (var item in TabItems)
            {
                Frame frm = new Frame { BackgroundColor = item.SelectedTabBackgroundColor, WidthRequest = item.WidthRequest, Padding = new Thickness(0, 0, 0, 0) };
                Button btn = new Button { Text = item.Title, CornerRadius = 0, WidthRequest = item.WidthRequest, TextColor = item.TextColor, BackgroundColor = item.TabBackgroundColor };
                frm.Content = btn;
                tablayout.Children.Add(frm);
                btn.Clicked += Btn_Clicked;
                scrlwidth += item.WidthRequest;

            }
            scrollbar.WidthRequest = scrlwidth;
            tablayout.WidthRequest = scrlwidth;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "SwipeEnable")
            {
                tabscontent.IsSwipeEnabled = SwipeEnable;
            }
            if (propertyName == "SelectedTab")
            {
                tabscontent.Position = SelectedTab;
            }
            if (propertyName == "TabBarPosition")
            {
                switch (TabBarPosition)
                {
                    case TabPosition.Left:
                        mainview.ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = new GridLength(TabbarHeight) }, new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } };
                        scrollbar.Orientation = ScrollOrientation.Vertical;
                        tablayout.Orientation = StackOrientation.Vertical;
                        mainview.Children.Add(scrollbar, 0, 0);
                        mainview.Children.Add(tabscontent, 1, 0); ;

                        break;
                    case TabPosition.Bottom:
                        mainview.RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, new RowDefinition { Height = new GridLength(TabbarHeight) } };
                        mainview.Children.Add(scrollbar, 0, 1);
                        mainview.Children.Add(tabscontent, 0, 0);
                        break;
                    case TabPosition.Right:
                        mainview.ColumnDefinitions = new ColumnDefinitionCollection { new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, new ColumnDefinition { Width = new GridLength(TabbarHeight) } };
                        scrollbar.Orientation = ScrollOrientation.Vertical;
                        tablayout.Orientation = StackOrientation.Vertical;
                        mainview.Children.Add(scrollbar, 1, 0);
                        mainview.Children.Add(tabscontent, 0, 0);
                        break;
                    case TabPosition.Top:
                        mainview.RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = new GridLength(TabbarHeight) }, new RowDefinition { Height = new GridLength(1, GridUnitType.Star) } };
                        mainview.Children.Add(scrollbar, 0, 0);
                        mainview.Children.Add(tabscontent, 0, 1);
                        break;
                    default:
                        mainview.RowDefinitions = new RowDefinitionCollection { new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }, new RowDefinition { Height = new GridLength(TabbarHeight) } };
                        break;
                }

            }
        }

        public enum TabPosition
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

    }
}