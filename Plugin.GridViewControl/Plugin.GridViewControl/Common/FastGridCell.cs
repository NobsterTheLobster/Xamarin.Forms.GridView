using System;
using Xamarin.Forms;

namespace Plugin.GridViewControl.Common
{
    /// <summary>
    /// Abstract view cell with callbacks for drawing the cell in the code behind (InitializeCell)
    /// and updating the cell based on the binding context (SetupCell). 
    /// </summary>
    public abstract class FastGridCell : ViewCell
    {
        /// <summary>
        /// Gets whether the cell has been initialized i.e. the view has been declared.
        /// </summary>
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the size of the cell.
        /// </summary>
        public Size CellSize { get; private set; }

        /// <summary>
        /// passes in the cell size as a convenience
        /// </summary>
        /// <param name="cellSize">Cell size.</param>
        public void PrepareCell(Size cellSize)
        {
            CellSize = cellSize;
            InitializeCell();
            if (BindingContext != null)
            {
                SetupCell(false);
            }
            IsInitialized = true;
        }

        /// <summary>
        /// Update the cell when the binding context changes.
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
           
            if (IsInitialized)
            {
                SetupCell(true);
            }
        }

        /// <summary>
        /// Setups the cell. You should call InitializeComponent in here
        /// </summary>
        protected abstract void InitializeCell();

        /// <summary>
        /// Do your cell setup using the binding context in here.
        /// </summary>
        /// <param name="isRecycled">If set to <c>true</c> is recycled.</param>
        protected abstract void SetupCell(bool isRecycled);

        /// <summary>
        /// Called when the size of the view changes. Override to do layout task if required
        /// </summary>
        /// <param name="size">Size.</param>
        public virtual void OnSizeChanged(Size size)
        {

        }

        /// <summary>
        /// Override if you are intereted in when a cell moves (for parallaxe effects etc)
        /// </summary>
        /// <param name="contentOffset">Content offset.</param>
        /// <param name="cellLocation">Cell location.</param>
        public virtual void OnScroll(Point contentOffset, Point cellLocation)
        {

        }
    }

    /// <summary>
    /// Implementation of FastGridCell with empty Initialize And Setup cell calls i.e.
    /// the view cell must be declared in XAML 
    /// </summary>
    public partial class GridViewXamlCell : FastGridCell
    {
        /// <summary>
        /// Layout the cell structure.
        /// </summary>
        protected override void InitializeCell()
        {
        }

        /// <summary>
        /// Update the cell based on the binding context.
        /// </summary>
        /// <param name="isRecycled">Indicates whether the cell is being re-used.</param>
        protected override void SetupCell(bool isRecycled)
        {
        }
    }
}

