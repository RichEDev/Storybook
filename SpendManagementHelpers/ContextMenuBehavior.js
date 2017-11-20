//=======================================================================================
// Iridescence.Ajax | Copyright © Fredrik Kalseth | http://blog.iridescence.no
//=======================================================================================
// This source code is usable under the Microsoft Permissive License:
// http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx
//=======================================================================================

Type.registerNamespace('SpendManagementHelpers');

SpendManagementHelpers.ContextMenuBehavior = function(element) {
    SpendManagementHelpers.ContextMenuBehavior.initializeBase(this, [element]);

    this._contextMenuControlID = null;

    this._onMouseDownHandler = null;
    this._onDocumentContextMenuHandler = null;
    this._onDocumentClickHandler = null;

    this._contextElement = null;
    this._menuElement = null;

    this._menuVisible = false;
    this._menuJustShown = false;

    this._cursorOffsetX = 0;
    this._cursorOffsetY = 0;
}

SpendManagementHelpers.ContextMenuBehavior.prototype =
{
    initialize: function() {
        SpendManagementHelpers.ContextMenuBehavior.callBaseMethod(this, 'initialize');

        _contextElement = this.get_element();
        _menuElement = $get(this._contextMenuControlID);

        // style the context menu
        _menuElement.style.display = 'none';
        _menuElement.style.position = 'absolute';

        // attach event handlers
        this._onMouseDownHandler = Function.createDelegate(this, this._onMouseDown);
        this._onDocumentContextMenuHandler = Function.createDelegate(this, this._onDocumentContextMenu);
        this._onDocumentClickHandler = Function.createDelegate(this, this._onDocumentClick);

        $addHandler(_contextElement, 'mousedown', this._onMouseDownHandler);
        $addHandler(document, 'contextmenu', this._onDocumentContextMenuHandler);
        $addHandler(document, 'click', this._onDocumentClickHandler);
    },

    _onMouseDown: function(e) {
        if ((e.which && e.which == 3) || (e.button && e.button == 2)) {
            // calculate current mouse position            
            var scrollTop = document.body.scrollTop ? document.body.scrollTop : document.documentElement.scrollTop;
            var scrollLeft = document.body.scrollLeft ? document.body.scrollLeft : document.documentElement.scrollLeft;

            // and move context menu there
            _menuElement.style.left = (e.clientX + this._cursorOffsetX) + scrollLeft + 'px';
            _menuElement.style.top = (e.clientY + this._cursorOffsetY) + scrollTop + 'px';
            _menuElement.style.display = '';

            // set flags
            this._menuVisible = true;
            this._menuJustShown = true;
        }
    },

    _onDocumentContextMenu: function(e) {
        if (this._menuJustShown) {
            // when our custom context menu is showing, we want to disable the browser context menu
            this._menuJustShown = false;

            if (e.preventDefault)
                e.preventDefault();
            else
                return false;

        }
        else if (this._menuVisible) {
            // user right-clicks anywhere while our custom context menu is visible; hide it
            this._hideMenu();
        }
    },

    _onDocumentClick: function(e) {
        if (this._menuVisible && e.button != 2) {
            // user left-clicked anywhere while custom context menu is visible; hide it
            this._hideMenu();
        }
    },

    _hideMenu: function() {
        _menuElement.style.display = 'none';
        this._menuVisible = false;
    },

    dispose: function() {
        // clean up
        $removeHandler(_contextElement, 'mousedown', this._onMouseDownHandler);
        $removeHandler(document, 'contextmenu', this._onDocumentContextMenuHandler);
        $removeHandler(document, 'click', this._onDocumentClickHandler);

        SpendManagementHelpers.ContextMenuBehavior.callBaseMethod(this, 'dispose');
    },

    get_ContextMenuControlID: function() {
        return this._contextMenuControlID;
    },

    set_ContextMenuControlID: function(value) {
        this._contextMenuControlID = value;
    },

    get_CursorOffsetX: function() {
        return this._cursorOffsetX;
    },
    set_CursorOffsetX: function(value) {
        this._cursorOffsetX = value;
    },
    get_CursorOffsetY: function() {
        return this._cursorOffsetY;
    },
    set_CursorOffsetY: function(value) {
        this._cursorOffsetY = value;
    }
}

SpendManagementHelpers.ContextMenuBehavior.registerClass('SpendManagementHelpers.ContextMenuBehavior', AjaxControlToolkit.BehaviorBase);
