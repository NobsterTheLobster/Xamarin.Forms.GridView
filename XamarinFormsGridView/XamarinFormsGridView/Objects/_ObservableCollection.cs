using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace XamarinFormsGridView
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when 
    /// items get added, removed, or when the whole list is refreshed. 
    /// The class is extended with new range methods which fire a single 
    /// collection changed event, and a single count property changed event 
    /// regardless of the number of items affected. Additionally there is
    /// now an event for property changes in child elements.
    /// </summary> 
    /// <typeparam name="T"></typeparam>
    public class _ObservableCollection<T> : ObservableRangeCollection<T>
    {
        bool _isSourceGrouped;

        public bool IsSourceGrouped
        {
            get { return _isSourceGrouped; }
            set
            {
                _isSourceGrouped = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsSourceGrouped"));
            }
        }

        #region ChildElementPropertyChangedEventArgs

        /// <summary>
        /// Nested class for new child element property changed event.
        /// </summary>
        public class ChildElementPropertyChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Get the name of the property that was changed.
            /// </summary>
            public string PropertyName { get; private set; }

            /// <summary>
            /// Get the original value of the property that was changed.
            /// </summary>
            public object OldValue { get; private set; }

            /// <summary>
            /// Get the new value of the property that was changed.
            /// </summary>
            public object NewValue { get; private set; }

            /// <summary>
            /// Initialize a new instance of the ChildElementPropertyChangedEventArgs class.
            /// </summary>
            /// <param name="name">The name of the property that was changed.</param>
            /// <param name="oldValue">The original value of the property that was changed.</param>
            /// <param name="newValue">The new value of the property that was changed.</param>
            public ChildElementPropertyChangedEventArgs(string name, object oldValue, object newValue)
            {
                //Set using private accessor.
                PropertyName = name;

                //Set using private accessor.
                OldValue = oldValue;

                //Set using private accessor.
                NewValue = newValue;
            }
        }

        #endregion

        #region ChildElementPropertyChanged

        /// <summary>
        /// Delegate for forwarding the property notification from 
        /// a child element as an event from the collection.
        /// </summary>
        /// <param name="sender">The source item in which the property change occured.</param>
        /// <param name="e">The arguments for the event.</param>
        public delegate void ChildElementPropertyChangedEventHandler(object sender, ChildElementPropertyChangedEventArgs e);

        /// <summary>
        /// Raises an event when a property notification 
        /// occurs in a child element.
        /// </summary>
        public event ChildElementPropertyChangedEventHandler ChildElementPropertyChanged;

        /// <summary>
        /// This method will raise an event when a property change event
        /// occurs in a child element. The source element is supplied
        /// in the event arguments. A protected member is accessible from 
        /// within the class in which it is declared, and from within any 
        /// class derived from the class that declared this member.
        /// The implementation of a virtual member can be changed by an 
        /// overriding member in a derived class.When a virtual method is 
        /// invoked, the run-time type of the object is checked for an 
        /// overriding member. The overriding member in the most derived 
        /// class is called, which might be the original member, if no 
        /// derived class has overridden the member.
        /// </summary>
        /// <param name="sender">The original source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        protected virtual void OnChildElementPropertyChanged(object sender, ChildElementPropertyChangedEventArgs e)
        {
            //If there is something listening for the event.
            if (ChildElementPropertyChanged != null)
            {
                //Raise the event.
                ChildElementPropertyChanged(sender, e);
            }
        }

        /// <summary>
        /// Calls the base OnCollectionChanged method. Child element property 
        /// notification is not effected.
        /// </summary>
        /// <param name="e">Provides data for the CollectionChanged event.</param>
        protected void OnCollectionFiltered(NotifyCollectionChangedEventArgs e)
        {
            //Call base method first.
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Override of OnCollectionChanged method in order to attach
        /// additional handler to the property changed event of each
        /// child item, so that the collection can subsequently raise
        /// its own event. The OnMethodName syntax is used to define
        /// a method which will raise an event. A protected member is 
        /// accessible from within the class in which it is declared, 
        /// and from within any class derived from the class that 
        /// declared this member.
        /// </summary>
        /// <param name="e">Provides data for the CollectionChanged event.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            //Call base method first.
            base.OnCollectionChanged(e);

            //If items are being added.
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //Itterate through new items.
                foreach (T item in e.NewItems)
                {
                    //Cast the object as INotifyPropertyChanged interface (object needs to inherit this.)
                    INotifyPropertyChanged convertedItem = item as INotifyPropertyChanged;

                    //If the item can implement property notification.
                    if (convertedItem != null)
                    {
                        //Attaching handling for property changed event.
                        convertedItem.PropertyChanged += new PropertyChangedEventHandler(convertedItem_PropertyChanged);
                    }
                }
            }
            //If items are being removed.
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //Itterate through old items.
                foreach (T item in e.OldItems)
                {
                    //Cast the object as before.
                    INotifyPropertyChanged convertedItem = item as INotifyPropertyChanged;

                    //If the item can implement property notification.
                    if (convertedItem != null)
                    {
                        //Detach the handler.
                        convertedItem.PropertyChanged -= new PropertyChangedEventHandler(convertedItem_PropertyChanged);
                    }
                }
            }
            //If collection has been reset.
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //Itterate through current items.
                foreach (T item in Items)
                {
                    //Cast the object as before.
                    INotifyPropertyChanged convertedItem = item as INotifyPropertyChanged;

                    //If the item can implement property notification.
                    if (convertedItem != null)
                    {
                        //Detach the handler (just in case, more than likely this will have been removed by the clear items.)
                        convertedItem.PropertyChanged -= new PropertyChangedEventHandler(convertedItem_PropertyChanged);

                        //Attaching handling for property changed event.
                        convertedItem.PropertyChanged += new PropertyChangedEventHandler(convertedItem_PropertyChanged);
                    }
                }
            }
        }

        /// <summary>
        /// This is the new event handler for property changes in the child elements.
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        void convertedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Store the name of the property.
            string propertyName = e.PropertyName;

            //Declare fields for old and new values.
            object oldvalue = null, newValue = null;

            //If using extended arguments class.
            if (e is _PropertyChangedEventArgs)
            {
                //Unbox the object.
                _PropertyChangedEventArgs ex = e as _PropertyChangedEventArgs;

                //Set the old-value.
                oldvalue = ex.OldValue;

                //Set the new-value.
                newValue = ex.NewValue;
            }

            //Call method to raise event.
            OnChildElementPropertyChanged(sender, new ChildElementPropertyChangedEventArgs(
                propertyName, oldvalue, newValue));
        }

        /// <summary>
        /// Clear all items in the collection. Property changed event 
        /// handlers are detached from each item before removing.
        /// </summary>
        protected override void ClearItems()
        {
            //Itterate through the items.
            foreach (object obj in Items)
            {
                //Get property changed interface.
                INotifyPropertyChanged item = obj as INotifyPropertyChanged;

                //If the object implements interface.
                if (item != null)
                {
                    //Remove the property changed handling.
                    item.PropertyChanged -= convertedItem_PropertyChanged;
                }
            }

            //Call the base method.
            base.ClearItems();
        }

        #endregion

        #region Constructors

        /// <summary> 
        /// Initializes a new instance of the System.Collections.
        /// ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public _ObservableCollection()
            : base()
        { }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.
        /// ObservableCollection(Of T) class that contains elements copied 
        /// from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public _ObservableCollection(IEnumerable<T> collection)
            : base(collection)
        { }

        #endregion
    }

    /// <summary>
    /// Represents a dynamic data collection that provides notifications when 
    /// items get added, removed, or when the whole list is refreshed. 
    /// The class is extended with new range methods which fire a single 
    /// collection changed event, and a single count property changed event 
    /// regardless of the number of items affected.
    /// </summary> 
    /// <typeparam name="T"></typeparam>
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        #region RangeMethods

        /// <summary> 
        /// Adds the elements of the specified collection 
        /// to the end of the ObservableCollection(Of T). 
        /// Because this calls Add on the protected Items IList(Of T)
        /// of Collection(Of T), there are no changed events 
        /// sent i.e. no individual notifications for each 
        /// item added.
        /// </summary> 
        /// <param name="collection">The collection of items to add.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        /// <summary> 
        /// Removes the first occurence of each item in the specified 
        /// collection from ObservableCollection(Of T). Because this 
        /// calls Remove on the protected Items IList(Of T) of Collection(Of T), 
        /// there are no changed events sent i.e. no individual notifications 
        /// for each item removed.
        /// </summary> 
        /// <param name="collection">The collection of items to remove.</param>
        public void RemoveRange(IEnumerable<T> collection)
        {
            foreach (var i in collection) Items.Remove(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        /// <summary> 
        /// Clears the current collection and 
        /// replaces it with the specified item. 
        /// </summary> 
        /// <param name="item">The single item to replace the current 
        /// collection of items with.</param>
        public void Replace(T item)
        {
            ReplaceRange(new T[] { item });
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// Because this calls Clear and Add on the protected Items IList(Of T) of Collection(Of T), 
        /// there are no changed events sent i.e. no individual notifications for each item 
        /// removed or added.
        /// </summary> 
        /// <param name="collection">The range of items of copy into the collection.</param>
        public void ReplaceRange(IEnumerable<T> collection)
        {
            //Clear and add new items.
            Items.Clear();
            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        #endregion

        #region Constructors

        /// <summary> 
        /// Initializes a new instance of the System.Collections.
        /// ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public ObservableRangeCollection()
            : base()
        { }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.
        /// ObservableCollection(Of T) class that contains elements copied 
        /// from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection)
        { }

        #endregion
    }

    /// <summary>
    /// Extended PropertyChangedEventArgs to store information
    /// regarding the old and new values as well as the name of
    /// property affected.
    /// </summary>
    public class _PropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Initialize a new instance of the PropertyChangedEventArgs class
        /// This constructor firstly calls the base constructor and then
        /// sets the additional oldValue and newValue properties.
        /// </summary>
        /// <param name="propertyName">The name of the property affected.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public _PropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName)
        {
            //Set using private accessor.
            this.OldValue = oldValue;

            //Set using private accessor
            this.NewValue = newValue;
        }

        /// <summary>
        /// Gets the new value of the property.
        /// </summary>
        public object NewValue { get; private set; }

        /// <summary>
        /// Gets the old value of the property.
        /// </summary>
        public object OldValue { get; private set; }
    }
}
