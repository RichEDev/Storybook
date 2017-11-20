var rteContext;

function setButtonHidden(editor, buttonName, toolbarNumber, state) {
    // get collection of buttons in the toolbar  
    var buttons = editor.get_editPanel().get_toolbars()[toolbarNumber].get_buttons();
    // looking for the 'buttonName' button  
    for (var i = 0; i < buttons.length; i++) {
        var button = buttons[i];
        if (button.get_buttonName() == buttonName) {
            // button's node  
            var element = button.get_element();
            if (state) {
                // restore visibility  
                element.style.position = "";
                element.style.top = "";
                element.style.left = "";
            } else {
                // hide button  
                element.style.position = "absolute";
                element.style.top = "-100px";
                element.style.left = "-100px";
            }
        }
    }
}

function setBottomToolbarHidden(editor, state) {
    var toolbar = editor.get_editPanel().get_toolbars()[0]; // 0 - bottom toolbar  
    var element = toolbar.get_element();
    if (state) {
        // restore visibility  
        element.style.position = "";
        element.style.top = "";
        element.style.left = "";
    } else {
        // hide toolbar  
        element.style.position = "absolute";
        element.style.top = "-100px";
        element.style.left = "-100px";
    }
}

function setTopToolbarHidden(editor, state) {
    var toolbar = editor.get_editPanel().get_toolbars()[0]; // 0 - bottom toolbar  
    var element = toolbar.get_element();
    if (state) {
        // restore visibility  
        element.style.position = "";
        element.style.top = "";
        element.style.left = "";
    } else {
        // hide toolbar  
        element.style.position = "absolute";
        element.style.top = "-100px";
        element.style.left = "-100px";
    }
}

function setButtonState_HTMLtext(editor, state) {
    // "HtmlMode" - name of the "HTML text" button   
    // (class: AjaxControlToolkit.HTMLEditor.ToolbarButton.HtmlMode)
    setButtonHidden(editor, "HtmlMode", 0, state); // 0 - bottom toolbar, 1 - top toolbar  
}


function initCustomEntityEditor(editorID) {
    setButtonState_HTMLtext($find(editorID), true);
}

function disableBottomToolbar() {
    var editPanel = $find(rteEditor);
    if (editPanel != null) {
        var ePanel = editPanel.get_editPanel();
        var bottomToolbar = ePanel.get_toolbars()[0];
        var bottomToolbarElement = bottomToolbar.get_element();

        //bottomToolbarElement.parentNode.style.display = "none";
    }
}

function setPasteShortcut() {
    if (rteEditor == null || rteEditor == '')
        return;

    disableDesignPanelContextMenu();

    $find(rteEditor).get_editPanel().get_modePanels()[0].captureInDesign = function (ev) {
        var designPanel = this;
        //this.set_noPaste(true); // this forces paste as plain text

        // Local key event signature
        var isKey = ((Sys.Extended.UI.HTMLEditor.isIE || Sys.Extended.UI.HTMLEditor.isSafari) && ev.type == "keydown") || (ev.type == "keypress");
        var paste = false;
        //        if (isKey) {
        //            // Shift-Ins
        //            if (ev.shiftKey && ev.keyCode == 45) {
        //                paste = true;
        //            } else {
        //                // If Ctrl+key
        //                if (ev.ctrlKey && !ev.altKey) {
        //                    // what a key
        //                    var code = (Sys.Extended.UI.HTMLEditor.isIE || window.opera || Sys.Extended.UI.HTMLEditor.isSafari) ? ev.keyCode : ev.charCode;
        //                    var key = String.fromCharCode(code).toLowerCase();
        //                    // Ctrl-V 
        //                    if (key == 'v') {
        //                        paste = true;
        //                    }
        //                }
        //            }
        //        }
        //        else {
        //            var isRightMouseClick = ((AjaxControlToolkit.HTMLEditor.isIE || AjaxControlToolkit.HTMLEditor.isSafari) && (ev.which && ev.which == 3) || (ev.button && ev.button == 2));
        //            if (isRightMouseClick) {
        //                var behavior = $find('_cmExt_Behavior');
        //                if (behavior != null) {
        //                    behavior._onMouseDown(ev);
        //                    behavior._onDocumentContextMenu(ev);
        //                    return false;
        //                }
        //            }

        //            var isLeftMouseClick = ((AjaxControlToolkit.HTMLEditor.isIE || AjaxControlToolkit.HTMLEditor.isSafari) && (ev.which && (ev.which == 1 || ev.which == 2)) || (ev.button && (ev.button == 0 || ev.button == 1)));
        //            if (isLeftMouseClick) {
        //                var behavior = $find('_cmExt_Behavior');
        //                if (behavior != null) {
        //                    behavior._hideMenu();
        //                }
        //            }
        //        }

        if (paste) {
            if (Sys.Extended.UI.HTMLEditor.isIE) {
                // Do 'paste with clean up'
                designPanel._paste(true, true);

                // Break the default event processing
                return false;
            } else {
                // Switch on 'pasting from MS Word'
                designPanel.isWord = true;
            }
            // Continue the default event processing
            return true;
        }
    };
}

