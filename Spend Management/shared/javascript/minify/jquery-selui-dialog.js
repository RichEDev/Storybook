/* Software Europe's extensions or overloads to jQuery or jQuery UI features*/

/*
	jQuery.ui.dialog extension
*/
(function ($)
{
    // Dialog: Expand, Collapse, Dock, UnDock
    // Extensions to the basic jQuery UI Dialog for our dockable dialog
    // not an especially flexible example, but demonstrates the basic method of extending things

    var _init = $.ui.dialog.prototype._init;

    // Custom Dialog Init
    $.ui.dialog.prototype._init = function ()
    {
        var self = this;
        _init.apply(this, arguments);

        var uiDialogTitlebar = this.uiDialogTitlebar;

        if (this.options.enableDock)
        {
            // If the dock functionality is on, then close functionality needs to be disabled
            this.options.closeOnEscape = false;
            $('a.ui-dialog-titlebar-close', uiDialogTitlebar).hide();

            // Store the original height and position
            this.options.originalHeight = this.options.height;
            this.options.lastPosition = this.uiDialog.position();

            // Track the state of the dialog, 'open', 'collapsed', 'expanded', 'docked'
            this.options.shownState = 'open';

            uiDialogTitlebar.append('<a href="#" class="dialog-dock ui-dialog-titlebar-dock ui-corner-all" role="button"><span class="ui-icon ui-icon-extlink" title="Dock"></span></a>');
            uiDialogTitlebar.append('<a href="#" class="dialog-restore ui-dialog-titlebar-restore ui-corner-all" role="button"><span class="ui-icon ui-icon-newwin" title="Restore"></span></a>');

            uiDialogTitlebar.append('<a href="#" class="dialog-expand ui-dialog-titlebar-expand ui-corner-all" role="button"><span class="ui-icon ui-icon-triangle-1-s" title="Expand"></span></a>');
            uiDialogTitlebar.append('<a href="#" class="dialog-collapse ui-dialog-titlebar-collapse ui-corner-all" role="button"><span class="ui-icon ui-icon-triangle-1-n" title="Collapse"></span></a>');

            uiDialogTitlebar.append('<a href="#" class="dialog-dockleft ui-dialog-titlebar-dockleft ui-corner-all" role="button"><span class="ui-icon ui-icon-triangle-1-w" title="Dock Left"></span></a>');
            uiDialogTitlebar.append('<a href="#" class="dialog-dockright ui-dialog-titlebar-dockright ui-corner-all" role="button"><span class="ui-icon ui-icon-triangle-1-e" title="Dock Right"></span></a>');

            // Dock Button
            this.uiDialogTitlebarDock = $('.dialog-dock', uiDialogTitlebar).hover(
                function ()
                {
                    $(this).addClass('ui-state-hover');
                },
                function ()
                {
                    $(this).removeClass('ui-state-hover');
                }
            )
            .click(
                function ()
                {
                    self.dock('right');
                    return false;
                }
            );

            // Restore Button
            this.uiDialogTitlebarRestore = $('.dialog-restore', uiDialogTitlebar).hover(
                function ()
                {
                    $(this).addClass('ui-state-hover');
                },
                function ()
                {
                    $(this).removeClass('ui-state-hover');
                }
            )
            .click(
                function ()
                {
                    self.restore();
                    return false;
                }
            )
            .hide();

            // Expand Button
            this.uiDialogTitlebarExpand = $('.dialog-expand', uiDialogTitlebar).hover(
                function ()
                {
                    $(this).addClass('ui-state-hover');
                },
                function ()
                {
                    $(this).removeClass('ui-state-hover');
                }
            )
            .click(
                function ()
                {
                    self.expand();
                    return false;
                }
            );

            // Collapse Button
            this.uiDialogTitlebarCollapse = $('.dialog-collapse', uiDialogTitlebar).hover(
                function ()
                {
                    $(this).addClass('ui-state-hover');
                },
                function ()
                {
                    $(this).removeClass('ui-state-hover');
                }
            )
            .click(
                function ()
                {
                    self.collapse();
                    return false;
                }
            )
            .hide();

            // Dockleft Button
            this.uiDialogTitlebarDockLeft = $('.dialog-dockleft', uiDialogTitlebar).hover(
                function ()
                {
                    $(this).addClass('ui-state-hover');
                },
                function ()
                {
                    $(this).removeClass('ui-state-hover');
                }
            )
            .click(
                function ()
                {
                    self.dock('left');
                    return false;
                }
            )
            .hide();

            // Dockright Button
            this.uiDialogTitlebarDockRight = $('.dialog-dockright', uiDialogTitlebar).hover(
                function ()
                {
                    $(this).addClass('ui-state-hover');
                },
                function ()
                {
                    $(this).removeClass('ui-state-hover');
                }
            )
            .click(
                function ()
                {
                    self.dock('right');
                    return false;
                }
            )
            .hide();

        }

    };
    $.extend($.ui.dialog.prototype, {
        dock: function (side)
        {
            if (side === null) { side = 'right'; }

            if (this.options.shownState !== 'docked')
            {
                this.options.shownState = 'docked';
                this.options.lastPosition = this.uiDialog.position();
                this.option("resizable", false);
                this.uiDialogTitlebarExpand.hide();
                this.uiDialogTitlebarCollapse.hide();
                this.option("draggable", false);

                this.uiDialog
                    .css('margin-top', '0px').css('margin-bottom', '0px')
                    .css('padding-top', '0px').css('padding-bottom', '0px')
                    .css('border-top-width', '0px').css('border-bottom-width', '0px');
                //.css('position', 'absolute');

                this.element.show().css('height', '92%');

                this.uiDialogTitlebarDock.hide();
                this.uiDialogTitlebarRestore.show();
            }

            if (side === 'right')
            {
                this.uiDialogTitlebarDockRight.hide();
                this.uiDialogTitlebarDockLeft.show();

                this.uiDialog
					.css('left', 'auto')
					.animate({ height: '100%', right: '0px', top: '0px' }, 200);
            }
            else
            {
                this.uiDialogTitlebarDockLeft.hide();
                this.uiDialogTitlebarDockRight.show();

                this.uiDialog
					.css('right', 'auto')
					.animate({ height: '100%', left: '0px', top: '0px' }, 200);
            }
        },
        restore: function ()
        {
            this.options.shownState = 'collapsed';

            // Prevent the dialog from restoring off the screen
            var windowHeight = $(window).height();
            var dialogTop = parseInt(this.options.lastPosition.top, 10);
            if (32 + dialogTop > windowHeight) { dialogTop = 10; }
            var windowWidth = $(window).width();
            var dialogLeft = parseInt(this.options.lastPosition.left, 10);
            if (300 + dialogLeft > windowWidth) { dialogLeft = 10; }

            this.collapse();
            this.option("draggable", true);

            var paddingLeft = this.uiDialog.css('padding-left');
            var borderLeft = this.uiDialog.css('border-left-width');
            var marginLeft = this.uiDialog.css('margin-left');
            this.uiDialog
                //.css('position', 'fixed')
                .css('margin-top', marginLeft).css('margin-bottom', marginLeft)
                .css('padding-top', paddingLeft).css('padding-bottom', paddingLeft)
                .css('border-top-width', borderLeft).css('border-bottom-width', borderLeft)
                .css('right', 'auto').css('left', dialogLeft).css('top', dialogTop);

            this.uiDialogTitlebarDockLeft.hide();
            this.uiDialogTitlebarDockRight.hide();

            this.uiDialogTitlebarRestore.hide();
            this.uiDialogTitlebarDock.show();
        },
        expand: function ()
        {
            this.options.shownState = 'expanded';

            this.uiDialog.css('height', 'auto');
            this.element.slideDown(400);
            this.option("resizable", true);

            this.uiDialogTitlebarExpand.hide();
            this.uiDialogTitlebarCollapse.show();
        },
        collapse: function ()
        {
            this.options.shownState = 'collapsed';

            this.element.hide();
            this.option("resizable", false);
            this.element.css('height', this.options.originalHeight);
            this.uiDialog.css('height', '32px');

            this.uiDialogTitlebarCollapse.hide();
            this.uiDialogTitlebarExpand.show();
        }
    });
    $.ui.dialog.defaults = $.extend({}, $.ui.dialog.defaults, { enableDock: false });


    // jQueryUI 1.9.2 introduced an issue when using ui.sortable's connectWith option, as described with a solution here: http://stackoverflow.com/questions/14514359/jquery-sortable-breaks-when-using-connectwith-on-sortable-grids
    // this is the overridden method with the fix applied
    $.ui.sortable.prototype._contactContainers = function(event)
    {

        // get innermost container that intersects with item
        var innermostContainer = null, innermostIndex = null;

        for (var i = this.containers.length - 1; i >= 0; i--)
        {

            // never consider a container that's located within the item itself
            if ($.contains(this.currentItem[0], this.containers[i].element[0]))
                continue;

            if (this._intersectsWith(this.containers[i].containerCache))
            {

                // if we've already found a container and it's more "inner" than this, then continue
                if (innermostContainer && $.contains(this.containers[i].element[0], innermostContainer.element[0]))
                    continue;

                innermostContainer = this.containers[i];
                innermostIndex = i;

            } else
            {
                // container doesn't intersect. trigger "out" event if necessary
                if (this.containers[i].containerCache.over)
                {
                    this.containers[i]._trigger("out", event, this._uiHash(this));
                    this.containers[i].containerCache.over = 0;
                }
            }

        }

        // if no intersecting containers found, return
        if (!innermostContainer) return;

        // move the item into the container if it's not there already
        if (this.containers.length === 1)
        {
            this.containers[innermostIndex]._trigger("over", event, this._uiHash(this));
            this.containers[innermostIndex].containerCache.over = 1;
        }
        else if (this.currentContainer != this.containers[innermostIndex]) //bug fix is here
        {
            //When entering a new container, we will find the item with the least distance and append our item near it
            var dist = 10000;
            var itemWithLeastDistance = null;
            var posProperty = this.containers[innermostIndex].floating ? 'left' : 'top';
            var sizeProperty = this.containers[innermostIndex].floating ? 'width' : 'height';
            var base = this.positionAbs[posProperty] + this.offset.click[posProperty];
            for (var j = this.items.length - 1; j >= 0; j--)
            {
                if (!$.contains(this.containers[innermostIndex].element[0], this.items[j].item[0])) continue;
                if (this.items[j].item[0] == this.currentItem[0]) continue;
                var cur = this.items[j].item.offset()[posProperty];
                var nearBottom = false;
                if (Math.abs(cur - base) > Math.abs(cur + this.items[j][sizeProperty] - base))
                {
                    nearBottom = true;
                    cur += this.items[j][sizeProperty];
                }

                if (Math.abs(cur - base) < dist)
                {
                    dist = Math.abs(cur - base);
                    itemWithLeastDistance = this.items[j];
                    this.direction = nearBottom ? "up" : "down";
                }
            }

            if (!itemWithLeastDistance && !this.options.dropOnEmpty) //Check if dropOnEmpty is enabled
                return;

            this.currentContainer = this.containers[innermostIndex];
            itemWithLeastDistance ? this._rearrange(event, itemWithLeastDistance, null, true) : this._rearrange(event, null, this.containers[innermostIndex].element, true);
            this._trigger("change", event, this._uiHash());
            this.containers[innermostIndex]._trigger("change", event, this._uiHash(this));

            //Update the placeholder
            this.options.placeholder.update(this.currentContainer, this.placeholder);

            this.containers[innermostIndex]._trigger("over", event, this._uiHash(this));
            this.containers[innermostIndex].containerCache.over = 1;
        }

    };

}(jQuery));