using System;
using Xamarin.Forms.Platform.Android;
using Android.Support.V7.Widget;
using Xamarin.Forms;
using Android.Content.Res;
using Android.Views;
using FormsGridView = Plugin.GridViewControl.Common.GridView;
using Android.Widget;
using Plugin.GridViewControl.Droid.Renderers;
using System.Reflection;
using System.Collections;
using Android.Util;
using System.Collections.Specialized;
using Android.Content;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics.Drawables;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Widget;

[assembly: ExportRenderer (typeof(FormsGridView), typeof(GridViewRenderer))]
namespace Plugin.GridViewControl.Droid.Renderers
{
    #region GridViewRenderer

    /// <summary>
    /// Renderer for GridView control on Android.
    /// </summary>
    public class GridViewRenderer : ViewRenderer<FormsGridView, SwipeRefreshLayout>
    {
        #region Fields

        readonly Android.Content.Res.Orientation _orientation = Android.Content.Res.Orientation.Undefined;

        ScrollRecyclerView _recyclerView;
        SwipeRefreshLayout _pullToRefresh;

        float _startEventY;
        float _heightChange;

        Context _context;
        GridLayoutManager _layoutManager;
        GridViewAdapter _adapter;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public GridViewRenderer(Context context) : base(context)
        {
            _context = context;
            AutoPackage = false;
        }

        #endregion

        #region Overides

        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (newConfig.Orientation != _orientation)
                OnElementChanged(new ElementChangedEventArgs<FormsGridView>(Element, Element));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FormsGridView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                DestroyRecyclerview();
                CreateRecyclerView(_context);
                SetNativeControl(_pullToRefresh);
                // after SetNativeControl, _pullToRefresh.Enabled was set the value of Element.IsEnabled;
                UpdateIsPullToRefreshEnabled();
            }


