using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using XamarinFormsGridView.Controls;
using XamarinFormsGridView.iOS.Renderers;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using System.Reflection;

[assembly: ExportRenderer (typeof(GridView), typeof(GridViewRenderer))]
namespace XamarinFormsGridView.iOS.Renderers
{
    #region GridViewRenderer

    /// <summary>
    /// Class GridViewRenderer.
    /// </summary>
    public class GridViewRenderer : ViewRenderer<GridView, GridCollectionView>, IGridViewProvider
    {
        #region Fields

        /// <summary>
        /// Instance of navtive view.
        /// </summary>
        GridCollectionView _gridCollectionView;

        /// <summary>
        /// Starting index for scrolling events.
        /// </summary>
        int? _initialIndex;

        /// <summary>
        /// The _data source
        /// </summary>
        private GridDataSource _dataSource;


        NSString cellId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewRenderer"/> class.
        /// </summary>l
        public GridViewRenderer()
        {
        }

        #endregion

        #region PaddingStuff

        //bool _isPaddingInvalid;

        //bool IsPaddingInvalid
        //      { 
        //	get { return _isPaddingInvalid; }
        //	set { _isPaddingInvalid = value; }
        //}

        //void InvalidatePadding ()
        //{
        //	IsPaddingInvalid = true;
        //	SetNeedsLayout ();
        //}

        //nfloat _previousWidth;

        //public override void LayoutSubviews ()
        //{
        //	base.LayoutSubviews ();
        //	_gridCollectionView.Frame = this.Bounds;
        //	bool widthChanged = _previousWidth != _gridCollectionView.Frame.Width;
        //	if (widthChanged) {
        //		_previousWidth = _gridCollectionView.Frame.Width;
        //	}
        //	if (IsPaddingInvalid || widthChanged) {
        //		UpdatePadding ();
        //	}
        //}

        /// <summary>
        /// Updates the padding for when we center the content in the gridview
        /// </summary>
        //void UpdatePadding ()
        //{
        //	if (Element == null || (ICollection)Element.ItemsSource == null) {
        //		return;
        //	}

        //          //Get the total number of items.
        //	var numberOfItems = ((ICollection)Element.ItemsSource).Count;

        //	UICollectionViewFlowLayout flowLayout = _gridCollectionView != null ? (UICollectionViewFlowLayout)_gridCollectionView.CollectionViewLayout : null;

        //	if (flowLayout != null)
        //          {
        //              _gridCollectionView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
        //		flowLayout.SectionInset = new UIEdgeInsets (0, 0, 0, 0);

        //		if (_gridCollectionView.Frame.Width > 0 && _gridCollectionView.Frame.Height > 0)
        //              {
        //			IsPaddingInvalid = false;
        //		}
        //	}
        //}

        #endregion

        #region Properties


        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        private GridDataSource DataSource
        {
            get
            {
                return _dataSource ??
                (_dataSource =
                        new GridDataSource(GetCell, RowsInSection, ItemSelected));
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<GridView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null)
            {
                return;
            }
            e.NewElement.GridViewProvider = this;

            _gridCollectionView = new GridCollectionView();
            _gridCollectionView.AllowsMultipleSelection = false;
            _gridCollectionView.SelectionEnable = true;
            _gridCollectionView.BackgroundColor = Element.BackgroundColor.ToUIColor();

            //Unbox the collection view layout manager.
            UICollectionViewFlowLayout flowLayout = (UICollectionViewFlowLayout)_gridCollectionView.CollectionViewLayout;

            //Remove any section or content insets.
            _gridCollectionView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            flowLayout.SectionInset = new UIEdgeInsets(0, 0, 0, 0);

            //Remove event handling..
            Unbind(e.OldElement);

            //Add event handling.
            Bind(e.NewElement);

            //Set the data source.
            _gridCollectionView.Source = (e.NewElement.ItemsSource != null) ? DataSource : null;
            _gridCollectionView.Delegate = new GridViewDelegate(ItemSelected, HandleOnScrolled);

            //Scroll to first item.
            ScrollToInitialIndex();


            //			UpdatePadding ();
            //InvalidatePadding();

            SetNativeControl(_gridCollectionView);
        }

        /// <summary>
        /// Set the item size according to the min item width property.
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void ElementSizeChanged(object sender, EventArgs e)
        {
            //Unbox the grid view.
            var gridView = sender as GridView;

            //Work out how many items can fit.
            var canfit = (int)Math.Max(1, Math.Floor(gridView.Width / gridView.MinItemWidth));

            //Stretch the width to fill row.
            var actualWidth = gridView.Width / canfit;

            //Set the item size accordingly.
            _gridCollectionView.ItemSize = new CoreGraphics.CGSize(actualWidth, (float)gridView.RowHeight);
        }

        /// <summary>
        /// Raises the element property changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var gridView = sender as GridView;

