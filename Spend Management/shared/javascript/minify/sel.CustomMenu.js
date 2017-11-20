(function(SEL, $,cu,$g) {
    var dialog, form;
    var scriptName = "CustomMenu";

    function execute() {
        SEL.registerNamespace("SEL.CustomMenu");
        SEL.CustomMenu = {
            DomIDs: {
                Menu: {
                    Icon: {
                        SearchFileName: null,
                        
                    },
                    },
                Attributes: {
                    name: null,
                    description: null,
                    treename: null,
                    IconName: 'window_dialog.png',
                    menuId: 0
                    
            },

            },
            Ids: {
                
            },

            zIndices:
            {
                
                Views:
                {
                    Modal: 10000,
                },

                Misc:
               {
                   InformationMessage: function () { return SEL.CustomMenu.zIndices.Views.Modal + 25; }
               }
            },

            Misc:
            {
                LoadingScreenCancelled: false,

                ShowInformationMessage: function (loadingText) {
                    if (SEL.CustomMenu.Misc.LoadingScreenCancelled === false && $('#loadingArea').length === 0) {
                        var loadingTextObj = $('<span id="loadingArea">' + loadingText + '</span>');

                        loadingTextObj.css('zIndex', SEL.CustomMenu.zIndices.Misc.InformationMessage());

                        loadingTextObj.css('left', ($(window).width() / 2) - 75).css('top', ($(window).height() / 2) - 90);

                        $('#divPages').append(loadingTextObj);
                    }
                },

                ErrorHandler: function (data) {
                    SEL.CustomMenu.Misc.LoadingScreenCancelled = true;
                    $('#loadingArea').remove();

                    SEL.Common.WebService.ErrorHandler(data);
                }
            },


            Messages:
            {
                DuplicateName: 'The Custom menu name you have entered already exists.',
                DeleteSure: 'Are you sure you wish to delete the selected Custom menu?',
                SelfParent: 'Custom menu cannot parent for itself.',
                CustomMenuSameAsViewName: 'Custom View with the same name is already exists, please use another name.',

            },

            ShowDialog: function () {
                SEL.CustomMenu.ShowIcon(SEL.CustomMenu.DomIDs.Attributes.IconName);
              $find(modThreshold).show();
            },

            ShowIcon: function (fileName) {
                var iconUrl = StaticLibPath + '/icons/48/plain/' + fileName;
                SEL.CustomMenu.Icon.SetSelectedIcon(iconUrl, fileName);
                SEL.CustomMenu.Icon.SearchFileName('', 0);
                SEL.CustomMenu.Icon.SetupIconSearch();
            },

           
            HideDialog: function () {
                $g(SEL.CustomMenu.DomIDs.Menu.Icon.SearchFileName).value = '';
                $g(SEL.CustomMenu.DomIDs.Attributes.name).value = '';
                $g(SEL.CustomMenu.DomIDs.Attributes.description).value = '';
                $g(SEL.CustomMenu.DomIDs.Attributes.treename).value = 'Home';
                SEL.CustomMenu.DomIDs.Attributes.IconName = 'window_dialog.png';
                SEL.CustomMenu.DomIDs.Attributes.menuId = 0;
                $find(modThreshold).hide();
            },

            Menu: {
                
                Save: function () {
                    if (validateform('vgCustomMenu') === false) {
                        return;
                    }
                    setTimeout(function () {SEL.CustomMenu.Misc.ShowInformationMessage('Saving...'); }, 450);
                    var menuname = document.getElementById(txtCustomMenuName).value;
                    var description = document.getElementById(txtCustomMenuDescription).value;
                  
                    var viewIcon = null;

                    if ($('#selectedIconSpan .selectedIcon').length === 1) {
                        viewIcon = $('#selectedIconSpan .selectedIcon').data('iconName');
                    }
                    var id = SEL.CustomMenu.DomIDs.Attributes.menuId;
                    Spend_Management.svcCustomMenu.SaveCustomMenu(cu.AccountID, id, menuname, description, parentId, viewIcon, true,
                        function (data) {

                            if (data == -1) {
                               SEL.CustomMenu.Misc.LoadingScreenCancelled = true;
                                $('#loadingArea').remove();
                                SEL.MasterPopup.ShowMasterPopup(SEL.CustomMenu.Messages.DuplicateName);
                                return;
                            }
                            if (data == -2) {
                                SEL.CustomMenu.Misc.LoadingScreenCancelled = true;
                                $('#loadingArea').remove();
                                SEL.MasterPopup.ShowMasterPopup(SEL.CustomMenu.Messages.SelfParent);
                                return;
                            }
                            if (data == -3) {
                                SEL.CustomMenu.Misc.LoadingScreenCancelled = true;
                                $('#loadingArea').remove();
                                SEL.MasterPopup.ShowMasterPopup(SEL.CustomMenu.Messages.CustomMenuSameAsViewName);
                                return;
                            }
                            if (data>=1) {
                                SEL.CustomMenu.HideDialog();
                                var ch = window.location.toString().contains("#");
                                if (ch) {
                                    window.location = "Custom_menu.aspx";
                                } else {
                                    window.location.reload();

                                }

                            }
                        },
                            SEL.CustomMenu.Misc.ErrorHandler
                    );
                   
                },

                Edit: function (id) {
                    SEL.CustomMenu.DomIDs.Attributes.IconName = 'add2.png';
                    setTimeout(function () { SEL.CustomMenu.Misc.ShowInformationMessage('Loading...'); }, 300);
                    Spend_Management.svcCustomMenu.getCustomeMenu(cu.AccountID, id,
                    function (data) {
                        $g(SEL.CustomMenu.DomIDs.Menu.Icon.SearchFileName).value = '';
                        $g(SEL.CustomMenu.DomIDs.Attributes.name).value = data.customNenuName;
                        $g(SEL.CustomMenu.DomIDs.Attributes.description).value = data.description;
                        $g(SEL.CustomMenu.DomIDs.Attributes.treename).value = data.parentName;
                        SEL.CustomMenu.DomIDs.Attributes.IconName = data.menuIconName;
                        parentId = data.parentId;
                        SEL.CustomMenu.DomIDs.Attributes.menuId = id;
                        SEL.CustomMenu.ShowDialog();
                    },
                    SEL.CustomMenu.Misc.ErrorHandler
                    );

                  
                },

                Delete: function (id) {
                    if (confirm(SEL.CustomMenu.Messages.DeleteSure)) {
                        Spend_Management.svcCustomMenu.deleteCustomMenu(cu.AccountID, id,
                            function (data) {
                                var ch = window.location.toString().contains("#");
                                if (ch) {
                                    window.location = "Custom_menu.aspx";
                                } else {
                                    window.location.reload();

                                }
                            },
                             SEL.CustomMenu.Misc.ErrorHandler
                        );
                    }

                },
            },

            Icon: {
                SetupIconSearch: function () {
                var thisNs = SEL.CustomMenu.Icon;
                var searchName = $('#' + SEL.CustomMenu.DomIDs.Menu.Icon.SearchFileName);
                var searchButton = $('#iconSearchButton');
                var clearSearchButton = $('#iconSearchRemoveButton');

                clearSearchButton.unbind('click').click(function () {
                    searchName.val('Search...');
                    thisNs.SearchFileName('', 0);
                    clearSearchButton.stop(true, true).fadeOut(200);
                });

                searchButton.unbind('click').click(function () {
                    if (searchName.val() !== 'Search...' && searchName.val() !== '') {
                        thisNs.SearchFileName(searchName.val(), 0);
                    }
                });

                searchName.unbind('click').click(function () {
                    if (searchName.val() === 'Search...') {
                        searchName.val('');
                    }
                });

                searchName.unbind('keypress.iconSearch').bind('keypress.iconSearch', function (e) {
                    if (e.which == 13) //Enter keycode
                    {
                        // Prevent the Enter key from performing any default action
                        e.preventDefault();

                        thisNs.SearchFileName(searchName.val(), 0);

                        if ($.trim(searchName.val()) === '') {
                            clearSearchButton.stop(true, true).fadeOut(200);
                        }
                        else {
                            clearSearchButton.stop(true, true).fadeIn(200);
                        }
                    }
                });

                searchName.unbind('blur').blur(function () {
                    if ($.trim(searchName.val()) === '') {
                        searchName.val('Search...');
                        clearSearchButton.stop(true, true).fadeOut(200);
                    }
                    else {
                        clearSearchButton.stop(true, true).fadeIn(200);
                    }
                });

                $('#iconResultsLeft, #iconResultsRight').disableSelection().unbind('click').click(function () {
                    if ($(this).hasClass('active') && $(this).data('fileName') !== undefined && $(this).data('startFrom') !== undefined) {
                        thisNs.SearchFileName($(this).data('fileName'), $(this).data('startFrom'));
                    }
                });

                $('#iconResultsLeft, #iconResultsRight').unbind('hover').hover(function () {
                    if ($(this).hasClass('active')) {
                        $(this).stop(true, false).animate({ 'font-size': '38pt' }, 200).data('large', true);
                    }
                }, function () {
                    if ($(this).hasClass('active')) {
                        $(this).stop(true, false).animate({ 'font-size': '28pt' }, 200).data('large', false);
                    }
                    else {
                        $(this).stop(true, false).animate({ 'font-size': '20pt' }, 200).data('large', false);
                    }
                });


            },

                SearchFileName: function (fileName, startFrom) {
                $.ajax({
                    type: "POST",
                    url: window.appPath + "/shared/webServices/svcCustomEntities.asmx/SearchStaticIconsByFileName",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: '{ "fileName":"' + fileName.replace(/\\/g, '').replace(/\"/g, '') + '", "searchStartNumber":"' + startFrom + '" }',
                    success: function (data) {
                        var thisNs = SEL.CustomMenu.Icon;
                        var resultsArea = $('#viewIconResults');
                        var newResults = $(document.createElement('span')).css('display', 'none');

                        resultsArea.fadeOut(200);

                        thisNs.Refresh(fileName, data.d.FurtherResults, startFrom, data.d.ResultEndNumber);

                        if (data.d.MenuIcons.length < 11) {
                            newResults.css('margin-top', '90px');
                        }
                        else if (data.d.MenuIcons.length < 21) {
                            newResults.css('margin-top', '55px');
                        }
                        else {
                            newResults.css('margin-top', '15px');
                        }

                        var icons = data.d.MenuIcons;
                        // The following is done in native js to try and keep IE running as fast as possible
                        for (var i = 0, len = icons.length; i < len; i++) {
                            var img = document.createElement('img');

                            img.setAttribute('src', icons[i].IconUrl);
                            img.setAttribute('class', 'viewPreviewIcon');
                            img.setAttribute('className', 'viewPreviewIcon');

                            $(img).data('iconName', icons[i].IconName);

                            var imgSpan = document.createElement('span');

                            imgSpan.setAttribute('class', 'iconContainer');
                            imgSpan.setAttribute('className', 'iconContainer');

                            imgSpan.appendChild(img);

                            newResults.append(imgSpan);
                        }

                        resultsArea.promise().done(function () {
                            $('#viewIconResults').remove();
                            newResults.attr('id', 'viewIconResults').appendTo($('#viewCustomIconContainer')).fadeIn(200, function () {
                                thisNs.SetupPreviewIcons();
                            });
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        SEL.Common.WebService.ErrorHandler(errorThrown);
                    }
                });
            },

            SetupPreviewIcons: function () {
                var previewIcons = $('#viewIconResults .viewPreviewIcon');

                previewIcons.unbind('hover').hover(function () {
                    $(this).stop(true, true).animate(
                        {
                            'margin-top': '-8px',
                            'margin-left': '-8px',
                            'height': '64px',
                            'width': '64px'
                        }, 200);
                },
                    function () {
                        $(this).stop(true, false).animate(
                            {
                                'margin-top': '0px',
                                'margin-left': '0px',
                                'height': '48px',
                                'width': '48px'
                            }, 200);
                    });

                previewIcons.unbind('click').click(function () {
                    var selectedIconArea = $('#selectedIconSpan');
                    var currentIcon = $('#selectedIconSpan .selectedIcon');

                    if (currentIcon.length === 1) {
                        var iconName = $(this).data('iconName');
                        var iconNameSpan = $('#selectedIconName');

                        iconNameSpan.stop(true, true).fadeOut(150, function () {
                            iconNameSpan.html(iconName);
                            iconNameSpan.fadeIn(150);
                        });

                        var menuIcon = $('<img></img>').attr('src', $(this).attr('src')).addClass('selectedIcon');

                        menuIcon.css('display', 'none').data('iconName', iconName);

                        currentIcon.css('position', 'absolute');

                        selectedIconArea.append(menuIcon);

                        currentIcon.fadeOut(500, function () {
                            currentIcon.remove();
                        });

                        menuIcon.fadeIn(500);
                    }
                });
            },

            Refresh: function (fileName, furtherResults, resultStartNumber, resultEndNumber) {
                var resultsRight = $('#iconResultsRight');
                var resultsLeft = $('#iconResultsLeft');

                if (furtherResults) {
                    if (resultsRight.data('large') !== true) {
                        resultsRight.addClass('active').stop(true, false).animate({ 'font-size': '28pt' }, 200);
                    }
                }
                else {
                    resultsRight.removeClass('active').stop(true, false).animate({ 'font-size': '20pt' }, 200);
                }

                if (resultEndNumber > 30) {
                    if (resultsLeft.data('large') !== true) {
                        resultsLeft.addClass('active').stop(true, false).animate({ 'font-size': '28pt' }, 200);
                    }
                }
                else {
                    $('#iconResultsLeft').removeClass('active').stop(true, false).animate({ 'font-size': '20pt' }, 200);
                }

                resultsRight.data('startFrom', resultEndNumber);
                resultsLeft.data('startFrom', resultStartNumber - 30);
                $('#iconResultsRight, #iconResultsLeft').data('fileName', fileName);
            },

            SetSelectedIcon: function (iconUrl, iconName) {
                var menuIcon = $('<img></img').attr('src', iconUrl).addClass('selectedIcon');

                menuIcon.data('iconName', iconName);

                $('#selectedIconSpan').html('').append(menuIcon);

                $('#selectedIconName').html(iconName);
            }
        }
        };

    }


    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }

}(SEL, jQuery, CurrentUserInfo,$g));

