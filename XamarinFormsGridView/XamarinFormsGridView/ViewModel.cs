using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinFormsGridView.Interfaces;

namespace XamarinFormsGridView
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Stores the selected item.
        /// </summary>
        object _selectedItem;

        /// <summary>
        /// Indicates the open state of the master detail page.
        /// </summary>
        bool _isPaneOpen;

        /// <summary>
        /// Indicates the data is loading.
        /// </summary>
        bool _isRefreshing;

        /// <summary>
        /// Indicates the current display mode of the master detail page.
        /// </summary>
        MasterBehavior _displayMode;

        /// <summary>
        /// Property change event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// List of navigation pages.
        /// </summary>
        List<string> _pages;

        /// <summary>
        /// Colours source.
        /// </summary>
        _ObservableCollection<object> _colors = new _ObservableCollection<object>();

        /// <summary>
        /// Grouped source.
        /// </summary>
        _ObservableCollection<object> _colorsGrouped = new _ObservableCollection<object>();

        /// <summary>
        /// Grouped source.
        /// </summary>
        _ObservableCollection<object> _colorsAndOtherThings = new _ObservableCollection<object>();

        /// <summary>
        /// The current page.
        /// </summary>
        string _currentPage;

        /// <summary>
        /// The item tapped command.
        /// </summary>
        Command _tappedCommand, _refreshCommand, _holdCommand;

        /// <summary>
        /// Interface for displaying page alerts.
        /// </summary>
        IDisplayActionSheet _actionSheetDisplayer;

        #endregion

        #region Properties

        /// <summary>
        /// Command to open and item for edit.
        /// </summary>
        public Command TappedCommand
        {
            get
            {
                if (_tappedCommand == null)
                {
                    _tappedCommand = new Command(
                        async param => await Tapped(param)
                        );
                }
                return _tappedCommand;
            }
        }

        public Command HoldCommand
        {
            get
            {
                if (_holdCommand == null)
                {
                    _holdCommand = new Command(
                        async param => await Hold(param)
                        );
                }
                return _holdCommand;
            }
        }

        public Command RefreshCommand
        {
            get
            {
                //If the command is nothing.
                if (_refreshCommand == null)
                {
                    //Initialize new command.
                    _refreshCommand = new Command(
                        async param => await LoadData()
                        );
                }

                //Return the command.
                return _refreshCommand;
            }
        }

        /// <summary>
        /// Collection of navigation pages.
        /// </summary>
        public List<string> Pages
        {
            get { return _pages; }
            set
            {
                _pages = value;


            }
        }

        /// <summary>
        /// List of colours.
        /// </summary>
        public _ObservableCollection<object> Colours
        {
            get { return _colors; }
            set
            {
                //Set value.
                _colors = value;

                //Notify binding targets.
                NotifyPropertyChanged("Colours");
            }
        }

        /// <summary>
        /// List of colours.
        /// </summary>
        public _ObservableCollection<object> ColoursGrouped
        {
            get { return _colorsGrouped; }
            set
            {
                //Set value.
                _colorsGrouped = value;

                //Notify binding targets.
                NotifyPropertyChanged("ColoursGrouped");
            }
        }

        /// <summary>
        /// List of colours.
        /// </summary>
        public _ObservableCollection<object> ColoursAndOtherThings
        {
            get { return _colorsAndOtherThings; }
            set
            {
                //Set value.
                _colorsAndOtherThings = value;

                //Notify binding targets.
                NotifyPropertyChanged("ColoursAndOtherThings");
            }
        }

        /// <summary>
        /// Gets or sets whether the pane is open.
        /// </summary>
        public bool IsPaneOpen
        {

            get { return _isPaneOpen; }
            set
            {
                _isPaneOpen = value;
                NotifyPropertyChanged("IsPaneOpen");
            }
        }

        /// <summary>
        /// Gets the display mode to use for the split view.
        /// </summary>
        public MasterBehavior MenusDisplayMode
        {
            get
            {
                return _displayMode;
            }
            set
            {
                _displayMode = value;
                NotifyPropertyChanged("MenusDisplayMode");
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                //Notify binding targets.
                NotifyPropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// Gets or sets whether the component is refreshing.
        /// </summary>
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;

                //Notify binding targets.
                NotifyPropertyChanged("IsRefreshing");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;

                //If you have clicked an item in the list and if the menu is an overlay
                //then the pane must be closed.
                if (MenusDisplayMode == MasterBehavior.Popover || MenusDisplayMode == MasterBehavior.Default && Device.Idiom == TargetIdiom.Phone)
                {
                    //Close the menu.
                    IsPaneOpen = false;
                }

                NotifyPropertyChanged("CurrentPage");
            }
        }

        /// <summary>
        /// Gets or sets the interface to show action sheets and alerts.
        /// </summary>
        public IDisplayActionSheet ActionSheetDisplayer
        {
            get { return _actionSheetDisplayer; }
            set
            {
                //Set the action sheet displayer.
                _actionSheetDisplayer = value;

            }
        }

        #endregion

        #region Constructors

        public ViewModel()
        {
            //Initialize the pages collection
            _pages = new List<string>(new string[] { "Sample1", "Sample2", "Sample3" });

            //Load the data asynchronously.
            LoadData();
        }

        /// <summary>
        /// Load the data for the view model.
        /// </summary>
        private async Task LoadData()
        {
            //Set the colors data source.
            _colors.ReplaceRange(typeof(Xamarin.Forms.Color).GetRuntimeFields().Skip(9).Where(r => r.Name != "Transparent").Select((r, index) => new ColorGroup()
            {
                Color = r.Name.ToString(),
                GroupId = (int)Math.Ceiling(index / 10D)
            }));

            //Group the colours.
            _colorsGrouped.ReplaceRange(_colors.Cast<ColorGroup>().GroupBy(r => r.GroupId));

            var list = new List<object>();

            list.AddRange(_colors);
            list.AddRange(Enumerable.Repeat(new MyOtherObject(), 100));
            _colorsAndOtherThings.AddRange(list.OrderBy(r => Guid.NewGuid()));

            //Notification that refresh is complete.
            IsRefreshing = false;
        }

        #endregion

        #region Methods

        public async Task Tapped(object item)
        {
           await _actionSheetDisplayer?.DisplayAlert("Hello", "You tapped an item:" + (item as ColorGroup).Color, "Clear");
        }

        public async Task Hold(object item)
        {
            await _actionSheetDisplayer?.DisplayAlert("Hello", "You hold an item:" + (item as ColorGroup).Color, "Clear");
        }

        /// <summary>
        /// Checks that there is atleast one listener 
        /// attached before firing the PropertyChanged Event.
        /// </summary>
        /// <param name="info">The name of the property that has changed.</param>
        public void NotifyPropertyChanged(String info)
        {
            //If the event is not nothing.
            if (PropertyChanged != null)
            {
                //Fire event.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }

    public class MyOtherObject : INotifyPropertyChanged
    {
        string _text = "The quick brown fox jumps over the lazy dog";

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks that there is atleast one listener 
        /// attached before firing the PropertyChanged Event.
        /// </summary>
        /// <param name="info">The name of the property that has changed.</param>
        public void NotifyPropertyChanged(String info)
        {
            //If the event is not nothing.
            if (PropertyChanged != null)
            {
                //Fire event.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class ColorGroup : INotifyPropertyChanged
    {
        string _color;

        int _groupId;

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                NotifyPropertyChanged("Color");
            }
        }

        public int GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                NotifyPropertyChanged("GroupId");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks that there is atleast one listener 
        /// attached before firing the PropertyChanged Event.
        /// </summary>
        /// <param name="info">The name of the property that has changed.</param>
        public void NotifyPropertyChanged(String info)
        {
            //If the event is not nothing.
            if (PropertyChanged != null)
            {
                //Fire event.
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
  
}