function tidyHTML(html) {
    if (html != null) {
        if (html != "") {
            html = CleanWordHTML(html);
            html = html.toString().replace(/FONT-FAMILY/g, "font-family").replace(/FONT-SIZE/g, "font-size").replace(/P>/g, "p>").replace(/<P/g, "<p").replace(/FONT-WEIGHT/g, "font-weight").replace(/<FONT/g, "<font").replace(/FONT>/g, "font>").replace(/FONT-STYLE>/g, "font-style>");
            html = html.replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&quot;/g, "\"").replace(/&amp;/g, "&");
            html = html.toString().replace(/SPAN>/g, "span>").replace(/<SPAN/g, "<span").replace(/TEXT-DECORATION/g, "text-decoration").replace(/BACKGROUND-COLOR/g, "background-color").replace(/COLOR/g, "color").replace(/BACKGROUND/g, "background");
            html = html.toString().replace(/UL>/g, "ul>").replace(/OL>/g, "ol>").replace(/UL>/g, "ul>").replace(/LI>/g, "li>");
            html = html.toString().replace(/MARGIN:/g, "margin:").replace(/B>/g, "b>").replace(/<B/g, "<b").replace(/U>/g, "u>").replace(/<U/g, "<u").replace(/I>/g, "i>").replace(/<i/g, "<i");
            html = html.toString().replace(/\r/g, "").replace(/\n/g, "<br/>").replace(/\t/g, "&nbsp;"); // .replace(/</g, "&lt;").replace(/>/g, "&gt;")
            html = html.toString().replace(/BORDER-BOTTOM/g, "border-bottom").replace(/BORDER-LEFT/g, "border-left").replace(/BORDER-RIGHT/g, "border-right").replace(/BORDER-TOP/g, "border-top");
            html = html.toString().replace(/<TABLE/g, "<table").replace(/TABLE>/g, "table>").replace(/<TD/g, "<td").replace(/TD>/g, "td>").replace(/<TR/g, "<tr").replace(/TR>/g, "tr>");
            html = html.toString().replace(/TBODY/g, "tbody").replace(/THEAD/g, "thead").replace(/WIDTH:/g, "width:");
            html = html.toString().replace(/vAlign/gi, "valign").replace(/Align/gi, "align").replace(/CELLSPACING/gi, "cellspacing").replace(/CELLPADDING/gi, "cellpadding");
            html = html.toString().replace(/lang=EN-Gb/g, "").replace(/<A\s/gi, '<a ').replace(/A>/gi, "a>");
            html = html.toString().replace(/cellpadding=([0-9*])/gi, "cellpadding=\"$1\"");
            html = html.toString().replace(/cellspacing=([0-9*])/gi, "cellspacing=\"$1\"");
            html = html.toString().replace(/align=(left|right|center|char|justify)/gi, 'align="$1"');
            html = html.toString().replace(/valign=(top|bottom|middle|baseline)/gi, 'valign="$1\"');
            html = html.toString().replace(/\s&\s/gi, '&nbsp;&amp;&nbsp;');
            html = ReplaceRgb(html);
        }
    }

    return html;
}

