using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Reflection;

namespace XamarinFormsGridView.Behaviours
{
    public class PickerBehaviour
    {
        #region BindableProperties

        /// <summary>
        /// Bindable collection.
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty =
     BindableProperty.CreateAttached(
         "ItemsSource",
         typeof(IList),
         typeof(PickerBehaviour),
         default(IList),
         propertyChanged: OnItemsSourceChanged);

        /// <summary>
        /// Bindabled selected item.
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.CreateAttached(
         "SelectedItem",
         typeof(object),
         typeof(PickerBehaviour),
         null,
         propertyChanged: OnSelectedItemChanged);

        /// <summary>
        /// Bindabled selected item.
        /// </summary>
        public static readonly BindableProperty DisplayMemberPathProperty =
            BindableProperty.CreateAttached(
         "DisplayMemberPath",
         typeof(string),
         typeof(PickerBehaviour),
         null);

        /// <summary>
        /// Bindabled selected item.
        /// </summary>
        public static readonly BindableProperty SelectedValuePathProperty =
            BindableProperty.CreateAttached(
         "SelectedValuePath",
         typeof(string),
         typeof(PickerBehaviour),
         null);

        /// <summary>
        /// Used for internal logic.
        /// </summary>
        public static readonly BindableProperty IsBusyProperty =
           BindableProperty.CreateAttached(
        "IsBusy",
        typeof(bool),
        typeof(PickerBehaviour),
        false);

        /// <summary>
        /// Used to set the focus on the control obeying mVVM principles.
        /// </summary>
        public static readonly BindableProperty FocusedProperty =
           BindableProperty.CreateAttached(
        "Focused",
        typeof(bool),
        typeof(PickerBehaviour),
        false, propertyChanged: OnFocusedChanged);

        #endregion

        #region Property Accessors

        /// <summary>
        /// Gets the value of the itemsource property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <returns>The value of the itemsource.</returns>
        public static IList GetItemsSource(BindableObject view)
        {
            return (IList)view.GetValue(ItemsSourceProperty);
        }

        /// <summary>
        /// Sets the value of the itemsource property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <param name="value">The new value of the itemsource.</param>
        public static void SetItemsSource(BindableObject view, IList value)
        {
            view.SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets the value of the selected Item property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <returns>The value of the selected item property.</returns>
        public static object GetSelectedItem(BindableObject view)
        {
            return (object)view.GetValue(SelectedItemProperty);
        }

        /// <summary>
        /// Sets the value of the Selected Item property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <param name="value">The new value for the property.</param>
        public static void SetSelectedItem(BindableObject view, object value)
        {
            view.SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// Gets the value of the Display Member property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <returns>The value of the selected item property.</returns>
        public static string GetDisplayMemberPath(BindableObject view)
        {
            return (string)view.GetValue(DisplayMemberPathProperty);
        }

        /// <summary>
        /// Sets the value of the Display Member property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <param name="value">The new value for the property.</param>
        public static void SetDisplayMemberPath(BindableObject view, string value)
        {
            view.SetValue(DisplayMemberPathProperty, value);
        }

        /// <summary>
        /// Gets the value of the Selected Value property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <returns>The value of the selected item property.</returns>
        public static string GetSelectedValuePath(BindableObject view)
        {
            return (string)view.GetValue(SelectedValuePathProperty);
        }

        /// <summary>
        /// Sets the value of the Selected Value property.
        /// </summary>
        /// <param name="view">The owner object of the dependency property, should be a dependency object.</param>
        /// <param name="value">The new value for the property.</param>
        public static void SetSelectedValuePath(BindableObject view, string value)
        {
            view.SetValue(SelectedValuePathProperty, value);
        }

        /// <summary>
        /// Gets the value of the IsBusy property.
        /// </summary>
        /// <param name="obj">The owner object of the dependency property, should be a dependency object.</param>
        /// <returns>The value of the IsBusy property.</returns>
        static bool GetIsBusy(BindableObject obj)
        {
            //Get the property value.
            return (bool)obj.GetValue(IsBusyProperty);
        }

        /// <summary>
        /// Sets the value of the IsBusy  property.
        /// </summary>
        /// <param name="obj">The owner object of the dependency property, should be a dependency object.</param>
        /// <param name="value">The new value for the property.</param>
        static void SetIsBusy(BindableObject obj, bool value)
        {
            //Set the property value.
            obj.SetValue(IsBusyProperty, value);
        }

        /// <summary>
        /// Gets the value of the Focused property.
        /// </summary>
        /// <param name="obj">The owner object of the dependency property, should be a dependency object.</param>
        /// <returns>The value of the IsBusy property.</returns>
        static bool GetFocused(BindableObject obj)
        {
            //Get the property value.
            return (bool)obj.GetValue(FocusedProperty);
        }

        /// <summary>
        /// Sets the value of the Focused  property.
        /// </summary>
        /// <param name="obj">The owner object of the dependency property, should be a dependency object.</param>
        /// <param name="value">The new value for the property.</param>
        static void SetFocused(BindableObject obj, bool value)
        {
            //Set the property value.
            obj.SetValue(FocusedProperty, value);
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Update the view when the focused property changes.
        /// </summary>
        /// <param name="bindable">The object the itemsource is bound to.</param>
        /// <param name="oldValue">The original value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        static void OnFocusedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //If the host object is a picker..
            if (bindable is Picker)
            {
                //Unbox.
                Picker picker = bindable as Picker;

                //If setting is true.
                if ((bool)newValue)
                {
                    //Set the Focus
                    picker.Focus();
                    

                    //Attach handling for lost focus.
                    picker.Unfocused += Picker_Unfocused;
                }
                else
                {
                    //Remove handling.
                    picker.Unfocused -= Picker_Unfocused;
                }
            }
        }

        /// <summary>
        /// Handling for when the control looses focus.
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        static void Picker_Unfocused(object sender, FocusEventArgs e)
        {
            //Unbox.
            Picker picker = sender as Picker;

            //Set dependency property.
            picker.SetValue(FocusedProperty, false);
        }

        /// <summary>
        /// Update the view when the itemsource changes in the view model.
        /// </summary>
        /// <param name="bindable">The object the itemsource is bound to.</param>
        /// <param name="oldValue">The original value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //Unbox the picker.
            var picker = bindable as Picker;

            //Clear existing.
            picker.Items.Clear();

            picker.SelectedIndexChanged -= Picker_SelectedIndexChanged;
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;

            //picker.Navigation.NavigationStack.Last().Disappearing += delegate (object sender, EventArgs e) { Disappearing(sender, e, picker); };

            //Attempt to unbox.
            var collection = newValue as IList;

            //If the intended itemsource implements IList
            if (newValue != null)
            {
                //Loop through each item
                foreach (var item in collection)
                {
                    picker.Items.Add(GetPickerDisplayItem(picker, item));
                }

                //Attempt to unbox.
                var iNotify = newValue as INotifyCollectionChanged;

                //If the intended itemsource implements INofifyCollectionChanged.
                if (iNotify != null)
                {
                    //Handling for collection changed events.
                    iNotify.CollectionChanged += (sender, args) =>
                    {
                        switch (args.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                {
                                    //Loop through each new item.
                                    foreach (var item in args.NewItems)
                                    {
                                        //Add it to the picker.
                                        picker.Items.Add(GetPickerDisplayItem(picker, item));
                                    }

                                    //No fall through.
                                    break;
                                }
                            case NotifyCollectionChangedAction.Remove:
                                {
                                    //Loop through each old item.
                                    foreach (var item in args.OldItems)
                                    {
                                        //Remove it from the picker.
                                        picker.Items.Remove(GetPickerDisplayItem(picker, item));
                                    }

                                    //No fall through.
                                    break;
                                }
                            default:
                                {
                                    //Clear the items.
                                    picker.Items.Clear();

                                    var list = sender as IList;

                                    //if there is any new items.
                                    if (list != null)
                                    {
                                        //Loop through each item
                                        foreach (var item in list)
                                        {
                                            //Add it to the picker.
                                            picker.Items.Add(GetPickerDisplayItem(picker, item));
                                        }
                                    }

                                    //No fall through.
                                    break;
                                }
                        }
                    };
                }
            }
        }

        //static void Disappearing(object sender, EventArgs e, Picker p)
        //{

        //}

        /// <summary>
        /// Selection changing in the view.
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        static void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Unbox the picker.
            var picker = sender as Picker;

            if (GetIsBusy(picker))
                return;

            //Set busy flag.
            SetIsBusy(picker, true);

            if (picker.SelectedIndex >= 0)
            {
                var item = GetItemsSource(picker)[picker.SelectedIndex];

                if (!String.IsNullOrEmpty(GetSelectedValuePath(picker)))
                {
                    var type = GetItemsSource(picker)[picker.SelectedIndex].GetType();
                    var prop = type.GetRuntimeProperty(GetSelectedValuePath(picker));
                    SetSelectedItem(picker, prop.GetValue(item));
                }
                else
                {
                    SetSelectedItem(picker, item);
                }
            }
            else
            {
                SetSelectedItem(picker, null);
            }

            //Set the selected item in the view model.
            //SetSelectedItem(picker, picker.SelectedIndex >= 0 ? GetItemsSource(picker)[picker.SelectedIndex] : null);

            //Set busy flag.
            SetIsBusy(picker, false);
        }

