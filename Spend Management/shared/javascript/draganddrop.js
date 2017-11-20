Type.registerNamespace('Custom.UI');

///////////////////////////////////////////////////////////////////////
// DragAndDrop class

Custom.UI.DragAndDrop = function(element)
{
    Custom.UI.DragAndDrop.initializeBase(this, [element]);
    this._mouseDownHandler = Function.createDelegate(this,
        this.mouseDownHandler);
    this._visual = null;
}

Custom.UI.DragAndDrop.prototype =
{
    // IDragSource methods
    get_dragDataType: function()
    {
        return 'DragDropStep';
    },

    get_dragMode: function()
    {
        return Sys.Preview.UI.DragMode.Copy;
    },

    getDragData: function() {},
    
    onDragStart: function() {},

    onDrag: function() {},

    onDragEnd: function(canceled)
    {
        if (this._visual)
            this.get_element().parentNode.removeChild(this._visual);
    },
    
    // Other methods
    initialize: function()
    {
        Custom.UI.DragAndDrop.callBaseMethod(this,
            'initialize');
        $addHandler(this.get_element(), 'mousedown',
            this._mouseDownHandler)
    },

    mouseDownHandler: function(ev)
    {
        window._event = ev; // Needed internally by _DragDropManager

        this._visual = this.get_element().cloneNode(true);
        this._visual.style.opacity = '0.4';
        this._visual.style.filter =
          'progid:DXImageTransform.Microsoft.BasicImage(opacity=0.4)';
        this._visual.style.zIndex = 99999;
        this.get_element().parentNode.appendChild(this._visual);
        var location =
            Sys.UI.DomElement.getLocation(this.get_element());
        Sys.UI.DomElement.setLocation(this._visual, location.x,
            location.y);

        Sys.Preview.UI.DragDropManager.startDragDrop(this,
            this._visual, null);
    },

    dispose: function()
    {
        if (this._mouseDownHandler)
            $removeHandler(this.get_element(), 'mousedown',
                this._mouseDownHandler);
        this._mouseDownHandler = null;
        Custom.UI.DragAndDrop.callBaseMethod(this,
            'dispose');
    }
}

Custom.UI.DragAndDrop.registerClass
    ('Custom.UI.DragAndDrop', Sys.UI.Behavior,
    Sys.Preview.UI.IDragSource);




















///////////////////////////////////////////////////////////////////////
// DragAndDropTargetBehavior class

Custom.UI.DragAndDropTargetBehavior = function(element)
{
    Custom.UI.DragAndDropTargetBehavior.initializeBase(this, [element]);
    this._color = null;
}
    
Custom.UI.DragAndDropTargetBehavior.prototype =
{
    // IDropTarget methods
    get_dropTargetElement: function()
    {
        return this.get_element();
    },

    canDrop: function(dragMode, dataType, data)
    {
        return (dataType == 'DragDropStep' && data);
    },

    drop: function(dragMode, dataType, data)
    {
        if (dataType == 'DragDropStep' && data)
        {
            this.get_element().style.backgroundColor = data;

        }
    },

    onDragEnterTarget: function(dragMode, dataType, data)
    {
        // Highlight the drop zone by changing its background
        // color to light gray
        if (dataType == 'DragDropStep' && data)
        {
            //this._color = this.get_element().style.backgroundColor;
            //this.get_element().style.backgroundColor = '#E0E0E0';
        }
    },
    
    onDragLeaveTarget: function(dragMode, dataType, data)
    {
        // Unhighlight the drop zone by restoring its original
        // background color
        if (dataType == 'DragDropStep' && data)
        {
            this.get_element().style.backgroundColor = this._color;
        }
    },

    onDragInTarget: function(dragMode, dataType, data) { },
    
    // Other methods
    initialize: function()
    {
        Custom.UI.DragAndDropTargetBehavior.callBaseMethod(this,
            'initialize');
        Sys.Preview.UI.DragDropManager.registerDropTarget(this);
    },
    
    dispose: function()
    {
        Sys.Preview.UI.DragDropManager.unregisterDropTarget(this);
        Custom.UI.DragAndDropTargetBehavior.callBaseMethod(this,
            'dispose');
    }
}

Custom.UI.DragAndDropTargetBehavior.registerClass
    ('Custom.UI.DragAndDropTargetBehavior', Sys.UI.Behavior,
    Sys.Preview.UI.IDropTarget);





















///////////////////////////////////////////////////////////////////////
// Script registration

Sys.Application.notifyScriptLoaded();