            if (e.PropertyName == "ItemsSource")
            {
                if (gridView.ItemsSource != null)
                {
                    _gridCollectionView.Source = DataSource;
                    ReloadData();
                    ScrollToInitialIndex();
                }
            }
            //else if (e.PropertyName == "MinItemWidth")
            //         {

            //             //Get the span count.
            //             //var canfit = (int)Math.Max(1, Math.Floor(gridView.Width / gridView.MinItemWidth));

            //             //var actualWidth = gridView.Width / canfit;

            //             _gridCollectionView.ItemSize = new CoreGraphics.CGSize(actualWidth, (float)gridView.RowHeight);

            //}
        }

        /// <summary>
        /// Unbinds the specified old element.
        /// </summary>
        /// <param name="oldElement">The old element.</param>
        private void Unbind(GridView oldElement)
        {
            if (oldElement != null)
            {
                oldElement.PropertyChanging -= ElementPropertyChanging;
                oldElement.PropertyChanged -= ElementPropertyChanged;
                oldElement.SizeChanged -= ElementSizeChanged;
                var itemsSource = oldElement.ItemsSource as INotifyCollectionChanged;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged -= DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Binds the specified new element.
        /// </summary>
        /// <param name="newElement">The new element.</param>
        private void Bind(GridView newElement)
        {
            if (newElement != null)
            {
                newElement.PropertyChanging += ElementPropertyChanging;
                newElement.PropertyChanged += ElementPropertyChanged;
                newElement.SizeChanged += ElementSizeChanged;
                if (newElement.ItemsSource is INotifyCollectionChanged)
                {
                    (newElement.ItemsSource as INotifyCollectionChanged).CollectionChanged += DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Elements the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
            {
                var itemsSource = Element != null ? Element.ItemsSource as INotifyCollectionChanged : null;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged -= DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Elements the property changing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangingEventArgs"/> instance containing the event data.</param>
        private void ElementPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
            {
                var itemsSource = Element != null ? Element.ItemsSource as INotifyCollectionChanged : null;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged += DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Datas the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //try
            //{
            if (Control != null)
            {
                ReloadData();
            }
            //}
            //catch (Exception ex)
            //{
            //   // Console.WriteLine("error " + ex.Message);
            //}
        }


        void HandleOnScrolled(CGPoint contentOffset)
        {
            foreach (GridViewCell nativeCell in _gridCollectionView.VisibleCells)
            {
                nativeCell.ViewCell.OnScroll(contentOffset.ToPoint(), new Xamarin.Forms.Point(nativeCell.Frame.X, nativeCell.Frame.Y));
            }
            Element.RaiseOnScroll(0, (float)contentOffset.Y);
        }

        void ScrollToInitialIndex()
        {
            if (_initialIndex.HasValue && _gridCollectionView != null && _gridCollectionView.DataSource != null)
            {
                ScrollToItemWithIndex(_initialIndex.Value, false);
                _initialIndex = null;
            }
        }

        /// <summary>
        /// Rowses the in section.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="section">The section.</param>
        /// <returns>System.Int32.</returns>
        public int RowsInSection(UICollectionView collectionView, nint section)
        {
            //			var property = Element.ItemsSource.GetType ().GetProperty ("InstanceId");
            //			string instanceId = property?.GetValue (Element.ItemsSource)?.ToString ();
            //			Console.WriteLine (">>>>> countfrom  collection {0} is {1}", instanceId, ((ICollection)Element.ItemsSource).Count);
            var numberOfItems = ((ICollection)Element.ItemsSource).Count;
            return numberOfItems;
        }

        /// <summary>
        /// Items the selected.
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="indexPath">The index path.</param>
        public void ItemSelected(UICollectionView tableView, NSIndexPath indexPath)
        {
            var item = Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
            Element.InvokeItemSelectedEvent(this, item);
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="indexPath">Index path.</param>
        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            cellId = cellId ?? new NSString(GridViewCell.Key);
            var item = Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
            var collectionCell = collectionView.DequeueReusableCell(cellId, indexPath) as GridViewCell;

            collectionCell.RecycleCell(item, Element.ItemTemplate, Element);
            return collectionCell;
        }

        /// <summary>
        /// Reloads the data.
        /// </summary>
        public void ReloadData()
        {
            if (_gridCollectionView != null)
            {
                InvokeOnMainThread(() =>
                {
                    //UpdatePadding ();
                    _gridCollectionView.ReloadData();
                    _gridCollectionView.Delegate = new GridViewDelegate(ItemSelected, HandleOnScrolled);
                }
                );
            }
        }

        //TODO this method/mechanism needs some more thought
        /// <summary>
        /// Scrolls the index of the to item with.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public void ScrollToItemWithIndex(int index, bool animated)
        {
            if (_gridCollectionView != null && _gridCollectionView.NumberOfItemsInSection(0) > index)
            {
                var indexPath = NSIndexPath.FromRowSection(index, 0);
                InvokeOnMainThread(() =>
                {
                    _gridCollectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.Top, animated);
                });
            }
            else
            {
                _initialIndex = index;
            }
        }

        #endregion
    }

    #endregion

    #region GridCollectionView

    /// <summary>
    /// Class GridCollectionView.
    /// </summary>
    public class GridCollectionView : UICollectionView
    {
        /// <summary>
        /// Gets or sets a value indicating whether [selection enable].
        /// </summary>
        /// <value><c>true</c> if [selection enable]; otherwise, <c>false</c>.</value>
        public bool SelectionEnable
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridCollectionView"/> class.
        /// </summary>
        public GridCollectionView() : this(default(CGRect))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridCollectionView"/> class.
        /// </summary>
        /// <param name="frm">The FRM.</param>
        public GridCollectionView(CGRect frm) : base(default(CGRect), new UICollectionViewFlowLayout() { })
        {
            AutoresizingMask = UIViewAutoresizing.All;
            ContentMode = UIViewContentMode.ScaleToFill;
            RowSpacing = 0;
            ColumnSpacing = 0;
            RegisterClassForCell(typeof(GridViewCell), new NSString(GridViewCell.Key));

        }

        /// <summary>
        /// Cells for item.
        /// </summary>
        /// <param name="indexPath">The index path.</param>
        /// <returns>UICollectionViewCell.</returns>
        public override UICollectionViewCell CellForItem(NSIndexPath indexPath)
        {
            if (indexPath == null)
            {
                return null;
            }
            return base.CellForItem(indexPath);
        }

        /// <summary>
        /// Draws the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public override void Draw(CGRect rect)
        {
            CollectionViewLayout.InvalidateLayout();

            base.Draw(rect);
        }

        /// <summary>
        /// Gets or sets the row spacing.
        /// </summary>
        /// <value>The row spacing.</value>
        public double RowSpacing
        {
            get
            {
                return (double)(CollectionViewLayout as UICollectionViewFlowLayout).MinimumLineSpacing;
            }
            set
            {
                (CollectionViewLayout as UICollectionViewFlowLayout).MinimumLineSpacing = (float)value;
            }
        }

        /// <summary>
        /// Gets or sets the column spacing.
        /// </summary>
        /// <value>The column spacing.</value>
        public double ColumnSpacing
        {
            get
            {
                return (double)(CollectionViewLayout as UICollectionViewFlowLayout).MinimumInteritemSpacing;
            }
            set
            {
                (CollectionViewLayout as UICollectionViewFlowLayout).MinimumInteritemSpacing = (float)value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the item.
        /// </summary>
        /// <value>The size of the item.</value>
        public CGSize ItemSize
        {
            get
            {
                return (CollectionViewLayout as UICollectionViewFlowLayout).ItemSize;
            }
            set
            {
                (CollectionViewLayout as UICollectionViewFlowLayout).ItemSize = value;
            }
        }
    }

    #endregion

    #region GridViewCell

    /// <summary>
	/// Class GridViewCell.
	/// </summary>
	public class GridViewCell : UICollectionViewCell
    {
        UIView _view;
        object _originalBindingContext;
        FastGridCell _viewCell;

        private static PropertyInfo _platform;
        public static PropertyInfo PlatformProperty
        {
            get
            {
                return _platform ?? (
                    _platform = typeof(Element).GetProperty("Platform", BindingFlags.NonPublic | BindingFlags.Instance));
            }
        }

        public FastGridCell ViewCell { get { return _viewCell; } }

        public void RecycleCell(object data, DataTemplate dataTemplate, VisualElement parent)
        {
            if (_viewCell == null)
            {


                _viewCell = (dataTemplate.CreateContent() as FastGridCell);


                //reflection method of setting isplatformenabled property
                // We are going to re - set the Platform here because in some cases (headers mostly) its possible this is unset and
                //   when the binding context gets updated the measure passes will all fail.By applying this here the Update call
                // further down will result in correct layouts.
                var p = PlatformProperty.GetValue(parent);
                PlatformProperty.SetValue(_viewCell, p);

                _viewCell.BindingContext = data;
                _viewCell.Parent = parent;
                _viewCell.PrepareCell(new Size(Bounds.Width, Bounds.Height));
                _originalBindingContext = _viewCell.BindingContext;
                var renderer = Platform.CreateRenderer(_viewCell.View); //RendererFactory.GetRenderer (_viewCell.View);
                _view = renderer.NativeView;
                _view.AutoresizingMask = UIViewAutoresizing.All;
                _view.ContentMode = UIViewContentMode.ScaleToFill;
                ContentView.AddSubview(_view);
                return;
            }
            else if (data == _originalBindingContext)
            {
                _viewCell.BindingContext = _originalBindingContext;
            }
            else
            {
                _viewCell.BindingContext = data;
            }
        }

        /// <summary>
        /// The key
        /// </summary>
        public const string Key = "GridViewCell";

        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewCell"/> class.
        /// </summary>
        /// <param name="frame">The frame.</param>
        [Export("initWithFrame:")]
        public GridViewCell(CGRect frame) : base(frame)
        {
            // SelectedBackgroundView = new GridItemSelectedViewOverlay (frame);
            // this.BringSubviewToFront (SelectedBackgroundView);
           // BackgroundColor = UIColor.Black;

        }

        CGSize _lastSize;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (_lastSize.Equals(CGSize.Empty) || !_lastSize.Equals(Frame.Size))
            {

                _viewCell.View.Layout(Frame.ToRectangle());
                _viewCell.OnSizeChanged(new Xamarin.Forms.Size(Frame.Size.Width, Frame.Size.Height));
                _lastSize = Frame.Size;
            }

            _view.Frame = ContentView.Bounds;
        }
    }

    #endregion

    #region GridDataSource

    /// <summary>
	/// Class GridDataSource.
	/// </summary>
	public class GridDataSource : UICollectionViewSource
    {
        /// <summary>
        /// Delegate OnGetCell
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="indexPath">The index path.</param>
        /// <returns>UICollectionViewCell.</returns>
        public delegate UICollectionViewCell OnGetCell(UICollectionView collectionView, NSIndexPath indexPath);

        /// <summary>
        /// Delegate OnRowsInSection
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="section">The section.</param>
        /// <returns>System.Int32.</returns>
        public delegate int OnRowsInSection(UICollectionView collectionView, nint section);

        /// <summary>
        /// Delegate OnItemSelected
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="indexPath">The index path.</param>
        public delegate void OnItemSelected(UICollectionView collectionView, NSIndexPath indexPath);

        /// <summary>
        /// The _on get cell
        /// </summary>
        private readonly OnGetCell _onGetCell;
        /// <summary>
        /// The _on rows in section
        /// </summary>
        private readonly OnRowsInSection _onRowsInSection;
        /// <summary>
        /// The _on item selected
        /// </summary>
        private readonly OnItemSelected _onItemSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataSource"/> class.
        /// </summary>
        /// <param name="onGetCell">The on get cell.</param>
        /// <param name="onRowsInSection">The on rows in section.</param>
        /// <param name="onItemSelected">The on item selected.</param>
        public GridDataSource(OnGetCell onGetCell, OnRowsInSection onRowsInSection, OnItemSelected onItemSelected)
        {
            _onGetCell = onGetCell;
            _onRowsInSection = onRowsInSection;
            _onItemSelected = onItemSelected;
        }

        #region implemented abstract members of UICollectionViewDataSource

        /// <summary>
        /// Gets the items count.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="section">The section.</param>
        /// <returns>System.Int32.</returns>
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return _onRowsInSection(collectionView, section);
        }

        /// <summary>
        /// Items the selected.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="indexPath">The index path.</param>
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            _onItemSelected(collectionView, indexPath);
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="indexPath">The index path.</param>
        /// <returns>UICollectionViewCell.</returns>
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell = _onGetCell(collectionView, indexPath);
            if ((collectionView as GridCollectionView).SelectionEnable)
            {
                cell.AddGestureRecognizer(new UITapGestureRecognizer((v) => {
                    ItemSelected(collectionView, indexPath);
                }));
            }
            else
                cell.SelectedBackgroundView = new UIView();

            return cell;
        }

        #endregion
    }

    #endregion

    #region GridViewDelegate

    /// <summary>
    /// Class GridViewDelegate.
    /// </summary>
    public class GridViewDelegate : UICollectionViewDelegate
    {
        /// <summary>
        /// Delegate OnItemSelected
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="indexPath">The index path.</param>
        public delegate void OnItemSelected(UICollectionView tableView, NSIndexPath indexPath);

        public delegate void OnScrolled(CGPoint contentOffset);

        /// <summary>
        /// The _on item selected
        /// </summary>
        private readonly OnItemSelected _onItemSelected;
        private readonly OnScrolled _onScrolled;


        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewDelegate"/> class.
        /// </summary>
        /// <param name="onItemSelected">The on item selected.</param>
        public GridViewDelegate(OnItemSelected onItemSelected, OnScrolled onScrolled)
        {
            _onItemSelected = onItemSelected;
            _onScrolled = onScrolled;
        }

        /// <summary>
        /// Items the selected.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="indexPath">The index path.</param>
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            _onItemSelected(collectionView, indexPath);
        }

        /// <summary>
        /// Items the highlighted.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="indexPath">The index path.</param>
        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _onScrolled(scrollView.ContentOffset);
        }

    }

    #endregion
}