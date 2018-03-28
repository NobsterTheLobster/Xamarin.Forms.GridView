using System;
using System.Collections;
using Xamarin.Forms;
using System.Diagnostics;
using System.Windows.Input;

namespace Plugin.GridViewControl.Common
{
    #region IGridViewProvider

    /// <summary>
    /// Interface for providing scroll access to underlying grid control
    /// </summary>
    public interface IGridViewProvider
	{
        /// <summary>
        /// Repopulate the grid view with items.
        /// </summary>
		void ReloadData ();

        /// <summary>
        /// Scroll to the item at the specified index.
        /// </summary>
        /// <param name="index">The index to scroll to.</param>
        /// <param name="animated">Whether the scrolling should be animated.</param>
		void ScrollToItemWithIndex (int index, bool animated);
	}

    #endregion

    #region ControlScrollEventArgs

    /// <summary>
    /// Arguments for a scroll event on the gridview.
    /// </summary>
    public class ControlScrollEventArgs : EventArgs
    {
        /// <summary>
        /// The delta.
        /// </summary>
        public float Delta { get; set; }

        /// <summary>
        /// The current vertical position.
        /// </summary>
        public float CurrentY { get; set; }

        /// <summary>
        /// Initialize a new instance of the ControlScrollEventArgs
        /// </summary>
        /// <param name="delta">The delta</param>
        /// <param name="currentY">The current vertical position.</param>
        public ControlScrollEventArgs(float delta, float currentY)
        {
            this.Delta = delta;
            this.CurrentY = currentY;
        }
    }

    #endregion

    #region IScrollAwareElement

    /// <summary>
    /// Interface for providing scrol awareness.
    /// </summary>
    public interface IScrollAwareElement
    {
        /// <summary>
        /// When the scrolling begins.
        /// </summary>
        event EventHandler OnStartScroll;

        /// <summary>
        /// When the scrolling ends.
        /// </summary>
        event EventHandler OnStopScroll;

        /// <summary>
        /// When the list is scrolled.
        /// </summary>
        event EventHandler<ControlScrollEventArgs> OnScroll;

        /// <summary>
        /// Raise the on scroll event.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="currentY">The current position.</param>
        void RaiseOnScroll(float delta, float currentY);

        /// <summary>
        /// Raise the start scroll event.
        /// </summary>
        void RaiseOnStartScroll();

        /// <summary>
        /// Raise the stop scroll event.
        /// </summary>
        void RaiseOnStopScroll();
    }

    #endregion

    #region GridView

    /// <summary>
    /// Class GridView.
    /// </summary>
    public class GridView : ListView, IScrollAwareElement
	{
        #region Fields

        int? _initialIndex;

        IGridViewProvider _gridViewProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridView"/> class.
        /// </summary>
        public GridView ()
		{
            //HasUnevenRows = false;
		}

        #endregion

        #region Bindable Properties

        /// <summary>
        /// Command for when a child item is pressed.
        /// </summary>
        public static readonly BindableProperty TappedCommandProperty =
          BindableProperty.CreateAttached(
              "TappedCommand",
              typeof(ICommand),
              typeof(GridView),
              null,
              propertyChanged: OnTappedCommandPropertyChanged);

        /// <summary>
        /// Gets the tapped command on the specified view.
        /// </summary>
        /// <param name="view">The view to retrieve the property from.</param>
        /// <returns>The tapped command from the specified view.</returns>
        public static ICommand GetTappedCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(TappedCommandProperty);
        }

        /// <summary>
        /// Sets the tapped command on the specified view.
        /// </summary>
        /// <param name="view">The view to set the property on.</param>
        /// <param name="value">The value of the property.</param>
        public static void SetTappedCommand(BindableObject view, bool value)
        {
            view.SetValue(TappedCommandProperty, value);
        }

        /// <summary>
        /// The item width property
        /// </summary>
        public static readonly BindableProperty MinItemWidthProperty =
            BindableProperty.Create(
                "MinItemWidth",
                typeof(double),
                typeof(GridView),
                (double)100D);

        /// <summary>
        /// Get the min item width from the specified view.
        /// </summary>
        /// <param name="view">The view to retrieve the property from.</param>
        /// <returns>The value of the min item width from the specified view.</returns>
        public static double GetMinItemWidth(BindableObject view)
        {
            return (double)view.GetValue(MinItemWidthProperty);
        }