function CleanWordHTML(str, removeTags) {
    str = str.replace(/\?=""/g, "");
    str = str.replace(/<o:p>\s*<\/o:p>/g, "");
    str = str.replace(/<o:p>.*?<\/o:p>/g, "&nbsp;");
    str = str.replace(/\s*mso-[^:]+:[^;">]+;?/gi, "");
    //    str = str.replace(/\s*MARGIN: 0cm 0cm 0pt\s*;/gi, "");
    //    str = str.replace(/\s*MARGIN: 0cm 0cm 0pt\s*"/gi, "\"");
    str = str.replace(/\s*TEXT-INDENT: 0cm\s*;/gi, "");
    str = str.replace(/\s*TEXT-INDENT: 0cm\s*"/gi, "\"");
    //str = str.replace(/\s*TEXT-ALIGN: [^\s;]+;?"/gi, "\"");
    str = str.replace(/\s*PAGE-BREAK-BEFORE: [^\s;]+;?"/gi, "\"");
    //str = str.replace(/\s*FONT-VARIANT: [^\s;]+;?"/gi, "\"");
    str = str.replace(/\s*tab-stops:[^;"]*;?/gi, "");
    str = str.replace(/\s*tab-stops:[^"]*/gi, "");
    //str = str.replace(/\s*face="[^"]*"/gi, "");
    //str = str.replace(/\s*face=[^ >]*/gi, "");
    //str = str.replace(/\s*FONT-FAMILY:[^;"]*;?/gi, "");
    str = str.replace(/<namespace(.*?)\/>/gi, "");
    str = str.replace(/<meta(.*?)\/>/gi, "");
    str = str.replace(/(<!--(.*?)\-->)/gi, "");
    str = str.replace(/(&lt;!--(.*?)--&gt;)/gi, "");
    str = str.replace(/(<link(.*?)\/>)/gi, "");
    str = str.replace(/(<object(.*?)\/>)/gi, "");
    str = str.replace(/(<xml(.*?)<\/xml>)/gi, "");
    str = str.replace(/<(\w[^>]*) class=([^ |>]*)([^>]*)/gi, "<$1$3");
    str = str.replace(/<(\w[^>]*) name=([^ |>]*)([^>]*)/gi, "<$1$3");
    //str = str.replace(/<(\w[^>]*) style="([^\"]*)"([^>]*)/gi, "<$1$3");
    str = str.replace(/(font:([^;>"]*))/gi, '');
    str = str.replace(/\s*style="\s*"/gi, '');
    str = str.replace(/<SPAN\s*[^>]*>\s*&nbsp;\s*<\/SPAN>/gi, '&nbsp;');
    str = str.replace(/<SPAN\s*[^>]*><\/SPAN>/gi, '');
    str = str.replace(/<(\w[^>]*) lang=([^ |>]*)([^>]*)/gi, "<$1$3");
    //str = str.replace(/<SPAN\s*>(.*?)<\/SPAN>/gi, '$1');
    //str = str.replace(/<FONT\s*>(.*?)<\/FONT>/gi, '$1');
    str = str.replace(/<\\?\?xml[^>]*>/gi, "");
    str = str.replace(/<\/?\w+:[^>]*>/gi, "");
    str = str.replace(/<H\d>\s*<\/H\d>/gi, '');
    //    str = str.replace(/<H1([^>]*)>/gi, '');
    //    str = str.replace(/<H2([^>]*)>/gi, '');
    //    str = str.replace(/<H3([^>]*)>/gi, '');
    //    str = str.replace(/<H4([^>]*)>/gi, '');
    //    str = str.replace(/<H5([^>]*)>/gi, '');
    //    str = str.replace(/<H6([^>]*)>/gi, '');
    //    str = str.replace(/<\/H\d>/gi, '<br>'); //remove this to take out breaks where Heading tags were
    str = str.replace(/<(U|I|STRIKE)>&nbsp;<\/\1>/g, '&nbsp;');
    str = str.replace(/<(B|b)>&nbsp;<(\/b|\/B)>/g, '');
    str = str.replace(/((line-height:\s)(normal|inherit))/gi, '$2 1');
    str = str.replace(/(line-height:\s(\d{1,3}(%));)/gi, '');

    // background style does not export to DocIO but background-color does
    str = str.replace(/(([="|\s*\;\s*])background:)/gi, '$2background-color:');

    // Syncfusion does not like <thead> and <tbody> tags in tables!
    str = str.replace(/(<thead>|<\/thead>|<tbody>|<\/tbody>)/gi, '');

    // remove <a> tags that don't have a href="" path
    str = str.replace(/<a([^>]*)>([^<]*)<\/a>/gi,
        function (match, p1, p2) {
            return p1.match(/href/) ? match : p2;
        });

    // margin: style is ignored by DocIO. Modify to honour the left margin for style purposes.
    // replace margin: 0cm 0cm 12pt with margin-left: 0cm
    str = str.replace(/(margin:)\s*([0-9.]+?)(pt|px|cm)\s+([0-9.]+?)(pt|px|cm)\s+([0-9.]+?)(pt|px|cm)(;|")/gi, 'margin-top: $2$3; margin-right: $4$5; margin-bottom: $6$7; margin-left: 0cm$8');
    // replace margin: 0cm 0cm 12pt 42.55pt with margin-left: 42.55pt
    str = str.replace(/(margin:)\s*([0-9.]+?)(pt|px|cm)\s+([0-9.]+?)(pt|px|cm)\s+([0-9.]+?)(pt|px|cm)\s+([0-9.]+?)(pt|px|cm)(;|")/gi, 'margin-top: $2$3; margin-right: $4$5; margin-bottom: $6$7; margin-left: $8$9$10');

    // override style sheet font to just Arial
    str = str.replace(/(([="|\s*\;\s*])font-family: "arial","sans-serif";)/gi, '$2font-family: "arial;"');
    str = str.replace(/(([="|\s*\;\s*])font-family: "times new roman","serif";)/gi, '$2font-family: "arial;"');
    str = str.replace(/(([="|\s*\;\s*])font-family: symbol;)/gi, '$2font-family: arial;');
    str = str.replace(/(([="|\s*\;\s*])font-family: arial,helvetica,sans-serif;)/gi, '$2font-family: Arial;');
    str = str.replace(/(arial,helvetica,sans-serif);/gi, 'arial');

    // replace empty <td> to contain &nbsp; then remove empty tags (keeps checking for empty <td> because don't want to lose them)
    str = str.replace(/<td([a-zA-Z0-9:;\.\,\-\s#"'=]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');
    str = str.replace(/<([^\s>]+)[^>]*>\s*<\/\1>/g, '');
    str = str.replace(/<td([a-zA-Z0-9:;\.\,\-\s#"'=]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');
    str = str.replace(/<([^\s>]+)[^>]*>\s*<\/\1>/g, '');
    str = str.replace(/<td([a-zA-Z0-9:;\.\,\-\s#"'=]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');
    str = str.replace(/<([^\s>]+)[^>]*>\s*<\/\1>/g, '');
    str = str.replace(/<td([a-zA-Z0-9:;\.\,\-\s#"'=]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');
    str = str.replace(/<([^\s>]+)[^>]*>\s*<\/\1>/g, '');
    str = str.replace(/<td([a-zA-Z0-9:;\.\,\-\s#"'=]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');
    str = str.replace(/<([^\s>]+)[^>]*>\s*<\/\1>/g, '');
    str = str.replace(/<td([a-zA-Z0-9:;\.\,\-\s#"'=]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');
    str = str.replace(/<([^\s>]+)[^>]*>\s*<\/\1>/g, '');
    str = str.replace(/<td([a-zA-Z0-9:;\.\,\-\s#"'=]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');

    //some RegEx code for the picky browsers
    //    var re = new RegExp("(<P)([^>]*>.*?)(<\/P>)", "gi");
    //    str = str.replace(re, "<div$2</div>");
    //    var re2 = new RegExp("(<font|<FONT)([^*>]*>.*?)(<\/FONT>|<\/font>)", "gi");
    //    str = str.replace(re2, "<span$2</span>");
    //str = str.replace(/size|SIZE = ([\d]{1})/g, '');

    //str = str.replace(/(<[^\s]+[^=>]*style=(['"])[^\2|>]*)>/gi, "$1$2>");

    // ensure that matching quotes exist in tags 
    str = str.replace(/(<[^\s>]+[^>]+?=('))([a-zA-Z0-9:;\.\,\-\s#"]*)(?:(?=\s\w+=)|(?=>))/gi, "$1$3$2");
    str = str.replace(/(<[^\s>]+[^>]+?=('))([a-zA-Z0-9:;\.\,\-\s#']*)(?:(?=\s\w+=)|(?=>))/gi, "$1$3$2");

    str = str.replace(/("")/gi, '"');
    str = str.replace(/('')/gi, "'");

    //ensure breaks are closed correctly
    str = str.replace(/<br>/g, "<br/>".replace(/<BR>/g, "<br/>"));
    str = ReplaceRgb(str);

    str = str.replace(/<blockquote.*?>/g, "<blockquote>");

    if (removeTags) {
        str = str.replace(/<FONT[^><]*>|<.FONT[^><]*>/g, '');
        str = str.replace(/<FONT[^><]*>|<.FONT[^><]*>/g, '');
        str = str.replace(/<font face[^><]*>|<.font face[^><]*>/g, '');
        str = str.replace(/font-size\s*:.*?;?/g, "");
        str = str.replace(/font-family\s*:.*?;/g, "");
        str = str.replace(/line-height\s*:.*?;?/g, "");
        str = str.replace(/<span style=.*?>/g, "<span>");
        str = str.replace(/<font .*?>/g, "");
        str = str.replace(/<\/font>/g, "");
        str = str.replace(/font-size:.\d*px;/g, "");
        str = str.replace(/line-height:.\d*px;/g, "");
    }

    return str;
}

function ReplaceRgb(value) {
    var result = value;
    var re = /(rgb\s*\(\s*([0-9]+)\s*,\s*([0-9]+)\s*,\s*([0-9]+)\s*\))/ig;

    function hex(d) {
        return (d < 16) ? ("0" + d.toString(16)) : d.toString(16);
    };
    function repl($0, $1, $2, $3, $4) {
        var r = parseInt($2);
        var g = parseInt($3);
        var b = parseInt($4);
        return "#" + hex(r) + hex(g) + hex(b);
    }
    try { // some versions of Safari don't support such replace
        result = result.replace(re, repl);
    } catch (e) { }
    return result;
}

function getHTML() {
    var thisEditor = $find(rteEditor + '_ctl02_ctl00');
    var html = $get(rteEditor + '_ctl02_ctl00').contentWindow.document.body.innerHTML;
    var editPanel = $find(rteEditor);
    if (editPanel != null) {
        var toolbar = editPanel.get_editPanel().get_toolbars()[0]; // 0 - bottom toolbar  

        var buttons = toolbar.get_buttons();
        thisEditor._editPanel.set_activeMode(1);
        thisEditor._editPanel.set_visible(true);

        var thisEditor1 = $find(rteEditor + '_ctl02_ctl01');

        var newhtml = thisEditor._editPanel._modePanels[1]._cachedContent;
        var newhtml1 = thisEditor1._editPanel._modePanels[1]._cachedContent;
        //buttons[1]._onclick();

        //var htmlview = editPanel.get_editPanel();
    }
    return true;
}

function CallPaste() {
    if (rteEditor == null || rteEditor == '')
        return;

    var designPanel = $find(rteEditor).get_editPanel().get_modePanels()[0];

    if (designPanel != null) {
        if (AjaxControlToolkit.HTMLEditor.isIE) {
            // Do 'paste with clean up'
            //            designPanel._saveContent();
            //            designPanel.openWait();
            //            setTimeout(function() {
            //                designPanel._editPanel.set_activeMode(1);
            //                designPanel.onContentChanged();
            //                designPanel._editPanel.set_activeMode(0);
            //                designPanel.onContentChanged();
            designPanel._paste(true, true);
            //                designPanel.closeWait();
            //            }, 0)


            // Break the default event processing
            return false;
        } else {
            // Switch on 'pasting from MS Word'
            var sel = designPanel._getSelection();
            var range = designPanel._createRange(sel);
            var useVerb = String.format(Sys.Extended.UI.Resources.HTMLEditor_toolbar_button_Use_verb, (Sys.Extended.UI.HTMLEditor.isSafari && navigator.userAgent.indexOf("mac") != -1) ? "Apple-V" : "Ctrl-V");
            var mess = String.format(Sys.Extended.UI.Resources.HTMLEditor_toolbar_button_OnPasteFromMSWord, useVerb);

            alert(mess);

            setTimeout(function () {
                designPanel._removeAllRanges(sel);
                designPanel._selectRange(sel, range);
            }, 0);

            designPanel.isWord = true;
        }
    }
    return true;
}

function disableDesignPanelContextMenu() {
    var editorBody = $get(rteEditor + '_ctl02_ctl00').contentWindow.document.body;
    if (editorBody != null) {
        editorBody.oncontextmenu = function () { return false; }
    }
}