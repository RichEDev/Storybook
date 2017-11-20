(function(SEL, moduleNameHTML, appPath)
{
    var scriptName = "templateName";

    function execute()
    {
        SEL.registerNamespace("SEL.Template");
        SEL.Template =
        {
            IDs:
            {
                TemplateItemId: null,
                TemplateItemSubItemId: null
            },
            
            DomIDs:
            {
                Summary:
                {
                    Grid: null
                },
                
                General:
                {
                    TemplateItemName: null,
                    TemplateItemDescription: null,
                    TemplateItemNameRequiredValidator: null
                },
                
                TemplateItemSubItem:
                {
                    TemplateItemSubItemName: null,
                    TemplateItemSubItemDescription: null,
                    TemplateItemSubItemNameRequiredValidator: null,
                    Modal: null,
                    Panel: null,
                    Grid: null
                }
            },
                       
            SetupEnterKeyBindings: function()
            {
                // Base Save
                SEL.Common.BindEnterKeyForSelector('.primaryPage', SEL.Template.TemplateItem.Save);
                // Sub Item Save
                SEL.Common.BindEnterKeyForSelector('#' + SEL.Template.DomIDs.TemplateSubItem.Panel, SEL.Template.TemplateSubItem.Save);
                
                $(document).keydown(function (e) {
                    if (e.keyCode === 27) // esc
                    {
                        e.preventDefault();
                        if ($g(SEL.Template.DomIDs.TemplateSubItem.Panel).style.display == '')
                        {
                            SEL.Template.TemplateSubItem.Cancel();
                            return;
                        }
                    }
                });
            },
            
            TemplateItem:
            {
                Delete: function(templateItemId)
                {
                    if (confirm('Are you sure you want to delete this template item?'))
                    {
                        $.ajax({
                            url: appPath + '/shared/webServices/svcTemplates.asmx',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ \"templateItemId\": " + templateItemId + " }",
                            success: function(r)
                            {
                                if (r.d === -1)
                                {
                                    SEL.MasterPopup.ShowMasterPopup('This template item cannot currently be deleted because...', 'Message from ' + moduleNameHTML);
                                    return;
                                }

                                SEL.Template.TemplateItem.RefreshGrid();
                            },
                            error: function(xmlHttpRequest, textStatus, errorThrown)
                            {
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    }
                },
                
                Save: function ()
                {
                    if (validateform('vgMain') === false)
                    {
                        return false;
                    }

                    var itemName = $g(SEL.Template.DomIDs.General.TemplateItemName).value;
                    var itemDescription = $g(SEL.Template.DomIDs.General.TemplateDescription).value;
                    
                    $.ajax({
                        url: appPath + '/shared/webServices/svcTemplates.asmx/SaveTemplateItem',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ templateItemId: " + SEL.Template.IDs.TemplateItemId + ", templateItemName: \"" + itemName + "\", itemDescription: \"" + itemDescription + "\" }",
                        success: function (r)
                        {
                            if (r.d === -1)
                            {
                                SEL.MasterPopup.ShowMasterPopup('The Template item name already exists.', 'Message from ' + moduleNameHTML);
                                return false;
                            }
                            document.location = "Template.aspx";
                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown)
                        {
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },

                Cancel: function()
                {
                    document.location = "Template.aspx";
                },

                RefreshGrid: function ()
                {
                    SEL.Grid.refreshGrid('gridTemplate', SEL.Grid.getCurrentPageNum('gridTemplate'));
                }
            },
            
            TemplateItemSubItem:
            {
                Focus: function ()
                {

                },
                
                Edit: function(subItemId)
                {

                },
                
                New: function ()
                {

                },
                
                Save: function (templateSubItemId)
                {
                    $.ajax({
                        url: appPath + '/shared/webServices/svcTemplate.asmx/SaveTemplateSubItem',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: "{ \"templateSubItemId\": " + templateSubItemId + " }",
                        success: function(r)
                        {

                        },
                        error: function(xmlHttpRequest, textStatus, errorThrown)
                        {
                            errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + moduleNameHTML);
                        }
                    });
                },
                
                RefreshGrid: function ()
                {

                },
                
                Cancel: function()
                {

                },
                
                Delete: function (templateSubItemId)
                {
                    if (confirm('Are you sure?'))
                    {
                        $.ajax({
                            url: appPath + '/shared/webServices/svcTemplate.asmx/DeleteTemplateSubItem',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            data: "{ \"templateSubItemId\": " + templateSubItemId + " }",
                            success: function(r)
                            {

                            },
                            error: function(xmlHttpRequest, textStatus, errorThrown)
                            {
                                errorThrown = errorThrown + ' ' + xmlHttpRequest.responseText;
                                SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.<span style="display:none;">' + errorThrown + '</span>',
                                    'Message from ' + moduleNameHTML);
                            }
                        });
                    }
                },
                
                Modal:
                {
                    Show: function()
                    {
                        SEL.Common.ShowModal(SEL.Template.DomIDs.TemplateItemSubItem.Modal);
                    },
                    
                    Hide: function()
                    {
                        SEL.Common.HideModal(SEL.Template.DomIDs.TemplateItemSubItem.Modal);
                    },

                    Clear: function ()
                    {
                        var modalDoms = SEL.Template.DomIDs.TemplateItemSubItem;

                    }
                }
            }
        };
    }

    if (window.Sys && window.Sys.loader)
    {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}(SEL, moduleNameHTML, appPath));