            //TODO unset
            //			this.Unbind (e.OldElement);
            //			this.Bind (e.NewElement);
        }
        
        private void UpdateIsPullToRefreshEnabled()
        {
            if(_pullToRefresh != null)
                _pullToRefresh.Enabled = Element.IsPullToRefreshEnabled;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "ItemsSource") {
                _adapter.Items = Element.ItemsSource;
            }
            //If the element IsRefreshing property is changing.
            else if (e.PropertyName == "IsRefreshing")
            {
                //Indicate whether the control is refreshing.
                _pullToRefresh.Refreshing = Element.IsRefreshing;
            }
            //If the element PullToRefresh property is changing.
            else if (e.PropertyName == "IsPullToRefreshEnabled")
            {
                //Indicate whether pull to refresh is enabled.
                _pullToRefresh.Enabled = Element.IsPullToRefreshEnabled;
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdateGridLayout();
        }

        #endregion

        #region Methods

        void DestroyRecyclerview()
        {
            if (_recyclerView != null)
            {
                //TODO
                _recyclerView.Touch -= _recyclerView_Touch;
            }
        }

        void CreateRecyclerView(Context context)
        {
           
            _recyclerView = new ScrollRecyclerView(context);
            _recyclerView.Touch += _recyclerView_Touch;
            var scrollListener = new GridViewScrollListener(Element, _recyclerView);
            _recyclerView.AddOnScrollListener(scrollListener);

            _recyclerView.SetItemAnimator(null);
            _recyclerView.HorizontalScrollBarEnabled = false;
            _recyclerView.VerticalScrollBarEnabled = true;

            _adapter = new GridViewAdapter(Element.ItemsSource, _recyclerView, Element, Resources.DisplayMetrics);
            _recyclerView.SetAdapter(_adapter);

            //Initialize the pull to refresh host.
            _pullToRefresh = new SwipeRefreshLayout(Android.App.Application.Context);

            //Add the recylcler to the refresh host.
            _pullToRefresh.AddView(_recyclerView);

            //Attach event handling for refresh.
            _pullToRefresh.Refresh += _pullToRefresh_Refresh;

            //Enable/Disable pull to refresh.
            _pullToRefresh.Enabled = Element.IsPullToRefreshEnabled;
            
            //Set the current refresh status.
            _pullToRefresh.Refreshing = Element.IsRefreshing;

            //Update the grid layout.
            UpdateGridLayout();
        }

        /// <summary>
        /// Call back to refresh the data.
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        void _pullToRefresh_Refresh(object sender, EventArgs e)
        {
            if (_pullToRefresh.Enabled)
            {
                //If there is a command associated.
                if (Element.RefreshCommand != null)
                {
                    //Call the command.
                    Element.IsRefreshing = true;
                    Element.RefreshCommand.Execute(null);
                }
                else
                {
                    //Indicate to the view we are no longer refreshing.
                    _pullToRefresh.Refreshing = false;
                }
            }
            else
            {
                //Indicate to the view we are no longer refreshing.
                _pullToRefresh.Refreshing = false;
            }
        }

        /// <summary>
        /// Update the grid layout manager instance.
        /// </summary>
        void UpdateGridLayout()
        {
            // Make sure that MinItemWidth is never 0.
            double minWidth = Element.MinItemWidth > 0 ? Element.MinItemWidth : 1;

            //Get the span count.
            var spanCount = (int)Math.Max(1, Math.Floor(Element.Width / minWidth));

            //I found that if I don't re-iniitalize a new layout manager 
            //each time the span count should change then the items don't render.
            _layoutManager = new GridLayoutManager(Context, spanCount);
            _layoutManager.SetSpanSizeLookup(new GroupedGridSpanSizeLookup(_recyclerView.GetAdapter() as GridViewAdapter, spanCount));

            _recyclerView.SetLayoutManager(_layoutManager);
        }

        #endregion

        #region Events

        void _recyclerView_Touch(object sender, TouchEventArgs e)
        {

            var ev = e.Event;
            MotionEventActions action = ev.Action & MotionEventActions.Mask;
            switch (action) {
                case MotionEventActions.Down:
                    _startEventY = ev.GetY();
                    _heightChange = 0;
                    Element.RaiseOnStartScroll();
                    break;
                case MotionEventActions.Move:
                    float delta = (ev.GetY() + _heightChange) - _startEventY;
                    Element.RaiseOnScroll(delta, _recyclerView.GetVerticalScrollOffset());
                    break;
                case MotionEventActions.Up:
                    Element.RaiseOnStopScroll();
                    break;
            }
            e.Handled = false;

        }

        #endregion
    }

    #endregion

    #region GroupedGridSpanSizeLookup

    public class GroupedGridSpanSizeLookup : GridLayoutManager.SpanSizeLookup
    {
        GridViewAdapter _adapter;
        int _spanCount;

        public GroupedGridSpanSizeLookup(GridViewAdapter adapter, int spanCount)
        {
            _adapter = adapter;
            _spanCount = spanCount;
        }

        public override int GetSpanSize(int position)
        {
            return _adapter.GetItemViewType(position) == 0 ? 1 : _spanCount;
        }
    }

    #endregion

    #region ScrollRecyclerView

    public class ScrollRecyclerView : RecyclerView
	{
		public ScrollRecyclerView (IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		}

		public ScrollRecyclerView (Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
		}

		public ScrollRecyclerView (Android.Content.Context context, Android.Util.IAttributeSet attrs) : base (context, attrs)
		{
		}

		public ScrollRecyclerView (Android.Content.Context context) : base (context)
		{
		}

		public int GetVerticalScrollOffset ()
		{
			return ComputeVerticalScrollOffset ();
		}

		public int GetHorizontalScrollOffset ()
		{
			return ComputeHorizontalScrollOffset ();
		}
	}

    #endregion

    #region GridViewScrollListener

    public class GridViewScrollListener : RecyclerView.OnScrollListener
	{
        FormsGridView _gridView;

		ScrollRecyclerView _recyclerView;

		public GridViewScrollListener (FormsGridView gridView, ScrollRecyclerView recyclerView)
		{
			_gridView = gridView;
			_recyclerView = recyclerView;
		}

		public override void OnScrolled (RecyclerView recyclerView, int dx, int dy)
		{
			base.OnScrolled (recyclerView, dx, dy);
			_gridView.RaiseOnScroll (dy, _recyclerView.GetVerticalScrollOffset ());
			//Console.WriteLine (">>>>>>>>> {0},{1}", dy, _recyclerView.GetVerticalScrollOffset ());
		}
	}

    #endregion

    #region GridViewAdapter

    public class GridViewAdapter : RecyclerView.Adapter, Android.Views.View.IOnLongClickListener
    {
        RecyclerView _recyclerView;
        IEnumerable _items;

        /// <summary>
        /// Holds the selected items.
        /// </summary>
        int _selectedItemPosition = -1;

        /// <summary>
        /// Holds a reference for each group and associated global index.
        /// </summary>
        Dictionary<int, int> _groupIndexDictionary = new Dictionary<int, int>();

        private static PropertyInfo _platform;
        public static PropertyInfo PlatformProperty
        {
            get
            {
                return _platform ?? (
                    _platform = typeof(Element).GetProperty("Platform", BindingFlags.NonPublic | BindingFlags.Instance) ??
                    typeof(Element).GetProperty("Platform", BindingFlags.Public | BindingFlags.Instance));
            }
        }

        DisplayMetrics _displayMetrics;

        FormsGridView _gridView;

        FormsGridView Element { get; set; }

        /// <summary>
        /// Contains the flatened collection of items.
        /// </summary>
        IEnumerable<object> _flattenedItems;

        public IEnumerable Items
        {
            get
            {
                return _items;
            }
            set
            {
                var oldColleciton = _items as INotifyCollectionChanged;
                if (oldColleciton != null)
                {
                    oldColleciton.CollectionChanged -= NewColleciton_CollectionChanged;
                }
                _items = value;
                var newColleciton = _items as INotifyCollectionChanged;
                if (newColleciton != null)
                {
                    newColleciton.CollectionChanged += NewColleciton_CollectionChanged;
                }
                NotifyDataSetChanged();
                PrepareGrouping();
            }
        }

        void NewColleciton_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    //TODO handle changes here, reload data etc.. if required.. 
                    //or brute force.. or something else
                    foreach (var item in e.NewItems)
                    {
                        var index = IndexOf(_items, item);
                        NotifyItemInserted(index);
                        
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    //If there are multiple items.
                    if (e.OldItems.Count > 1)
                    {
                        //Not very effiencent but there is no way to establish index 
                        //of unsequential removed items without maintaining an extra 
                        //virtual dataset which seems like more trouble then its worth.
                        NotifyDataSetChanged();
                    }
                    else
                    {
                        //Remove the single item.
                        NotifyItemRemoved(e.OldStartingIndex);
                    }
                }
            }
            else
            {
                NotifyDataSetChanged();

            }

            PrepareGrouping();
        }

        public GridViewAdapter(IEnumerable items, RecyclerView recyclerView, FormsGridView gridView, DisplayMetrics displayMetrics)
        {
            Items = items;
            _recyclerView = recyclerView;
            Element = gridView;
            _displayMetrics = displayMetrics;
            _gridView = gridView;
        }

        public class GridViewCell : RecyclerView.ViewHolder
        {
            public GridViewCellContainer ViewCellContainer { get; set; }

            public GridViewCell(GridViewCellContainer view) : base(view)
            {
                ViewCellContainer = view;
            }
        }

        public override int GetItemViewType(int position)
        {
            //Default type is item
            return _groupIndexDictionary.ContainsValue(position) ? 1 : 0;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //var selector = Element.ItemTemplate as DataTemplateSelector;
            //if (selector != null)
            //{
            //    selector.SelectTemplate(item, _listView);
            //}

            var gridViewCell = viewType == 0 ?
            Element.ItemTemplate.CreateContent() as Plugin.GridViewControl.Common.FastGridCell :
            Element.GroupHeaderTemplate.CreateContent() as Plugin.GridViewControl.Common.FastGridCell;

            // reflection method of setting isplatformenabled property
            // We are going to re-set the Platform here because in some cases (headers mostly) its possible this is unset and
            // when the binding context gets updated the measure passes will all fail. By applying this here the Update call
            // further down will result in correct layouts.
            var p = PlatformProperty.GetValue(Element);
            PlatformProperty.SetValue(gridViewCell, p);

            var initialCellSize = new Xamarin.Forms.Size(Element.MinItemWidth, Element.RowHeight);
            var view = new GridViewCellContainer(parent.Context, gridViewCell, parent, initialCellSize);

            //Only add the click handler for items (not group headers.)
            if (viewType == 0)
            {
                view.Click += MMainView_Click;
            }

            //Height of the view will be taken as RowHeight for child items
            //and the heightrequest property of the first view in the template
            //or the render height (whatever that is) if the heightrequest property is not defined.
            var height = viewType == 0 ? Convert.ToInt32(Element.RowHeight) :
                gridViewCell.View.HeightRequest != -1 ?
                Convert.ToInt32(gridViewCell.View.HeightRequest) :
                Convert.ToInt32(gridViewCell.RenderHeight);

            var width = Convert.ToInt32(Element.MinItemWidth);
            var dpW = ConvertDpToPixels(width);
            var dpH = ConvertDpToPixels(height);

            //If there are no minimums then the view doesn't render at all.
            view.SetMinimumWidth(dpW);
            view.SetMinimumHeight(dpH);

            var d =  parent.Context.GetDrawable("statelist_item_background.xml");
            view.SetBackground(d);

            //If not set to match parent then the view doesn't stretch to fill. 
            //This is not necessary anymore with the plaform fix above.
            //view.LayoutParameters = new GridLayoutManager.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            //Wrap the view.
            view.SetOnLongClickListener(this);
            GridViewCell myView = new GridViewCell(view);

            //return the view.
            return myView;
        }

        private int ConvertDpToPixels(float dpValue)
        {
            var pixels = (int)((dpValue) * _displayMetrics.Density);
            return pixels;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            GridViewCell myHolder = holder as GridViewCell;

            myHolder.ItemView.Selected = _selectedItemPosition == position;

            var item = _flattenedItems == null ? 
                Items.Cast<object>().ElementAt(position) : 
                _flattenedItems.ElementAt(position);

            myHolder.ViewCellContainer.Update(item);
        }

        void MMainView_Click(object sender, EventArgs e)
        {
            //If there is a current item selected.
            if (_selectedItemPosition >= 0)
            {
                //Remove the activated state.
                var previousSelection = _recyclerView.FindViewHolderForAdapterPosition(_selectedItemPosition);

                if (previousSelection != null)
                {
                    previousSelection.ItemView.Selected = false;
                }
            }

            //Unbox the view.
            var newSelectedView = (Android.Views.View)sender;

            //Set selected property.
            newSelectedView.Selected = true;
         
            //Get the position of this view.
            int position = _recyclerView.GetChildAdapterPosition(newSelectedView);

            //Store the selected item position.
            _selectedItemPosition = position;

            var item = _flattenedItems == null ?
                Items.Cast<object>().ElementAt(position) :
                _flattenedItems.ElementAt(position);

            Element.InvokeItemSelectedEvent(this, item);
        }

        /// <summary>
        /// Prepare the renderer for grouped collection.
        /// </summary>
        void PrepareGrouping()
        {
            int count = 0;
            int group = 0;

            //Reset.
            _selectedItemPosition = -1;
            _flattenedItems = null;
            _groupIndexDictionary.Clear();

            //If there are any items.
            if (_items != null)
            {
                //Get the groups within the items source.
                var groups = _items.OfType<IEnumerable>().Where(grp => !(grp is string));

                //If there are any groups.
                if (groups.Any())
                {
                    //Build the flattended item collection.
                    _flattenedItems = groups.SelectMany((r =>
                    {
                        _groupIndexDictionary.Add(group++, count++);
                        var childItems = r.Cast<object>().ToList();
                        count += childItems.Count;
                        childItems.Insert(0, r);
                        return childItems;
                    }));

                }
            }
        }

        public override int ItemCount
        {
            get
            {
                if (_flattenedItems != null) return _flattenedItems.Count();
                if(Items != null) return Items.Cast<object>().Count();
                return 0;
            }

        }

        public static int IndexOf(IEnumerable collection, object element, IEqualityComparer comparer = null)
        {
            int i = 0;
            comparer = comparer ?? EqualityComparer<object>.Default;
            foreach (var currentElement in collection)
            {
                if (comparer.Equals(currentElement, element))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public bool OnLongClick(Android.Views.View v)
        {
            var view = (GridViewCellContainer)v;
            var item = view.Element.BindingContext;
            _gridView.RaiseOnItemHold(item);
            return true;
        }
    }

    #endregion

    #region GridViewCellContainer

    public class GridViewCellContainer : ViewGroup
    {
        global::Android.Views.View _parent;
        Plugin.GridViewControl.Common.FastGridCell _viewCell;
        Android.Views.View _nativeView;
        Xamarin.Forms.Size _previousSize;
        bool _isLaidOut;

        public Element Element
        {
            get { return _viewCell; }
        }

        public GridViewCellContainer(Context context, Plugin.GridViewControl.Common.FastGridCell viewCell, global::Android.Views.View parent, Xamarin.Forms.Size initialCellSize) : base(context)
        {
            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(() =>
                {
                    _parent = parent;
                    _viewCell = viewCell;
                    _viewCell.PrepareCell(initialCellSize);
                    var renderer = Platform.GetRenderer(viewCell.View);
                    if (renderer == null)
                    {
                        renderer = Platform.CreateRendererWithContext(viewCell.View, context);
                        Platform.SetRenderer(viewCell.View, renderer);

                    }
                    _nativeView = renderer.View;
                    AddView(_nativeView);
                });
            }
        }

        public void Update(object bindingContext)
        {
            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(() => {
                    _viewCell.BindingContext = bindingContext;
                });
            }
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(() =>
                {
                    double width = base.Context.FromPixels((double)(right - left));
                    double height = base.Context.FromPixels((double)(bottom - top));
                    var size = new Xamarin.Forms.Size(width, height);

                    var msw = MeasureSpec.MakeMeasureSpec(right - left, MeasureSpecMode.Exactly);
                    var msh = MeasureSpec.MakeMeasureSpec(bottom - top, MeasureSpecMode.Exactly);
                    _nativeView.Measure(msw, msh);
                    _nativeView.Layout(0, 0, right - left, bottom - top);

                    if (size != _previousSize || !_isLaidOut)
                    {
                        if (_viewCell.View is Layout layout)
                        {
                            layout.Layout(new Rectangle(0, 0, width, height));

                            //Doesn't appear to be necessary?
                            //layout.ForceLayout();
                            //FixChildLayouts(layout);


                            _isLaidOut = true;
                        }
                    }

                    _previousSize = size;
                });
            }
        }

        void FixChildLayouts(Layout layout)
        {
            foreach (var child in layout.Children.OfType<Layout>())
            {
                child.ForceLayout();
                FixChildLayouts(child);
            }
        }
    }

    #endregion
}

