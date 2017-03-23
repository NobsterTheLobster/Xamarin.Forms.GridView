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

        object _selectedItem;

        bool _isPaneOpen;

        /// <summary>
        /// 
        /// </summary>
        MasterBehavior _displayMode;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// You can use observable collections
        /// </summary>
        List<string> _pages;

        //Or colors
        _ObservableCollection<object> _colors = new _ObservableCollection<object>();

        _ObservableCollection<object> _colorsGrouped = new _ObservableCollection<object>();

        string _currentPage;

        Command _tappedCommand;

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
            _pages = new List<string>(new string[] { "Sample1", "Sample2" });

            //Set the colors data source.
            _colors.ReplaceRange(typeof(Xamarin.Forms.Color).GetRuntimeFields().Skip(9).Where(r=>r.Name != "Transparent").Select((r, index) => new ColorGroup()
            {
                Color = r.Name.ToString(),
                GroupId = (int)Math.Ceiling(index / 10D)
            }));

            //Group the colours.
            _colorsGrouped.ReplaceRange(_colors.Cast<ColorGroup>().GroupBy(r => r.GroupId));
        }

        #endregion

        #region Methods

        public async Task Tapped(object item)
        {
           await _actionSheetDisplayer?.DisplayAlert("Hello", "You tapped an item:" + (item as ColorGroup).Color, "Clear");
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