        /// <summary>
        /// Sets the min item width on the specified view.
        /// </summary>
        /// <param name="view">the view to set the property on.</param>
        /// <param name="value">The value of the property.</param>
        public static void SetMinItemWidth(BindableObject view, bool value)
        {
            view.SetValue(MinItemWidthProperty, value);
        }

        #endregion

        #region CLR Accessors

        /// <summary>
        /// Gets or sets the tapped command.
        /// </summary>
        public ICommand TappedCommand
        {
            get
            {
                return (ICommand)GetValue(TappedCommandProperty);
            }
            set
            {
                SetValue(TappedCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the grid view provider.
        /// </summary>
		public IGridViewProvider GridViewProvider {
			get{ return _gridViewProvider; }
			set {
				_gridViewProvider = value;
				if (_initialIndex.HasValue) {
					GridViewProvider.ScrollToItemWithIndex (_initialIndex.Value, false);
					_initialIndex = null;
				}
			}
		}

   		/// <summary>
		/// Gets or sets the width of the item.
		/// </summary>
		/// <value>The width of the item.</value>
		public double MinItemWidth {
			get {
				return (double)base.GetValue (GridView.MinItemWidthProperty);
			}
			set {
				base.SetValue (GridView.MinItemWidthProperty, value);
			}
		}

        #endregion

        #region Methods

        /// <summary>
        /// Attach handling for list item tapped event.
        /// </summary>
        /// <param name="view">The source of the property change.</param>
        /// <param name="oldValue">The original value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        private static void OnTappedCommandPropertyChanged(BindableObject view, object oldValue, object newValue)
        {
            var list = view as ListView;

            if (list != null)
            {
                list.ItemTapped -= List_ItemTapped;
                list.ItemTapped += List_ItemTapped;
            }
        }

        /// <summary>
        /// Callback when an item in the list view is tapped.
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        static void List_ItemTapped(object sender, ItemTappedEventArgs e)
        {
           // Get object
           var listView = (sender as ListView);

            // Get command
            ICommand command = GetTappedCommand(listView);

            // Execute command
            command.Execute(e.Item);
        }

        /// <summary>
        /// Invokes the item selected event.
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="item">The arguments for the event.</param>
        public void InvokeItemSelectedEvent (object sender, object item)
		{
            //If this is not already the selected item.
            if (SelectedItem != item)
            {
                //Set the selected item property.
                SelectedItem = item;
            }

            //Fire the command
            TappedCommand?.Execute(item);
        }

        /// <summary>
        /// 
        /// </summary>
		public void ReloadData ()
		{
			if (GridViewProvider != null) {
				GridViewProvider.ReloadData ();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="animated"></param>
		public void ScrollToItemWithIndex (int index, bool animated)
		{
			if (GridViewProvider != null) {
				GridViewProvider.ScrollToItemWithIndex (index, animated);
			} else {
				_initialIndex = index;
			}
		}

        #endregion

        #region ISCrollAwareElement

        /// <summary>
        /// When scrolling begins.
        /// </summary>
        public event EventHandler OnStartScroll;

        /// <summary>
        /// When scrolling ends.
        /// </summary>
		public event EventHandler OnStopScroll;

        /// <summary>
        /// When the list is scrolled.
        /// </summary>
		public event EventHandler<ControlScrollEventArgs> OnScroll;

        /// <summary>
        /// Raise the on scroll event.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="currentY">The current position.</param>
		public void RaiseOnScroll (float delta, float currentY)
		{
			var args = new ControlScrollEventArgs (delta, currentY);
			if (OnScroll != null) {
				OnScroll (this, args);
			}
		}

        /// <summary>
        /// Raise the start scroll event.
        /// </summary>
		public void RaiseOnStartScroll ()
		{
			if (OnStartScroll != null) {
				OnStartScroll (this, new EventArgs ());
			}
		}

        /// <summary>
        /// Raise the stop scroll event.
        /// </summary>
		public void RaiseOnStopScroll ()
		{
			if (OnStopScroll != null) {
				OnStopScroll (this, new EventArgs ());
			}
		}

		#endregion
	}

    #endregion
}
