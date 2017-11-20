(function ($, win, SEL, $g, $f, $e, cu)
{
    var ImageBrowser = {
        init: function (options)
        {
            this.options = options;
            this.SetupIconSearch();
        },

        SetupIconSearch: function ()
        {
            var searchName = $('#' + $.ImageBrowser.options['SearchFileName']);
            var searchButton = $('#iconSearchButton');
            var clearSearchButton = $('#iconSearchRemoveButton');

            clearSearchButton.unbind('click').click(function ()
            {
                searchName.val('Search...');
                ImageBrowser.SetSelectedIcon(ImageBrowser.options['LibraryPath'] + ImageBrowser.options['DefaultIcon'], ImageBrowser.options['DefaultIcon']);
                SearchFileName('', 0);
                clearSearchButton.stop(true, true).fadeOut(200);
            });

            searchButton.unbind('click').click(function ()
            {
                if (searchName.val() !== 'Search...' && searchName.val() !== '')
                {
                    ImageBrowser.SearchFileName(searchName.val(), 0);
                };
            });

            searchName.unbind('click').click(function ()
            {
                if (searchName.val() === 'Search...')
                {
                    searchName.val('');
                };
            });

            searchName.unbind('keypress.iconSearch').bind('keypress.iconSearch', function (e)
            {
                if (e.which == 13) //Enter keycode
                {
                    // Prevent the Enter key from performing any default action
                    e.preventDefault();

                    ImageBrowser.SearchFileName(searchName.val(), 0);

                    if ($.trim(searchName.val()) === '')
                    {
                        clearSearchButton.stop(true, true).fadeOut(200);
                    }
                    else
                    {
                        clearSearchButton.stop(true, true).fadeIn(200);
                    };
                };
            });

            searchName.unbind('blur').blur(function ()
            {
                if ($.trim(searchName.val()) === '')
                {
                    searchName.val('Search...');
                    clearSearchButton.stop(true, true).fadeOut(200);
                }
                else
                {
                    clearSearchButton.stop(true, true).fadeIn(200);
                };
            });

            $('#iconResultsLeft, #iconResultsRight').disableSelection().unbind('click').click(function ()
            {
                if ($(this).hasClass('active') && $(this).data('fileName') !== undefined && $(this).data('startFrom') !== undefined)
                {
                    ImageBrowser.SearchFileName($(this).data('fileName'), $(this).data('startFrom'));
                };
            });

            $('#iconResultsLeft, #iconResultsRight').unbind('hover').hover(function ()
            {
                if ($(this).hasClass('active'))
                {
                    $(this).stop(true, false).animate({ 'font-size': '38pt' }, 200).data('large', true);
                };
            }, function ()
            {
                if ($(this).hasClass('active'))
                {
                    $(this).stop(true, false).animate({ 'font-size': '28pt' }, 200).data('large', false);
                }
                else
                {
                    $(this).stop(true, false).animate({ 'font-size': '20pt' }, 200).data('large', false);
                };
            });

            $("input[id*=btnImageLibraryOk]").unbind('click').click(function ()
            {
                var fileName = $('#selectedIconName').text();
                var dotonated = fileName;
                var filePath = ImageBrowser.options['LibraryPath'] + fileName;

                if (fileName.length > 6)
                {
                    dotonated = fileName.substring(0, 6) + '...';
                }

                $('#' + ImageBrowser.options['PathControl']).val(filePath);
                $('#' + ImageBrowser.options['NameControl']).val(fileName);
                $('#' + ImageBrowser.options['UploadTypeControl']).val('ImageBrowser');
                $('#' + ImageBrowser.options['HyperlinkControl']).css('display', 'none');
                $('#' + ImageBrowser.options['ReplacementControl']).html(dotonated);
                $('#' + ImageBrowser.options['ReplacementControl'])[0].title = fileName;
                $('#' + ImageBrowser.options['ChangeFlagControl']).val('changed');
                if (ImageBrowser.options['IsMandatory'] == 'False')
                {
                    $('#' + ImageBrowser.options['DeleteIconControl']).css('display', '');
                }

                var theValidator = $('#' + ImageBrowser.options['ValidatorControl'])[0];
                theValidator.isvalid = true;
                ValidatorUpdateDisplay(theValidator);
                $f('mdlImageLibrary').hide();
                return false;
            });

            $("input[id*=btnImageLibraryCancel]").unbind('click').click(function ()
            {
                $('#' + ImageBrowser.options['PathControl']).val('');
                $('#' + ImageBrowser.options['NameControl']).val('');
                $f('mdlImageLibrary').hide();
                return false;
            });

            $("input[id*=btnImageLibraryBrowse]").unbind('click').click(function ()
            {
                $('#' + ImageBrowser.options['FileUploadControl']).click();
                return false;
            });

            ImageBrowser.SearchFileName('', 0);
            ImageBrowser.SetSelectedFileNameIfExists(ImageBrowser.options['SelectedIcon']);
        },

        SearchFileName: function (fileName, startFrom)
        {
            $.ajax({
                type: "POST",
                url: window.appPath + "/shared/webServices/svcCustomEntities.asmx/SearchStaticIconsByFileName",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: '{ "fileName":"' + fileName.replace(/\\/g, '').replace(/\"/g, '') + '", "searchStartNumber":"' + startFrom + '" }',
                success: function (data)
                {
                    var resultsArea = $('#viewIconResults');
                    var newResults = $(document.createElement('span')).css('display', 'none');

                    resultsArea.fadeOut(200);
                    ImageBrowser.Refresh(fileName, data.d.FurtherResults, startFrom, data.d.ResultEndNumber);

                    if (data.d.MenuIcons.length < 11)
                    {
                        newResults.css('margin-top', '90px');
                    }
                    else if (data.d.MenuIcons.length < 21)
                    {
                        newResults.css('margin-top', '55px');
                    }
                    else
                    {
                        newResults.css('margin-top', '15px');
                    };

                    var icons = data.d.MenuIcons;

                    // The following is done in native js to try and keep IE running as fast as possible
                    for (var i = 0, len = icons.length; i < len; i++)
                    {
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
                    };

                    resultsArea.promise().done(function ()
                    {
                        $('#viewIconResults').remove();
                        newResults.attr('id', 'viewIconResults').appendTo($('#viewCustomIconContainer')).fadeIn(200, function ()
                        {
                            ImageBrowser.SetupPreviewIcons();
                        });
                    });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    SEL.Common.WebService.ErrorHandler(errorThrown);
                }
            });
        },

        SetSelectedFileNameIfExists: function (fileName)
        {
            $.ajax({
                type: "POST",
                url: window.appPath + "/shared/webServices/svcCustomEntities.asmx/SearchStaticIconsByFileName",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: '{ "fileName":"' + fileName.replace(/\\/g, '').replace(/\"/g, '') + '", "searchStartNumber":"' + 0 + '" }',
                success: function (data)
                {
                    var icon = ImageBrowser.options['DefaultIcon'];
                    if (data.d.MenuIcons.length > 0)
                    {
                        if (ImageBrowser.options['SelectedIcon'] != '')
                        {
                            icon = ImageBrowser.options['SelectedIcon'];
                        }
                        ImageBrowser.SetSelectedIcon(ImageBrowser.options['LibraryPath'] + icon, icon);
                    }
                    else
                    {
                        ImageBrowser.SetSelectedIcon(ImageBrowser.options['LibraryPath'] + icon, icon);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    SEL.Common.WebService.ErrorHandler(errorThrown);
                }
            });
        },

        SetupPreviewIcons: function ()
        {
            var previewIcons = $('#viewIconResults .viewPreviewIcon');

            previewIcons.unbind('hover').hover(function ()
            {
                $(this).stop(true, true).animate(
                    {
                        'margin-top': '-8px',
                        'margin-left': '-8px',
                        'height': '64px',
                        'width': '64px'
                    }, 200);
            },
                function ()
                {
                    $(this).stop(true, false).animate(
                        {
                            'margin-top': '0px',
                            'margin-left': '0px',
                            'height': '48px',
                            'width': '48px'
                        }, 200);
                });

            previewIcons.unbind('click').click(function ()
            {
                var selectedIconArea = $('#selectedIconSpan');
                var currentIcon = $('#selectedIconSpan .selectedIcon');

                if (currentIcon.length === 1)
                {
                    var iconName = $(this).data('iconName');
                    var iconNameSpan = $('#selectedIconName');

                    iconNameSpan.stop(true, true).fadeOut(150, function ()
                    {
                        iconNameSpan.html(iconName);
                        iconNameSpan.fadeIn(150);
                    });

                    var menuIcon = $('<img></img>').attr('src', $(this).attr('src')).addClass('selectedIcon');

                    menuIcon.css('display', 'none').data('iconName', iconName);
                    currentIcon.css('position', 'absolute');
                    selectedIconArea.append(menuIcon);
                    currentIcon.fadeOut(500, function ()
                    {
                        currentIcon.remove();
                    });

                    menuIcon.fadeIn(500);
                };
            });
        },

        Refresh: function (fileName, furtherResults, resultStartNumber, resultEndNumber)
        {
            var resultsRight = $('#iconResultsRight');
            var resultsLeft = $('#iconResultsLeft');

            if (furtherResults)
            {
                if (resultsRight.data('large') !== true)
                {
                    resultsRight.addClass('active').stop(true, false).animate({ 'font-size': '28pt' }, 200);
                };
            }
            else
            {
                resultsRight.removeClass('active').stop(true, false).animate({ 'font-size': '20pt' }, 200);
            };

            if (resultEndNumber > 30)
            {
                if (resultsLeft.data('large') !== true)
                {
                    resultsLeft.addClass('active').stop(true, false).animate({ 'font-size': '28pt' }, 200);
                };
            }
            else
            {
                $('#iconResultsLeft').removeClass('active').stop(true, false).animate({ 'font-size': '20pt' }, 200);
            };

            resultsRight.data('startFrom', resultEndNumber);
            resultsLeft.data('startFrom', resultStartNumber - 30);
            $('#iconResultsRight, #iconResultsLeft').data('fileName', fileName);
        },

        SetSelectedIcon: function (iconUrl, iconName)
        {
            var menuIcon = $('<img></img').attr('src', iconUrl).addClass('selectedIcon');

            menuIcon.data('iconName', iconName);
            $('#selectedIconSpan').html('').append(menuIcon);
            $('#selectedIconName').html(iconName);
        }
    };

    // expose
    $.ImageBrowser = function (element, options)
    {
        ImageBrowser.init($.extend($.ImageBrowser.options, options));
        return this;
    };

    // options
    $.ImageBrowser.options = {
        // the name of the current module
        moduleName: 'Expenses',
        SearchFileName: 'txtViewCustomIconSearch',
        LibraryPath: '/static/icons/48/new/',
        DefaultIcon: 'window_dialog.png',
        SelectedIcon: 'z.lala',
        PathControl: 'txtImageLibraryFilePath',
        NameControl: 'txtImageLibraryFileName',
        FileUploadControl: 'fileUpload',
        ChangeFlagControl: 'txtChangeControl',
        ValidatorControl: 'reqFileUpload'
    };

})(jQuery, window, SEL, $g, $f, $e, CurrentUserInfo);