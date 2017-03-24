# Xamarin.Forms.GridView
A working! GridView for xamarin forms 

Supports UWP (ItemsPanelTemplate set to ItemsWrapGrid), Android (Recycler)  and IOS (UICollectionView) 

The control inherits from the xamarin listview and on UWP uses the built in xamarin ListViewRenderer but with changes to set the ItemsPanelTemplate to be a ItemsWrapGrid. Therefore in theory all of the existing xamarin listview features should be supported for the gridview when targetting UWP.

The android and ios use custom renderers. 90% of android and ios renderer code came from https://github.com/twintechs/TwinTechsFormsLib. However this solution was not working for me and so I ended up making several changes. There were also some features removed that I didn't need and/or were causing undesired behaviour. 

I've also modified the renderers to support grouping however please note that the group header template is not currently working on ios.
Also the grouping support is a little inconsitent at the moment; on ios it will handle it based on whether the provided itemsource is a collection of collections. On Android and UWP you must additionally set the IsGroupingEnabled property. On UWP this is because its a requirement of the xamarin listviewrenderer which is still being used. On Android this was simply a design choice which I may change to be more like iOS.

There are several files in the solution but for the gridview you really only need the following.

1. XamarinFormsGridView/XamarinFormsGridView/Controls/GridView.cs
2. XamarinFormsGridView/XamarinFormsGridView/Controls/FastGridCell.cs
3. XamarinFormsGridView/XamarinFormsGridView.UWP/Renderers/ListViewRenderer.cs
4. XamarinFormsGridView/XamarinFormsGridView.iOS/Renderers/GridViewRenderer.cs
5. XamarinFormsGridView/XamarinFormsGridView.Android/Renderers/GridViewRenderer.cs
