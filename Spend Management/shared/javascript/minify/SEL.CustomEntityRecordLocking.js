(function (SEL, $g, $f, $e, $ddlValue) {
	var scriptName = "CustomEntityRecordLocking";

    function execute() {
        SEL.registerNamespace("SEL.CustomEntityRecordLocking");
       
        SEL.CustomEntityRecordLocking = {
			DomIDs: {
			    
			},
			Ids: {
                Timeout: 15000,
                UnlockTimer: null,
                LockTimer: null,
                ElementId: null,
                Id: null
			},
            StartLockTimer: function() {
                if (navigator.appVersion.indexOf("MSIE 7.") === -1) {
                    $(window).bind('beforeunload', function () {
                        if ($('#divElementLocking').css('display') === 'none') {
                            SEL.CustomEntityRecordLocking.UnlockElement();
                        }
                    });
                }

                SEL.CustomEntityRecordLocking.RestartLockTimer();
            },
			RestartLockTimer: function () {
			    if (SEL.CustomEntityRecordLocking.Ids.LockTimer) {
			        clearTimeout(SEL.CustomEntityRecordLocking.Ids.LockTimer);
			    }
			    SEL.CustomEntityRecordLocking.Ids.LockTimer = setTimeout("SEL.CustomEntityRecordLocking.KeepRecordLocked()", SEL.CustomEntityRecordLocking.Ids.Timeout);
			},
			ShowDialog: function() {
			    var modal = $('#divElementLockingDialog');
			    modal.dialog({
                    modal: true,			        
			        closeOnEscape: true,
			        draggable: false,
			        resizable: false,
                    width:450,
                    buttons: [
                        {
                            text: "close",
                            "class": "jQueryUIButton",
                            click: function() {
                                 $(this).dialog("close");
                            }
                        }],
			        open: function () {
                        //replace button classes
			            $(".ui-button").unbind();
			            $(".ui-button").click(function () {
			                $('#divElementLockingDialog').dialog('destroy');
			            });
			            $(".ui-button").keyup(function (event) {
			                if (event.keyCode == 13) {
			                    $('#divElementLockingDialog').dialog('destroy');
			                }
			            });
			            $('.ui-dialog-buttonpane.ui-widget-content.ui-helper-clearfix').removeClass('ui-dialog-buttonpane ui-widget-content ui-helper-clearfix').addClass('formbuttons').addClass('formpanel');
			            $('.ui-dialog-buttonset').removeClass('ui-dialog-buttonset').addClass('buttonContainer');
			            $('.ui-button.ui-widget.ui-corner-all.ui-button-text-only').removeClass('ui-button ui-widget ui-corner-all ui-button-text-only').addClass('buttonInner');

                        $('.ui-button-text').removeClass('ui-button-text');
			            $('.ui-state-default').removeClass('ui-state-default');
			            $('.ui-state-hover').removeClass('ui-state-hover');
			            $('.ui-state-focus').removeClass('ui-state-focus');
                    }
			    });
                modal.dialog("option", "dialogClass", "modalpanel");
                modal.dialog("option", "dialogClass", "formpanel");

            
			    $(".ui-dialog-titlebar").hide();

			    modal.dialog('open');

			    $('#divElementLocking').css('display', '');
			    $('.sm_panel :input').prop('disabled', true);
			    $('a:contains(\"Edit Text\")').css('display', 'none');
			    $('.inputicon>img').css('display', 'none');
			    $('img[id$="_SelectinatorTextSearchIcon"], .dateCalImg').remove();
			    SEL.CustomEntityRecordLocking.Ids.UnlockTimer = setTimeout("SEL.CustomEntityRecordLocking.IsRecordLocked()", SEL.CustomEntityRecordLocking.Ids.Timeout);
			    
			},
			HideDialog: function() {
				$('#divElementLockingDialog').dialog('destroy');
			},
			IsRecordLocked: function () {
			    var params = new SEL.CustomEntityRecordLocking.Misc.WebServiceParameters.RecordLocked();
                SEL.Ajax.Service('/shared/webServices/svcCustomEntityRecordLocking.asmx/', 'IsRecordLocked', params, SEL.CustomEntityRecordLocking.IsElementLockedComplete);
			},
			KeepRecordLocked: function () {
                var params = new SEL.CustomEntityRecordLocking.Misc.WebServiceParameters.RecordLocked();
                SEL.Ajax.Service('/shared/webServices/svcCustomEntityRecordLocking.asmx/', 'KeepRecordLocked', params, SEL.CustomEntityRecordLocking.KeepElementLockedComplete);
            },
            IsElementLockedComplete: function (data) {
                if (!data.d) {
                    location.reload(true);
                } else {
                    if (SEL.CustomEntityRecordLocking.Ids.UnlockTimer) {
                        clearTimeout(SEL.CustomEntityRecordLocking.Ids.UnlockTimer);
                    }
                    SEL.CustomEntityRecordLocking.Ids.UnlockTimer = setTimeout("SEL.CustomEntityRecordLocking.IsRecordLocked()", SEL.CustomEntityRecordLocking.Ids.Timeout);
                }
            },
            KeepElementLockedComplete: function () {
                SEL.CustomEntityRecordLocking.RestartLockTimer();
            },
            UnlockElement: function() {
                var params = new SEL.CustomEntityRecordLocking.Misc.WebServiceParameters.RecordLocked();
                SEL.Ajax.Service('/shared/webServices/svcCustomEntityRecordLocking.asmx/', 'UnlockRecord', params, null, null, false);
            },
            Misc:
            {
                WebServiceParameters:
                {
                    RecordLocked: function() {
                        this.customEntityId = SEL.CustomEntityRecordLocking.Ids.ElementId;
                        this.id = SEL.CustomEntityRecordLocking.Ids.Id;
                    }
                }
            }
			
        }
	}

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, $g, $f, $e, $ddlValue));

