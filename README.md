 # Xamarin.Forms.GridView
A working! (tested with Xamarin.Forms 2.5.0.280555) GridView for xamarin forms with grouping, pull to refresh and selection visual state support.

Supports UWP (ItemsPanelTemplate set to ItemsWrapGrid), Android (Recycler)  and IOS (UICollectionView) 

The control inherits from the xamarin listview and on UWP uses the built in xamarin ListViewRenderer but with changes to set the ItemsPanelTemplate to be a ItemsWrapGrid. Therefore in theory all of the existing xamarin listview features should be supported for the gridview when targetting UWP.

The android and ios use custom renderers. 90% of android and ios renderer code came from https://github.com/twintechs/TwinTechsFormsLib. However this solution was not working for me and so I ended up making several changes. There were also some features removed that I didn't need and/or were causing undesired behaviour. 

![Alt text](/XamarinGridView.png?raw=true "Screenshot")

Grouping
----------------
I've also modified the renderers to support grouping however the grouping support is a little inconsistent at the moment; on ios and android it will handle it based on whether the provided itemsource is a collection of collections. On UWP you must additionally set the IsGroupingEnabled property because its a requirement of the xamarin listviewrenderer which is still being used.

Additionally on ios and android the header height is determined by the height request of the group header datatemplate. On UWP the header height will be the same as RowHeight if RowHeight is a postive value. Alternatively on UWP you can choose not to set the RowHeight and set the property HasUnevenRows to true instead. In this configuration the height of cells are determined by their respective datatemplates. See Sample2.xaml for an example. Note that HasUnevenRows has no effect on ios/android.

Selection State
----------------
The selection visual states will follow the default behaviour on UWP since the default rendererer is still being used. 
ON Android you can add a drawable statelist_item_background.xml to specify the active (selected) state of an item.On IOS you can add a xib statelist_item_background.xib to define the selected background view. See repo for more details.

Pull To Refresh
----------------
Pull to Refresh is supported and I reckon it should behave as you would expect i.e. You first set the IsPullToRefreshEnabled property to true. Then you need to attach the RefreshCommand to your view model. When the user pulls down on the items control a busy animation will appear, the RefreshCommand will be exceuted and the IsRefreshing property will be set to true. When your view model has finished loading the data you need to set the IsRefreshing property back to false typically via a binding in your view model. Once the IsRefreshing property is set back to false the busy animation will disappear. 

DataTemplateSelector
--------------------
Due to the nature of the recycler control it is very difficult to support the datatemplateselector. I have provided a sample (Sample3) in the repo using a binding converter which I think is a valid workaround although I have not thoroughly tested it.

ItemHold (long click/right click)
--------------------
Courtesy of https://github.com/kipters the control now supports a long click/right click action.

Required Files
----------------
There are several files in the solution but for the gridview you really only need the following.

1. Plugin.GridViewControl/Plugin.GridViewControl/Common/GridView.cs
2. Plugin.GridViewControl/Plugin.GridViewControl/Common/FastGridCell.cs
3. Plugin.GridViewControl/Plugin.GridViewControl.Android/Renderers/GridViewRenderer.cs
4. Plugin.GridViewControl/Plugin.GridViewControl.iOS/Renderers/GridViewRenderer.cs
5. Plugin.GridViewControl/Plugin.GridViewControl.UWP/Renderers/GridViewRenderer.cs

Nuget 
----------------
Alternatively you can install the nuget package https://www.nuget.org/packages/Plugin.GridViewControl which only contains the files listed above.

There are a couple of things to note from an iOS perspective.

1. The renderer doesn't appear to get loaded unless you reference it in your appdelegate.e.g.

```
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            var x = typeof(Plugin.GridViewControl.iOS.Renderers.GridViewRenderer);
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
```

2. For some reason the selection state xib has become mandatory. So currently you need to manually add the statelist_item_background.xib from the repo to your ios project. I haven't figured out how to do this with the nuget package manager yet.
