(function (moduleNameHtml) {
    var scriptName = "knowledge";

    function execute() {
        SEL.registerNamespace("SEL.Knowledge");
        SEL.Knowledge =
        {
            Identifiers:
            {
                CustomArticle: null
            },

            Dom:
            {
                Summary:
                {
                    Grid: null
                },

                CustomArticle:
                {
                    Modal: null,
                    Panel: null,
                    FormPanel: null,
                    PanelTitle: null,
                    HtmlEditor: null,

                    Identifier: null,
                    ProductCategory: null,
                    ProductCategoryValue: null,
                    Title: null,
                    Summary: null,
                    Body: null
                },

                ViewArticle:
                {
                    Panel: null,
                    Modal: null
                }
            },

            Page:
            {
                RefreshGrid: function () {
                    SEL.Grid.refreshGrid('gridArticles', SEL.Grid.getCurrentPageNum('gridArticles'));
                }
            },

            OpenSalesForceUrl: function(urlSuffix) {

                var url = "http://knowledge.selenity.com/" + urlSuffix;

                if (!$("#knowledgeForm").length) {
                    var form = $("<form>").attr({
                        id: "knowledgeForm",
                        method: "POST",
                        target: "_blank"
                    }).append($("<input>").attr({
                        type: "hidden",
                        name: "kb_auth",
                        value: "kb_auth"
                    }));
                            
                    $("body").append(form);
                }

                // set the action of the form to the URL of the article, then submit it
                $("#knowledgeForm").attr("action", url).submit();
                    
            },

            CustomArticle:
            {
                New: function () {
                    var ns = SEL.Knowledge;

                    ns.Identifiers.CustomArticle = 0;
                    ns.CustomArticle.Modal.Clear.All();
                    ns.CustomArticle.Modal.Show();
                },

                Edit: function (articleIdentifier) {
                    // validation
                    if (articleIdentifier <= 0) {
                        return false;
                    }

                    // variable declaration and assignment
                    var article = null,
                        ns = SEL.Knowledge,
                        identifiers = ns.Identifiers,
                        doms = ns.Dom.CustomArticle;

                    identifiers.CustomArticle = articleIdentifier;

                    // modal setup
                    ns.CustomArticle.Modal.Clear.All();

                    // attempt to retrieve article
                    SEL.Data.Ajax({
                        data: {
                             identifier: articleIdentifier
                        },
                        methodName: "GetCustomArticle",
                        serviceName: "svcKnowledge",
                        success: function(data) {
                            if ("d" in data) {
                                if (data.d === -999) {
                                    SEL.MasterPopup.ShowMasterPopup("You do not have permission to edit this article.", "Message from " + moduleNameHtml);
                                    return;
                                }

                                article = data.d;

                                if (typeof article !== "undefined" && article !== null) {

                                    $("#" + doms.PanelTitle).html("Knowledge Article: " + article.Title);

                                    $("#" + doms.Title).val(article.Title);
                                    $("#" + doms.ProductCategory).val(article.ProductCategoryValue);
                                    $("#" + doms.Summary).val(article.Summary);
                                    $("#" + doms.Body).val(article.Body);

                                    var editor = $find(SEL.Knowledge.Dom.CustomArticle.HtmlEditor);
                                    editor._editableDiv.innerHTML = article.Body;
                                    editor._editableDiv_onblur();

                                    ns.CustomArticle.Modal.Show();

                                    $("#" + doms.Title).focus();
                                }
                            }
                        }
                    });
                },

                Delete: function (articleIdentifier) {
                    if (confirm("Are you sure you want to delete this article?")) {
                        SEL.Data.Ajax({
                            data: {
                                identifier: articleIdentifier
                            },
                            methodName: "DeleteCustomArticle",
                            serviceName: "svcKnowledge",
                            success: function (data) {
                                if ("d" in data) {
                                    if (data.d === -999) {
                                        SEL.MasterPopup.ShowMasterPopup("You do not have permission to delete this article.", "Message from " + moduleNameHtml);
                                        return;
                                    }

                                    if (data.d !== 0) {
                                        SEL.MasterPopup.ShowMasterPopup("The article could not be deleted.", "Message from " + moduleNameHtml);
                                        return; 
                                    }

                                    SEL.Knowledge.Page.RefreshGrid();
                                }
                            }
                        });
                    }
                },

                TogglePublished: function (articleIdentifier) {
                    SEL.Data.Ajax({
                        data: {
                            identifier: articleIdentifier
                        },
                        methodName: "ToggleCustomArticlePublished",
                        serviceName: "svcKnowledge",
                        success: function (data) {
                            if (data.d === -999) {
                                SEL.MasterPopup.ShowMasterPopup("You do not have permission to edit this article.", "Message from " + moduleNameHtml);
                                return;
                            }

                            if (data.d < 0) {
                                SEL.MasterPopup.ShowMasterPopup("This article cannot be published.", "Message from " + moduleNameHtml);
                                return;
                            }

                            SEL.Knowledge.Page.RefreshGrid();
                        }
                    });
                },

                Save: function () {
                    if (validateform("vgArticle") === false) {
                        return false;
                    }

                    var ns = SEL.Knowledge,
                        identifiers = ns.Identifiers,
                        doms = ns.Dom.CustomArticle,

                        title = $("#" + doms.Title).val(),
                        productCategory = $("#" + doms.ProductCategory).val(),
                        summary = $("#" + doms.Summary).val(),
                        body = $("#" + doms.Body).val();

                    SEL.Data.Ajax({
                        data: {
                            identifier: identifiers.CustomArticle,
                            title: title,
                            productCategory: productCategory,
                            summary: summary,
                            body: body
                        },
                        methodName: "SaveCustomArticle",
                        serviceName: "svcKnowledge",
                        success: function (data) {
                            if ("d" in data) {
                                if (data.d === -999) {
                                    SEL.MasterPopup.ShowMasterPopup("You do not have permission to " + (identifiers.KnowledgeCustomArticleId === 0 ? "add" : "edit") + " this article.", "Message from " + moduleNameHtml);
                                    return false;
                                }

                                if (data.d === -1) {
                                    SEL.MasterPopup.ShowMasterPopup("An article with this Title / Question already exists.", "Message from " + moduleNameHtml);
                                    return false;
                                }

                                ns.Page.RefreshGrid();
                                ns.CustomArticle.Modal.Hide();
                            }
                        }
                    });
                },

                Cancel: function () {
                    SEL.Knowledge.CustomArticle.Modal.Hide();
                },

                Modal:
                {
                    Clear:
                    {
                        All: function () {
                            var ns = SEL.Knowledge,
                                modal = ns.CustomArticle.Modal;

                            modal.Clear.Title();
                            modal.Clear.CustomArticle();
                        },

                        Title: function () {
                            $("#" + SEL.Knowledge.Dom.CustomArticle.PanelTitle).html("New Knowledge Article");
                        },

                        CustomArticle: function () {
                            var doms = SEL.Knowledge.Dom.CustomArticle;

                            $("#" + doms.ProductCategory).val("");
                            $("#" + doms.Title).val("");
                            $("#" + doms.Summary).val("");
                            $("#" + doms.Body).val("");

                            var editor = $find(SEL.Knowledge.Dom.CustomArticle.HtmlEditor);
                            editor._editableDiv.innerHTML = "";
                            editor._editableDiv_onblur();
                       }
                    },

                    Show: function () {
                        SEL.Common.ShowModal(SEL.Knowledge.Dom.CustomArticle.Modal);

                        $("#" + SEL.Knowledge.Dom.CustomArticle.ProductCategory).focus();
                    },

                    Hide: function () {
                        SEL.Common.HideModal(SEL.Knowledge.Dom.CustomArticle.Modal);
                    }

                },

                Setup: function ()
                {
                    // setup key bindings
                    window.Sys.Application.add_load(function () {

                        // Base Save
                        SEL.Common.BindEnterKeyForSelector("#" + SEL.Knowledge.Dom.CustomArticle.FormPanel + ".formpanel>div:not(:has(textarea))", SEL.Knowledge.CustomArticle.Save);

                        $(document).keydown(function (e) {
                            if (e.keyCode === $.ui.keyCode.ESCAPE) {
                                e.preventDefault();

                                if ($("#" + SEL.Knowledge.Dom.CustomArticle.Panel).filter(":visible").length > 0) {
                                    SEL.Knowledge.CustomArticle.Cancel();
                                    return;
                                }
                            }
                        });

                    });
                }
            },

            ViewArticle:
            {
                Close: function () {
                    // removing the hash completely will cause the page to jump to the top
                    window.location.hash = "_";

                    SEL.Knowledge.ViewArticle.Modal.Hide();
                },

                Open: function (listItem) {
                    // a custom article
                    if (listItem.find(".KnowledgeCustomArticle").length) {

                        var articleId = listItem.data("identifier");
                        SEL.Knowledge.ViewArticle.OpenCustomArticle(articleId);
                    }

                    // a salesforce article
                    else if (listItem.find(".KnowledgeSalesForceArticle").length) {

                        SEL.Knowledge.OpenSalesForceUrl(listItem.find("a").attr("href"));
                    }
                },

                OpenCustomArticle: function(articleId) {
                    window.location.hash = "article-" + articleId;

                    SEL.Data.Ajax({
                        serviceName: "svcKnowledge",
                        methodName: "GetCustomArticle",
                        data: {
                            identifier: articleId
                        },
                        success: function (data) {
                            if ("d" in data) {
                                $("#" + SEL.Knowledge.Dom.ViewArticle.Panel + " .articleTitle").html(data.d.Title);
                                $("#" + SEL.Knowledge.Dom.ViewArticle.Panel + " .articleBody").html(data.d.Body);
                                $("#" + SEL.Knowledge.Dom.ViewArticle.Panel + " .email").attr("href", "mailto:?subject=Article: " + data.d.Title + "&body=Article URL: " + window.location.href);
                                $("#" + SEL.Knowledge.Dom.ViewArticle.Panel + " .popup").attr("href", window.location.href);

                                SEL.Knowledge.ViewArticle.Modal.Show();
                            }
                        }
                    });                
                },

                Modal:
                {
                    Show: function() {
                        SEL.Common.ShowModal(SEL.Knowledge.Dom.ViewArticle.Modal);
                    },

                    Hide: function() {
                        SEL.Common.HideModal(SEL.Knowledge.Dom.ViewArticle.Modal);
                    }
                },

                Setup: function () {

                    // move the modal outside of the rest of the page, to the end of <body> so that it can be easily used for print CSS 
                    $("#" + SEL.Knowledge.Dom.ViewArticle.Panel).appendTo($("body"));

                    $(document).keydown(function (e) {

                        if (e.keyCode === $.ui.keyCode.ESCAPE) {

                            e.preventDefault();

                            if ($("#" + SEL.Knowledge.Dom.ViewArticle.Panel).filter(":visible").length > 0) {

                                SEL.Knowledge.ViewArticle.Close();
                                return;
                            }
                        }
                    });

                    // client side validation check on the search form
                    var performValidation = function(event) {
                        if (!validateform()) {
                            event.preventDefault();
                        }
                    };

                    $(".searchForm input[type=submit]").on("click", function (event) {
                        performValidation(event);
                    });

                    $(".searchForm input[type=text]").on("keydown", function (event) {
                        if (event.keyCode === $.ui.keyCode.ENTER) {
                            performValidation(event);
                        }
                    });

                    // clicks on search results
                    $(".searchResults li").on("click", function (event) {

                        event.preventDefault();
                        SEL.Knowledge.ViewArticle.Open($(this));
                    });

                    // clicks on question/statements which lead to a Knowledge url
                    $(".helpAndSupportQuestions a.knowledgeUrl").on("click", function (event) {

                        event.preventDefault();

                        // hide any existing messages
                        $(".ticketRedirectMessage").hide();
                        var message = $(".knowledgeRedirectMessage");
                        message.hide();

                        var url = $(this).attr("href");
                        var listItem = $(this).parent();

                        message.children("a").attr("href", url).off("click").on("click", function(e) {

                            e.preventDefault();
                            SEL.Knowledge.OpenSalesForceUrl(url);
                        });

                        message.appendTo(listItem).slideDown();
                    });

                    // clicks on question/statements which lead to a support ticket
                    $(".helpAndSupportQuestions a.internalTicket, .helpAndSupportQuestions a.selTicket").on("click", function (event) {

                        event.preventDefault();

                        // hide any existing messages
                        $(".knowledgeRedirectMessage").hide();
                        var message = $(".ticketRedirectMessage");
                        message.hide();

                        var url = $(this).attr("href");
                        var listItem = $(this).parent();

                        message.children("a").attr("href", url.replace("None+of+the+above", ""));
                        message.appendTo(listItem).slideDown();
                    });

                    // expand/collapse behaviour for the list of questions/statements
                    $("div.helpAndSupportQuestions > ul > li").on("click", function (event) {

                        var listItem = $(this);

                        if (!listItem.hasClass("active")) {

                            listItem.siblings().removeClass("active").children("ul").hide();
                            listItem.addClass("active").children("ul").slideDown();

                        } else if (!listItem.children("ul").has(event.target).length) {

                            listItem.removeClass("active").children("ul").slideUp();

                        }

                    });

                    // place some watermark text in the search field, using the "watermark" jquery plugin
                    $('input.searchQuery').watermark("Search Help & Support or choose from the list below");

                    // if the location hash is #article-[articleId] then open that article automatically
                    if (window.location.hash.indexOf("#article-") === 0) {
                        var articleId = +window.location.hash.replace("#article-", "");

                        SEL.Knowledge.ViewArticle.OpenCustomArticle(articleId);
                    }
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
}(moduleNameHTML));