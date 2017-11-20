/* <summary>Menu Methods</summary> */
(function ()
{
    var scriptName = "Menu";
    function execute()
    {
        SEL.registerNamespace("SEL.Menu");
        SEL.Menu =
        {
            MessageGenericError: "An error occurred while processing your request.",

            SetupFavouritesArea: function ()
            {
                var allMenuItems = $('table[id^="menuitem"]');
                var infoBar = $('#infodiv .infobar');
                var favouritesArea = $('#favouritesArea');

                if (favouritesArea.find('.favMenuItem').length !== 0)
                {
                    SEL.Menu.CalculateLeftMargin(true);
                    infoBar.css('height', '140px');
                    favouritesArea.css('display', '');
                }

                SEL.Menu.MakeMenuItemsDraggable(allMenuItems, infoBar);

                SEL.Menu.MakeMenuFavouritesDraggable(favouritesArea.find('.favMenuItem'));

                SEL.Menu.MakeInfoBarDroppable(infoBar);

                SEL.Menu.MakeFavouritesAreaDroppable(favouritesArea);

                SEL.Menu.SetupInfoBarHover(infoBar, favouritesArea);
            },

            MakeMenuItemsDraggable: function (menuItems, infoBar)
            {
                menuItems.draggable({
                    distance: 20,
                    helper: function ()
                    {
                        var helperObj = $("<span id='favMenuHelper'></span>");
                        var menuImageSource = $(this).find('img').attr("src");
                        var menuTitle = $(this).find('.menuitemtitle').text();
                        var imgSpan = "<span class=\"favMenuImage\"><img src=\"" + menuImageSource + "\" title=\"Item\"/></span>";
                        var titleSpan = "<span class=\"favMenuTitle\">" + menuTitle + "</span>";

                        helperObj.append(imgSpan).append(titleSpan);

                        return helperObj;
                    },
                    start: function ()
                    {
                        if (infoBar.css('height') === '43px')
                        {
                            infoBar.animate({ height: 70 }, 200);
                        }
                    },
                    stop: function ()
                    {
                        if (infoBar.css('height') === '70px')
                        {
                            infoBar.animate({ height: 43 }, 200);
                        }
                    },
                    cursorAt: { top: -5, left: -5 }
                });
            },

            MakeMenuFavouritesDraggable: function (favourites)
            {
                favourites.draggable({
                    distance: 20,
                    helper: function ()
                    {
                        var helper = $(this).clone(false).appendTo($('#pagediv'));

                        // The ID of 'helper' has to be updated in this way to avoid IE7 issues
                        helper[0].id = 'favMenuHelper';

                        return helper;
                    },
                    stop: function ()
                    {
                        if ($(this).draggable().data('dropped') === true)
                        {
                            var menuFavourite = $(this).draggable();
                            var favouritesArea = $('#favouritesArea');
                            var saveIndicator = $("#favouritesSaveIndicator");

                            // show progress indicator
                            saveIndicator.appendTo(favouritesArea).show();

                            if (menuFavourite.attr('id') !== undefined)
                            {
                                $.ajax({
                                    type: "POST",
                                    url: window.appPath + "/shared/webServices/svcMenu.asmx/DeleteMenuFavourite",
                                    dataType: "json",
                                    contentType: "application/json; charset=utf-8",
                                    data: '{ "menuFavouriteID":"' + menuFavourite.attr('id') + '" }',
                                    success: function (data)
                                    {
                                        switch (data)
                                        {
                                            case -1:
                                                // The menu favourite was not found in the database. Therefore, remove it anyway
                                                menuFavourite.remove();
                                                SEL.Menu.CalculateLeftMargin();
                                                saveIndicator.insertAfter(favouritesArea).hide();
                                                return;
                                            case -2:
                                            case -3:
                                                SEL.MasterPopup.ShowMasterPopup('The Menu Favourite could not be deleted.', 'Message from Expenses');
                                                saveIndicator.insertAfter(favouritesArea).hide();
                                                return;
                                        }

                                        menuFavourite.animate({ 'width': '0px' }, 200,
                                            function ()
                                            {
                                                menuFavourite.remove();
                                                SEL.Menu.CalculateLeftMargin();

                                                if (favouritesArea.find('.favMenuItem').length === 0)
                                                {
                                                    favouritesArea.stop(true, true).fadeOut(400);
                                                    $('#infodiv .infobar').stop(true, false).animate({ 'height': '43px' }, '500');
                                                }

                                                // hide progress indicator
                                                saveIndicator.insertAfter(favouritesArea).hide();
                                            });
                                    },
                                    error: function (XMLHttpRequest, textStatus, errorThrown)
                                    {
                                        SEL.Common.WebService.ErrorHandler(errorThrown);

                                        // hide progress indicator
                                        saveIndicator.insertAfter(favouritesArea).hide();
                                    }
                                });
                            }
                        }
                    }
                });
            },

            MakeInfoBarDroppable: function (infoBar)
            {
                var favouritesArea = $('#favouritesArea');
                var favouritesSpan = $('#favouritesContainer');

                infoBar.droppable({
                    accept: 'table[id^="menuitem"]',
                    tolerance: 'pointer',
                    over: function ()
                    {
                        infoBar.stop(true, false).animate({ 'height': '140px' }, '500');

                        favouritesArea.stop(true, true).fadeIn(400);
                    },
                    drop: function (event, ui)
                    {
                        if (favouritesSpan.children().length < 5)
                        {
                            var menuImageSource = ui.draggable.find('img').attr("src");
                            var menuTitle = ui.draggable.find('.menuitemtitle').text();
                            var menuClickUrl = ui.draggable.attr('onclick');
                            // This needs to be changed when we let the user sort when dropping
                            var menuOrder = favouritesSpan.children().length;

                            // Remove () from Check and Pay, Claims etc
                            if (menuTitle.indexOf('Check & Pay Expenses (') >= 0 || menuTitle.indexOf('Current Claims (') >= 0
                                || menuTitle.indexOf('Previous Claims (') >= 0 || menuTitle.indexOf('Submitted Claims (') >= 0)
                                menuTitle = menuTitle.substring(0, menuTitle.indexOf('('));

                            var imgSpan = "<span class=\"favMenuImage\"><img src=\"" + menuImageSource + "\" title=\"" + menuTitle + "\"/></span>";
                            var titleSpan = "<span class=\"favMenuTitle\">" + menuTitle + "</span>";

                            // Check the Menu has not already been added - perhaps put into sepearte function
                            var alreadyExists = false;

                            favouritesSpan.children().each(function (x, favourite)
                            {
                                var fav = $(favourite);

                                if ($.trim(fav.text()) === $.trim(menuTitle))
                                {
                                    if (fav.html().indexOf(menuImageSource) >= 0)
                                    {
                                        alreadyExists = true;
                                        return false;
                                    }
                                }
                            });

                            if (alreadyExists || menuClickUrl.indexOf('window.open(') !== -1)
                                return;

                            var containerSpan = $('<span class="favMenuItem"></span>').append(imgSpan).append(titleSpan);
                            var actualUrl = menuClickUrl;

                            containerSpan.click(function ()
                            {
                                actualUrl = actualUrl.replace("document.location='", "").replace("';", "");

                                document.location = actualUrl;
                            });

                            favouritesSpan.append(containerSpan);
                            SEL.Menu.CalculateLeftMargin();

                            SEL.Menu.MakeMenuFavouritesDraggable($(containerSpan));

                            // show progress indicator
                            var saveIndicator = $("#favouritesSaveIndicator");
                            saveIndicator.appendTo(favouritesArea).show();

                            menuImageSource = menuImageSource.substring(menuImageSource.lastIndexOf('/') + 1, menuImageSource.length);

                            menuClickUrl = menuClickUrl.replace("document.location='" + appPath, '');

                            $.ajax({
                                type: "POST",
                                url: window.appPath + "/shared/webServices/svcMenu.asmx/SaveMenuFavourite",
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                data: '{ "menuTitle":"' + menuTitle + '", "menuImageLocation":"' + menuImageSource
                                + '", "menuOnClickUrl":"' + menuClickUrl + '", "order":"' + menuOrder + '" }',
                                success: function (data)
                                {
                                    switch (data.d)
                                    {
                                        case -1:
                                            // The menu item already exists, remove containerSpan
                                            containerSpan.remove();
                                            SEL.Menu.CalculateLeftMargin();
                                            return;
                                        case -2:
                                        case -3:
                                            SEL.MasterPopup.ShowMasterPopup('The Menu Favourite could not be added.', 'Message from Expenses');
                                            containerSpan.remove();
                                            SEL.Menu.CalculateLeftMargin();
                                            return;
                                    }

                                    containerSpan.attr('id', data.d);

                                    // hide progress indicator
                                    saveIndicator.insertAfter(favouritesArea).hide();
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown)
                                {
                                    SEL.Common.WebService.ErrorHandler(errorThrown);

                                    // hide progress indicator
                                    saveIndicator.insertAfter(favouritesArea).hide();
                                }
                            });
                        }
                    }
                });
            },

            MakeFavouritesAreaDroppable: function (favouritesArea)
            {
                favouritesArea.droppable({
                    out: function (event, ui)
                    {
                        ui.draggable.data('dropped', true);
                    },
                    over: function (event, ui)
                    {
                        ui.draggable.data('dropped', false);
                    }
                });
            },

            SetupInfoBarHover: function (infoBar, favouritesArea)
            {
                infoBar.hover(function ()
                {
                    var delayTime = infoBar.css('height') === '43px' ? 500 : 0;

                    infoBar.stop(true, false).delay(delayTime).animate({ 'height': '140px' }, '500');
                    favouritesArea.stop(true, true).delay(delayTime).fadeIn(400);
                }, function ()
                {
                    if (favouritesArea.find('.favMenuItem').length === 0)
                    {
                        favouritesArea.stop(true, true).fadeOut(400);
                        infoBar.stop(true, false).animate({ 'height': '43px' }, '500');
                    }
                });
            },

            CalculateLeftMargin: function (noAnimation)
            {
                var favouritesSpan = $('#favouritesContainer');

                var childrenWidth = favouritesSpan.children().length * 178;
                var newLeft = childrenWidth / 2;

                favouritesSpan.css('width', childrenWidth + 10 + 'px');

                if (noAnimation === undefined)
                {
                    favouritesSpan.animate({ 'left': -newLeft + 'px' });
                }
                else
                {
                    favouritesSpan.css('left', -newLeft + 'px');
                }
            }
        };
    }

    if (window.Sys && Sys.loader)
    {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
})();
