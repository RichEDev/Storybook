function detectIE() {
    var ua = window.navigator.userAgent;

    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    var edge = ua.indexOf('Edge/');
    if (edge > 0) {
        // IE 12 => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // other browser
    return false;
}

$(document).ready(function () {

    $(window).resize(function () {
        clearTimeout(window.resizedFinished);
            window.resizedFinished = setTimeout(function () {
                var resizeWidth = $(window).width();
                height = $(window).height();
                if (((resizeWidth <= 1084) && (height <= 768)) || ((height <= 1024) && (resizeWidth <= 768))) {
                    loadCSS("mediaQueryStylesheet", "/static/styles/expense/css/mediaQuery.css");
                } else {
                   if ($("link[href='/static/styles/expense/css/mediaQuery.css']").length)
                    document.getElementById('mediaQueryStylesheet').disabled = true;
                }

                if ((resizeWidth < 968)) {
                    loadCSS("mediaQueryMobileStylesheet", "/static/styles/expense/css/mobileMediaQuery.css");
                }
                else {
                    if ($("link[href='/static/styles/expense/css/mobileMediaQuery.css']").length)
                    document.getElementById('mediaQueryMobileStylesheet').disabled = true;
                }
            }, 200);
    });

    //add custom styles referring to Framwork
    if (CurrentUserInfo.Module.ID == 3) {
        $('html').addClass('frameworkApp');
    }

    var width = $(window).width(), height = $(window).height();

    if ((width <= 1024) && (height <= 768)) {
        if (window.location.href.indexOf("ContractSummary.aspx") > 0) {
            $('#maindiv').css("min-width", "756px");
            $('body').css("display", "inline-block");
        }
    };

    var isie = detectIE();
    if (isie && (isie == 8 || isie == 7)) {
        $('.r_nav').addClass("activeie8");
    }

    $(".nav").click(function () {

        $("#wrapper").toggleClass("toggled");
        if ((width <= 1024) && (height <= 768)) {
            if (window.location.href.indexOf("aenotificationtemplate.aspx") == -1 && window.location.href.indexOf("colours.aspx") == -1 && window.location.href.indexOf("claimViewer.aspx") == -1 && window.location.href.indexOf("aeexpense.aspx") == -1 && window.location.href.indexOf("ContractSummary.aspx") == -1) {
                if ($('#wrapper').hasClass('toggled')) {
                    $('.formpanel .onecolumnlarge .inputs').css('width', '');
                    $('.formpanel .twocolumn label').css('width', '');
                    $('.formpanel .onecolumn label').css('width', '');
                    $('.formpanel .onecolumn label').css('height', '');
                    $('.formpanel .onecolumn .inputs').css('width', '');
                    $('.formpanel .onecolumnsmall label').css('width', '');
                    $('.formpanel .onecolumnsmall .inputs').css('width', '');
                    $('.formpanel .onecolumnsmall input, .formpanel .onecolumnsmall .inputs select').css('width', '');
                    $('.formpanel .onecolumnsmall .fillspan').css('width', '');
                    $('.frameworkApp .formpanel .onecolumn .fillspan').css('width', '');
                } else {
                    $('.formpanel .onecolumnlarge .inputs').css('width', '40%');
                    $('.formpanel .twocolumn label').css('width', '40%');
                    $('.formpanel .onecolumn label').css('width', '40%');
                    $('.formpanel .onecolumn label').css('height', '40px');
                    $('.formpanel .onecolumn .inputs').css('width', '40%');
                    $('.formpanel .onecolumnsmall label').css('width', '40%');
                    $('.formpanel .onecolumnsmall .inputs').css('width', '40%');
                    $('.formpanel .onecolumnsmall input, .formpanel .onecolumnsmall .inputs select').css('width', '53%');
                    $('.formpanel .onecolumnsmall .fillspan').css('width', '100%');
                    $('.frameworkApp .formpanel .onecolumn .fillspan').css('width', '100%');                  
                }
            }
        }

        if ($('#wrapper').hasClass('toggled')) {
            initializeTooltip();
            $('tooltipCustomClass').removeClass('hideTooltip');

            if (width > 1366) {
                $('.homeSearchPanel').animate({ marginLeft: '70px', width: '104%' });
            }
            else if (width <= 1024) {
                $('.homeSearchPanel').animate({ marginLeft: '70px', width: '95%' });
            }
            else {
                $('.homeSearchPanel').animate({ marginLeft: '70px', width: '100%' });
            }

            $('.div_updatedon').css('margin-left', '');

        } else {
            $('.div_updatedon').css('margin-left', '0');
            $('tooltipCustomClass').addClass('hideTooltip');
            if (width > 1366) {
                $('.homeSearchPanel').animate({ marginLeft: '146px', width: '92%' });
            }
            else if (width <= 1024) {
                $('.homeSearchPanel').animate({ marginLeft: '135px', width: '80%' });
            }
            else {
                $('.homeSearchPanel').animate({ marginLeft: '146px', width: '87%' });
            }
            $('.quickTooltip li img').each(function () {
                $(this).qtip('destroy');
            });
        }
        $('.r_nav').toggleClass("active");
        if (isie && isie == 9) {
            $('.r_nav').toggleClass("activeie");
        }
        if (isie && (isie == 8 || isie == 7)) {
            $('.r_nav').toggleClass("activeie8 activeie");
        }
    });

    if (((width <= 1084) && (height <= 768)) || ((height <= 1084) && (width <= 768))) {
        if (isie && isie == 10) {
            $('.main-footer img').css('margin-bottom', '15px');
        }
        loadCSS("mediaQueryStylesheet", "/static/styles/expense/css/mediaQuery.css");
    }

    $(".submenuitem").click(function () {
        if (!$('#wrapper').hasClass('toggled')) {
            $(".nav").trigger('click');
        };
    });

    initializeTooltip();
    function initializeTooltip() {
        // Grab all elements with the class "quickTooltip"
        $('.quickTooltip li img').each(function () {
            $(this).qtip({
                content: {
                    text: function () {
                        return $(this).next('p').text();
                    } // Use the "P" element for the content

                }, style: {
                    def: false, // Remove the default styling 
                    classes: 'tooltipCustomClass'
                }, position: {
                    my: 'left center',
                    at: 'right center'
                }
            });

        });

    }

    //Dynamically adjust the contrast of the breadcrumb content, 
    //but only if there's more than one breadcrumb.
    $(".breadcrumb li:last-child > a label").unwrap();
    var breadcrumbs = $('.breadcrumb li');
    if (breadcrumbs.length > 1) {
        breadcrumbs.last().attr('style', 'opacity: 0.7')
                          .children('.breadcrumb_arrow')
                          .attr('style', 'opacity: 1!important');
    }

    var doc = document.documentElement;
    doc.setAttribute('data-useragent', navigator.userAgent);
});

loadCSS = function (id, href) {

    var $ele = $("#" + id);

    if (!$ele[0]) { // check first to see if the element already exists
        var cssLink = $("<link>");
        $("head").append(cssLink); //IE fix: append before setting href
        cssLink.attr({
            rel: "stylesheet",
            type: "text/css",
            href: href,
            id: id
        });
    } else {
        $("#" + id).removeAttr('disabled');
    }
};