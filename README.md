
# Xamarin.Forms.GridView
A working! GridView for xamarin forms 

Supports UWP (ItemsPanelTemplate set to ItemsWrapGrid), Android (Recycler)  and IOS (UICollectionView)

90% of android and ios renderer code came from https://github.com/twintechs/TwinTechsFormsLib. However this solution was not working for me and so I ended up making several changes. There were also some features removed that I didn't need and/or was causing undesired behaviour.

Note that grouping is not currently working on ios.

There are several files in the solution but really the only files you need are

XamarinFormsGridView/XamarinFormsGridView/Controls/GridView.cs
XamarinFormsGridView/XamarinFormsGridView/Controls/FastGridCell.cs

XamarinFormsGridView/XamarinFormsGridView.UWP/Renderers/ListViewRenderer.cs

XamarinFormsGridView/XamarinFormsGridView.iOS/Renderers/GridViewRenderer.cs

XamarinFormsGridView/XamarinFormsGridView.Android/Renderers/GridViewRenderer.cs