        /// <summary>
        /// Selection changing in the view model.
        /// </summary>
        /// <param name="bindable">The object the selected item is bound to.</param>
        /// <param name="oldValue">The original value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            //Unbox the picker.
            var picker = bindable as Picker;

            //Get the busy indicator.
            if (GetIsBusy(picker))
                return;

            //Set busy flag.
            SetIsBusy(picker, true);

            if (newvalue != null)
            {
                string lookupItem = null;

                //If there is a selected value path. Then this is what
                //this piece of data repersents and we can assume that 
                //the SelectedValuePath must be a unique field.
                //Therefore we need to get 
                if (!String.IsNullOrEmpty(GetSelectedValuePath(picker)))
                {
                    //Get the item 
                    var item = GetItemsSource(picker).Cast<object>().FirstOrDefault(r =>
                    r.GetType().GetRuntimeProperty(GetSelectedValuePath(picker)).GetValue(r).ToString() == newvalue.ToString());

                    if (item != null)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(GetDisplayMemberPath(picker));
                        lookupItem = prop.GetValue(item).ToString();
                    }
                }
                else
                {
                    lookupItem = newvalue.ToString();
                }

                //If there is a selected item.
                if (lookupItem != null)
                {
                    picker.SelectedIndex = picker.Items.IndexOf(lookupItem);
                }
            }

            //Set busy flag.
            SetIsBusy(picker, false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the text to display in the picker.
        /// </summary>
        /// <param name="picker">The picker object that will display the item.</param>
        /// <param name="item">The datasource item to display</param>
        /// <returns>A string to display in the picker control.</returns>
        private static string GetPickerDisplayItem(Picker picker, object item)
        {
            string itemResult = string.Empty;

            if (item != null)
            {
                if (string.IsNullOrEmpty(GetDisplayMemberPath(picker)))
                {
                    itemResult = item.ToString();
                }
                else
                {
                    var type = item.GetType();
                    var prop = type.GetRuntimeProperty(GetDisplayMemberPath(picker));
                    itemResult = prop.GetValue(item).ToString();
                }
            }

            return itemResult;
        }

        #endregion
    }
}
