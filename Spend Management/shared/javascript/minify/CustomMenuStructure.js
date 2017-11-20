(function () {
    var scriptName = 'CustomMenuStructure';
    var deletedMenuList = null;

    var isValidDataPost = false;

    function execute() {
        SEL.registerNamespace("SEL.CustomMenuStructure");
        SEL.CustomMenuStructure = {
            Elements: {
                menuTreeData: null
            },
            Tree: {
                Variables: {
                    ScrollOffset: null,
                    EasyTrees: [],
                    newItemCount: 1,
                    DefaultMenuName: "Custom Menu"
                },
                CreateEasyTree: function (easyTreeDiv, dataDiv) {
                    SEL.CustomMenuStructure.Tree.Variables.EasyTrees.push($('#' + easyTreeDiv).easytree({
                        data: dataDiv,
                        disableIcons: true,
                        enableDnd: true,
                        sortByOrderId: true,
                        opening: function () {
                            SEL.CustomMenuStructure.Tree.Variables.ScrollOffset = $('#' + easyTreeDiv + '>ul').scrollTop();
                        },
                        built: function (nodes) {
                            if (SEL.CustomMenuStructure.Tree.Variables.ScrollOffset !== null) {
                                $('#' + easyTreeDiv + '>ul').scrollTop(SEL.CustomMenuStructure.Tree.Variables.ScrollOffset);
                                $('#' + easyTreeDiv + '>ul').show();
                            }
                        },
                        canDrop: function (event, nodes, isSourceNode, source, isTargetNode, target) {
                            //Admin must not be able to rearrange nodes on Greenlights page while selecting a menu for a view 
                            //Check for duplicate menu's and disable drag-drop
                            if (window.location.pathname === "/shared/admin/aecustomentity.aspx" || SEL.CustomMenuStructure.Tree.CheckForDuplicateMenuNames(source.id, source.text, target.id)) {
                                return false;
                            }
                            if (isTargetNode && source.IsSystemMenu !== true) {
                                return true;
                            }
                            return false;
                        },
                        dropped: function (event, nodes, isSourceNode, source, isTargetNode, target) {
                            if (isTargetNode) {
                                source.ParentId = target.internalId;
                                source.Order = target.children.length;
                                SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].rebuildTree();
                                $('#' + source.id).addClass("isEdited");
                                //skip editing for newly created created menu's
                                if (source.internalId.indexOf('New') == -1) {
                                    $('#editedItems').append($('#' + source.id).parent().html());
                                }
                                $('.easytree-container #' + source.id).removeClass("isEdited");
                            }
                        }
                    }));
                },

                ResetNode: function (sourceNode) {
                    sourceNode.text = "Custom Menu 0" + SEL.CustomMenuStructure.Tree.Variables.newItemCount;
                    sourceNode.id = null;
                    sourceNode.children = null;
                    sourceNode.internalId = "New" + SEL.CustomMenuStructure.Tree.Variables.newItemCount;
                    sourceNode.liClass = "field f1 newMenuStructure";
                    sourceNode.isLazy = false;
                    sourceNode.description = null;
                    sourceNode.IsSystemMenu = false;
                    sourceNode.IconName = null;
                    SEL.CustomMenuStructure.Tree.Variables.newItemCount++;
                    return sourceNode;
                },

                SaveMenuStructure: function () {
                    // Creating Json array object for Ajax call
                    var menuJsonStructure = '{' + SEL.CustomMenuStructure.Tree.JsonBuilder('.isEdited', 'Edited') + ',' + SEL.CustomMenuStructure.Tree.JsonBuilder('.newMenuStructure', 'New') + ',' + SEL.CustomMenuStructure.Tree.JsonBuilder('deletedMenuList', 'Deleted') + '}';
                    if (isValidDataPost) {
                        SEL.Data.Ajax({
                            serviceName: "SvcCustomMenu",
                            methodName: "ManageCustomMenu",
                            data: {
                                "customMenu": [menuJsonStructure]
                            },
                            success: function (data) {
                                if (data.d == 0) {
                                    window.location = "/GreenLightAdminMenu.aspx";
                                } else if (data.d) {
                                    var failedItemsJson = jQuery.parseJSON(data.d).split(",");
                                    var failedItems = "Cannot update:<br/> <div class='errorModalContent'>";
                                    $.each(failedItemsJson, function (i) {
                                        failedItems += "\n\u2022 " + failedItemsJson[i] + "<br/>";
                                    });
                                    SEL.MasterPopup.ShowMasterPopup(failedItems + '</div>');
                                } else {
                                    SEL.MasterPopup.ShowMasterPopup("<div class='errorModalContent'>Could not save data</div>");
                                }
                            }

                        });
                    } else {
                        window.location = "/GreenLightAdminMenu.aspx";
                    }
                },

                JsonBuilder: function (mode, label) {
                    var count = 0;
                    var jsonMenuObj = '"' + label + '":[';
                    var menuObj = {};
                    if (label == "Deleted" && deletedMenuList) {
                        var deletedMenuItems = deletedMenuList.split('|');
                        for (var item = 0; item < deletedMenuItems.length; item++) {
                            if (deletedMenuItems[item].toString().indexOf('New') == -1) {
                                isValidDataPost = true;
                                menuObj.ID = deletedMenuItems[item];
                                jsonMenuObj = jsonMenuObj + JSON.stringify(menuObj);
                                if (item < deletedMenuItems.length - 1) {
                                    jsonMenuObj = jsonMenuObj + ',';
                                }
                            }
                        };
                    } else if (label != "Deleted") {
                        var uniqueIdList = [];
                        $(mode).each(function () {
                            count++;
                            var dataHolder = $(this);
                            if (jQuery.inArray(dataHolder.attr('id'), uniqueIdList) == -1) {
                                isValidDataPost = true;
                                var deleteNode = SEL.CustomMenuStructure.Tree.GetAttributeValuesInNode(dataHolder.attr('id'));
                                if (deleteNode) {
                                    menuObj.ID = deleteNode.internalId;
                                    menuObj.Name = deleteNode.text;
                                    menuObj.Description = deleteNode.Description;
                                    if (deleteNode.ParentId.toString().indexOf('New') == -1) {
                                        menuObj.ParentId = deleteNode.ParentId;
                                    } else {
                                        menuObj.ReferenceDynamicParentId = deleteNode.ParentId;
                                    }
                                    menuObj.IconName = deleteNode.IconName;
                                    menuObj.Order = deleteNode.Order;
                                    jsonMenuObj = jsonMenuObj + JSON.stringify(menuObj);
                                    if (count < $(mode).length) {
                                        jsonMenuObj = jsonMenuObj + ',';
                                    }
                                    uniqueIdList.push(dataHolder.attr('id'));
                                }
                            }

                        });
                    }
                    jsonMenuObj = jsonMenuObj + ']';
                    return jsonMenuObj;
                },

                GetAttributeValuesInNode: function (nodeId) {
                    return SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode(nodeId);
                },

                CheckForDuplicateMenuNames: function (node, currentValue, parentId) {
                    var found = false;
                    var parentNode = parentId || $('#' + node.id).parent().parent().prev().attr('id');
                    parentNode = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode(parentNode);
                    if (parentNode!=null && parentNode.children) {
                        $.each(parentNode.children, function (key, value) {
                            if (currentValue.toLowerCase() == value.text.toLowerCase() && node.id != value.id) {
                                found = true;
                                return false;
                            }
                        });
                    }
                    return found;
                },

                PageLoad: function () {
                    // Disable return(Enter) key
                    function stopRKey(enterKeyEvent) {
                        enterKeyEvent = (enterKeyEvent) ? enterKeyEvent : ((event) ? event : null);
                        var node = (enterKeyEvent.target) ? enterKeyEvent.target : ((enterKeyEvent.srcElement) ? enterKeyEvent.srcElement : null);
                        if ((enterKeyEvent.keyCode == 13) && (node.type == "text")) { return false; }
                    }
                    document.onkeypress = stopRKey;
                    $(window).load(function () {
                        $('.menuPreLoader').hide();
                        $('#menuWrapper').fadeIn();
                    });
                    // Initialize Easy Tree and editor
                    var checkEdit = true;
                    SEL.CustomMenuStructure.Tree.CreateEasyTree('CustomMenuTree', SEL.CustomMenuStructure.Elements.menuTreeData);

                    //Real time editor update for Menu name and description
                    $('#txtMenuName').on({
                        'focusout': function () {
                            if (!$(this).val().trim().length) {
                                SEL.MasterPopup.ShowMasterPopup("Menu name cannot be blank");
                                $("#txtMenuName").focus();
                            }
                            var currentValue = $(this).val();
                            var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode($('#tooltipCustomMenuHolder').attr('data-node'));
                            checkEdit = true;
                            if (SEL.CustomMenuStructure.Tree.CheckForDuplicateMenuNames(node, currentValue)) {
                                SEL.MasterPopup.ShowMasterPopup("There is already a menu with the name " + currentValue);
                                checkEdit = false;
                            }
                        },
                        'keyup input': function () {
                            if ($(this).val()) {
                                var targetNode = $('#tabs').attr('data-related');
                                $('#' + targetNode).find('.easytree-title').text($(this).val());
                                var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode(targetNode);
                                node.text = $(this).val();
                                checkEdit = true;
                                if (SEL.CustomMenuStructure.Tree.CheckForDuplicateMenuNames(node, node.text)) {
                                    checkEdit = false;
                                }
                            }
                        }
                    });

                    $('#txtMenuName').change(function () {
                        if ($(this).val()) {
                            var menuLength = $(this).val().length;
                            var allowedLength = $(this).attr('maxlength');
                            if (menuLength <= allowedLength) {
                                var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode($('#tabs').attr('data-related'));
                                node.text = $(this).val();
                            }
                        }
                    });
                    $('#txtviewdescription').change(function () {
                        var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode($('#tabs').attr('data-related'));
                        node.Description = $(this).val();
                    });
                    $('#txtviewdescription').on('keyup input', function () {
                        var descriptionLength = $(this).val().length;
                        var allowedLength = $(this).attr('textareamaxlength');
                        var text = $(this).val();
                        if (descriptionLength >= allowedLength) {
                            var updatedDescription = text.substr(0, allowedLength);
                            $(this).val(updatedDescription);
                        }
                        var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode($('#tabs').attr('data-related'));
                        node.Description = $(this).val();
                    });

                    //Icon section
                    $(document).on('click', '.viewPreviewIcon', function () {
                        if ($('#tabs').attr('data-related')) {
                            var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode($('#tabs').attr('data-related'));
                            var src = $(this).attr('src').split('/');
                            var imageName = src[src.length - 1];
                            node.IconName = imageName;
                        }
                    });

                    //Disable inputs on Page Load
                    $('#tabs :input').prop("disabled", true);
                    $('.modalcontentssmall').addClass('modalpanelOpacityControl');
                    //On click of the menu title trigger edit mode
                    $(document).on('click', '.easytree-node', function () {
                        var currentNode = SEL.CustomMenuStructure.Tree.GetAttributeValuesInNode($(this).attr('id'));
                        var activeNode = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode($(this).attr('id'));
                        $('#tooltipCustomMenuHolder').attr('data-id', currentNode.internalId);
                        $('#tooltipCustomMenuHolder').attr('data-node', currentNode.id);
                        //Disable delete for System menu's and Menu's having sub menu's under it
                        if (checkEdit) {
                            var currentNodeHelper = $('#' + currentNode.id);
                            $('#txtviewdescription').val(activeNode.Description);
                            $('#txtMenuName').val(activeNode.text);
                            SEL.CustomMenuStructure.Icon.SetSelectedIcon(activeNode.IconName);
                            if (!activeNode.IsSystemMenu) {
                                $('#tabs').attr('data-related', activeNode.id);
                                $('#tabs :input').prop("disabled", false);
                                $('.modalcontentssmall').removeClass('modalpanelOpacityControl');
                                SEL.CustomMenuStructure.Icon.SetupPreviewIcons();
                                currentNodeHelper.addClass('isEdited');
                                if (activeNode.internalId.indexOf('New') == -1) {
                                    $('#editedItems').append(currentNodeHelper.parent().html());
                                }
                                currentNodeHelper.removeClass('isEdited');
                                //IE fix
                                $('#txtMenuName').focusout();
                                $('#txtMenuName').focus();
                            } else {
                                $('#tabs :input').prop("disabled", true);
                                $(".viewPreviewIcon").unbind('click');
                                $('.modalcontentssmall').addClass('modalpanelOpacityControl');
                            }

                        } else {
                            SEL.MasterPopup.ShowMasterPopup("There is already a menu with the name " + $('#txtMenuName').val());
                            $('#txtMenuName').focus();
                        }
                    });

                    //Floating tooltip icon clicks for edit/new/delete
                    $('#tooltipCustomMenuHolder img').click(function () {
                        var oldCustomNumber = 0;
                        var newCustomNumber = 0;
                        var customMenuId = $('#tooltipCustomMenuHolder').attr('data-id');
                        // cancel if no tree node is selected
                        if (typeof customMenuId === 'undefined') {
                            return;
                        }
                        var node = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getNode($('#tooltipCustomMenuHolder').attr('data-node'));
                        var currentNodeObj = SEL.CustomMenuStructure.Tree.GetAttributeValuesInNode($('#tooltipCustomMenuHolder').attr('data-node'));
                        if (currentNodeObj.children != null) {
                            for (var count = 0; count < currentNodeObj.children.length; count++) {
                                var currentNodeChildren = currentNodeObj.children[count].text;
                                if (currentNodeChildren.indexOf(SEL.CustomMenuStructure.Tree.Variables.DefaultMenuName) > -1) {
                                    newCustomNumber = parseInt(currentNodeChildren.substring(SEL.CustomMenuStructure.Tree.Variables.DefaultMenuName.length + 2));
                                    if (newCustomNumber >= oldCustomNumber) {
                                        SEL.CustomMenuStructure.Tree.Variables.newItemCount = (1 * newCustomNumber + 1);
                                        oldCustomNumber = parseInt(newCustomNumber);
                                    }
                                }
                            }
                        }

                        switch ($(this).attr('id')) {
                            case "AddNewMenu":
                                if (checkEdit) {
                                    var nodes = SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].getAllNodes();
                                    var sourceNode = jQuery.extend({}, nodes[0]);
                                    sourceNode = SEL.CustomMenuStructure.Tree.ResetNode(sourceNode);
                                    SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].addNode(sourceNode, $('#tooltipCustomMenuHolder').attr('data-node'));
                                    node.isExpanded = true;
                                    sourceNode.ParentId = node.internalId;
                                    sourceNode.Order = node.children.length;
                                    SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].rebuildTree();
                                    $('#' + sourceNode.id).trigger('click');
                                    $('#txtMenuName').focus();
                                } else {
                                    SEL.MasterPopup.ShowMasterPopup("There is already a menu with the name " + $('#txtMenuName').val());
                                    $('#txtMenuName').focus();
                                }
                                break;
                            case "DeleteMenu":
                                if (SEL.CustomMenuStructure.Tree.CheckForDuplicateMenuNames(node, node.text)) {
                                    checkEdit = true;
                                }
                                if (checkEdit) {
                                    // Skip if the user is trying to delete a system menu / menu which has sub menu's under it
                                    if (currentNodeObj.IsSystemMenu || currentNodeObj.children ? true : false) {
                                        if (currentNodeObj.IsSystemMenu) {
                                            SEL.MasterPopup.ShowMasterPopup("<div class='errorModalContent'>System menus cannot be deleted</div>");
                                            return;
                                        }
                                        if (currentNodeObj.children.length) {
                                            SEL.MasterPopup.ShowMasterPopup("<div class='errorModalContent'>Cannot delete the menu  " + currentNodeObj.text + ", as there are sub menus associated to it</div>");
                                            return;
                                        }
                                    }
                                    if (customMenuId.indexOf("New") == -1) {
                                        SEL.Data.Ajax({
                                            serviceName: "SvcCustomMenu",
                                            methodName: "MenuItemHasView",
                                            data: {
                                                "customMenuId": customMenuId
                                            },
                                            success: function (data) {
                                                if (data.d == 1) {
                                                    SEL.MasterPopup.ShowMasterPopup("<div class='errorModalContent'>Cannot delete the menu  " + currentNodeObj.text + ", as there are  GreenLight views associated to it or one of its submenus</div>");
                                                } else {
                                                    SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].removeNode($('#tooltipCustomMenuHolder').attr('data-node'));
                                                    deletedMenuList = deletedMenuList ? deletedMenuList + '|' + $('#tooltipCustomMenuHolder').attr('data-id') : $('#tooltipCustomMenuHolder').attr('data-id');
                                                    $('#tabs :input').prop("disabled", true);
                                                    $(".viewPreviewIcon").unbind('click');
                                                    $('.modalcontentssmall').addClass('modalpanelOpacityControl');
                                                    $('#txtviewdescription').val('');
                                                    $('#txtMenuName').val('');
                                                    SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].rebuildTree();
                                                }
                                            }
                                        });
                                    } else {
                                        SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].removeNode($('#tooltipCustomMenuHolder').attr('data-node'));
                                        deletedMenuList = deletedMenuList ? deletedMenuList + '|' + $('#tooltipCustomMenuHolder').attr('data-id') : $('#tooltipCustomMenuHolder').attr('data-id');
                                        $('#tabs :input').prop("disabled", true);
                                        $(".viewPreviewIcon").unbind('click');
                                        $('.modalcontentssmall').addClass('modalpanelOpacityControl');
                                        $('#txtviewdescription').val('');
                                        $('#txtMenuName').val('');
                                        SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].rebuildTree();
                                    }
                                } else {
                                    SEL.MasterPopup.ShowMasterPopup("There is already a menu with the name " + $('#txtMenuName').val());
                                    $('#txtMenuName').focus();
                                }
                                break;
                            default:
                                if (checkEdit) {
                                    var nodeId = $(this).parent().attr('data-node');
                                    // cancel if no tree node is selected
                                    if (typeof nodeId === 'undefined') {
                                        return;
                                    }
                                    var targetNodeObj;
                                    currentNodeObj = SEL.CustomMenuStructure.Tree.GetAttributeValuesInNode(nodeId);
                                    //Disable sorting for system menu's
                                    if (!currentNodeObj.IsSystemMenu) {
                                        if ($(this).attr('id') === 'sortUp') {
                                            targetNodeObj = SEL.CustomMenuStructure.Tree.GetAttributeValuesInNode($('#' + $(this).parent().attr('data-node')).parent().prev().find('.easytree-node').attr('id'));
                                            if (targetNodeObj && !targetNodeObj.IsSystemMenu) {
                                                targetNodeObj.Order++;
                                                currentNodeObj.Order--;
                                            }
                                        } else {
                                            targetNodeObj = SEL.CustomMenuStructure.Tree.GetAttributeValuesInNode($('#' + $(this).parent().attr('data-node')).parent().next().find('.easytree-node').attr('id'));
                                            if (targetNodeObj && !targetNodeObj.IsSystemMenu) {
                                                targetNodeObj.Order--;
                                                currentNodeObj.Order++;
                                            }
                                        }
                                        //Validate dead ends and update target node
                                        if (targetNodeObj && !targetNodeObj.IsSystemMenu) {
                                            if (targetNodeObj.internalId.indexOf('New') == -1) {
                                                $('#' + targetNodeObj.id).addClass("isEdited");
                                                $('#editedItems').append($('#' + targetNodeObj.id).parent().html());
                                                $('.easytree-container #' + currentNodeObj.id).removeClass("isEdited");
                                            }
                                            if (currentNodeObj.internalId.indexOf('New') == -1) {
                                                $('#' + currentNodeObj.id).addClass("isEdited");
                                                $('#editedItems').append($('#' + $(this).parent().attr('data-node')).parent().html());
                                                $('.easytree-container #' + targetNodeObj.id).removeClass("isEdited");
                                            }
                                            SEL.CustomMenuStructure.Tree.Variables.EasyTrees[0].rebuildTree();
                                        }
                                    } else {
                                        SEL.MasterPopup.ShowMasterPopup("The order of system menus is fixed, they cannot be reordered");
                                    }
                                } else {
                                    SEL.MasterPopup.ShowMasterPopup("There is already a menu with the name " + $('#txtMenuName').val());
                                    $('#txtMenuName').focus();
                                }
                                break;
                        }
                    });
                    $('#CmdOk').click(function (e) {
                        e.preventDefault();
                        if (checkEdit) {
                            SEL.CustomMenuStructure.Tree.SaveMenuStructure();
                        } else {
                            SEL.MasterPopup.ShowMasterPopup("There is already a menu with the name " + $('#txtMenuName').val());
                            $('#txtMenuName').focus();
                        }
                    });

                }
            },
            Cancel: function () {
                window.location = "/GreenLightAdminMenu.aspx";
            },
            Icon: {
                SetupIconSearch: function () {
                    // Custom Icon Area
                    $('.searchBox').val('Search...');
                    $('#viewCustomMenuIconResults').html('')

                    var thisNs = SEL.CustomMenuStructure.Icon;
                    var searchName = $('.searchBox');
                    var searchButton = $('#iconSearchButton');
                    var clearSearchButton = $('#iconSearchRemoveButton');
                    thisNs.SearchFileName('', 0);

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
                            var thisNs = SEL.CustomMenuStructure.Icon;
                            var resultsArea = $('#viewCustomMenuIconResults');
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
                                $('#viewCustomMenuIconResults').remove();
                                newResults.attr('id', 'viewCustomMenuIconResults').appendTo($('#viewCustomMenuIconContainer')).fadeIn(200, function () {
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
                    var previewIcons = $('#viewCustomMenuIconResults .viewPreviewIcon');
                    var width = $(window).width(), height = $(window).height();
                    var originalHeightAndWidth = '42px';
                    var animatedHeightAndWidth = '50px';
                    if ((width <= 1024) && (height <= 768)) {
                        originalHeightAndWidth = '32px';
                        animatedHeightAndWidth = '40px';
                    }
                    previewIcons.unbind('hover').hover(function () {
                        $(this).stop(true, true).animate(
                            {
                                'margin-top': '-8px',
                                'margin-left': '-8px',
                                'height': animatedHeightAndWidth,
                                'width': animatedHeightAndWidth
                            }, 200);
                    },
                        function () {
                            $(this).stop(true, false).animate(
                                {
                                    'margin-top': '0px',
                                    'margin-left': '0px',
                                    'height': originalHeightAndWidth,
                                    'width': originalHeightAndWidth
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

                SetSelectedIcon: function (iconName) {
                    if (!iconName) {
                        iconName = 'window_dialog.png';
                    }
                    $('.selectedIcon').attr('src', StaticLibPath + '/icons/48/new/' + iconName);
                    $('#selectedIconName').html(iconName);
                }
            }
        };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    } else {
        execute();
    }

}());
