(function ()
{
    var scriptName = "Logon";
    
    function execute()
    {
        SEL.registerNamespace("SEL.Logon");
        
        SEL.Logon =
        {
            /* Dom ID's */
            /* Forgotten Detail Panel */
            ModalWindowDomID: null,
            ModalForgottenDetailsMessageDomID: null,
            ModalForgottenDetailsEmailAddressDomID: null,
            ModalForgottenDetailsSubmitSpanDomID: null,
            ModalForgottenDetailsCloseSpanDomID: null,
            FlashForgottenDetailsButton: false,
            /* Logon Controls */
            CompanyIDDomID: null,
            UsernameDomID: null,
            PasswordDomID: null,
            HostName: null,
            OriginalButtonBorderColour: null,
            OriginalButtonTextColour: null,

            /* Messages  */
            ModalMessageEmailNotUnique: "Sorry, the email address you have entered is not unique so your logon details cannot be sent there. Please call your administrator for assistance.",
            ModalMessageEmailNotFound: "Sorry, the email address you have entered does not exist.  Please call your administrator for assistance.",
            ModalMessageArchivedEmployee: "Your account is currently archived, please call your administrator to un-archive your account and reset your password.",
            ModalMessageInactiveEmployee: "Your account is currently waiting to be approved, please call your administrator to have your account activated.",
            ModalMessageEmployeeDetailsSent: "Thank you, you will shortly receive an email with instructions on how to reset your password.",
            ModalMessageEmployeeLocked: "Your account is currently locked, please call your administrator to unlock your account and reset your password.",
            /* Messages to enum */
            ForgottenDetailResponse: {
                1: SEL.Logon.ModalMessageEmailNotUnique,
                2: SEL.Logon.ModalMessageEmailNotFound,
                3: SEL.Logon.ModalMessageArchivedEmployee,
                4: SEL.Logon.ModalMessageInactiveEmployee,
                5: SEL.Logon.ModalMessageEmployeeDetailsSent,
                6: SEL.Logon.ModalMessageEmployeeLocked
            },

            HideForgottenDetails: function () {
                $("#forgottenDetailsPanel").hide();

                var button = $("#lnkForgottenDetails");
                button.finish();
                button.removeAttr("style");
                button.attr("href", "#forgottendetails");
                button.attr("title", "forgotten details");
                button.children("span").text("forgotten details");
                button.button().off("click").on("click", function ()
                {
                    SEL.Logon.ShowForgottenDetails();
                    return false;
                });
                
                $("#logonPanel").fadeIn();

                $g(SEL.Logon.ModalForgottenDetailsEmailAddressDomID).value = "";
                $g(SEL.Logon.ModalForgottenDetailsMessageDomID).innerHTML = "";
            },
            //            Show: function () {
            //                $g(SEL.Logon.ModalForgottenDetailsMessageDomID).innerHTML = "";
            //                SEL.Common.ShowModal(SEL.Logon.ModalWindowDomID);
            //                SEL.Logon.ChangeButtonDisplay(true, true);
            //            },
            PromptForgottenDetails: function () {
                var button = $("#lnkForgottenDetails");
                this.OriginalButtonBorderColour = button.css("border-top-color");
                this.OriginalButtonTextColour = button.css("color");

                var speed = 600;
                var newColor = "#F00";

                // make the button text/border glow blue
                $("#lnkForgottenDetails").animate({ borderColor: newColor, color: newColor }, speed)
                                         .animate({ borderColor: this.OriginalButtonBorderColour, color: this.OriginalButtonTextColour }, speed)
                                         .animate({ borderColor: newColor, color: newColor }, speed)
                                         .animate({ borderColor: this.OriginalButtonBorderColour, color: this.OriginalButtonTextColour }, speed)
                                         .animate({ borderColor: newColor, color: newColor }, speed)
                                         .animate({ borderColor: this.OriginalButtonBorderColour, color: this.OriginalButtonTextColour }, speed)
                                         .animate({ borderColor: newColor }, speed);
            },
            ShowForgottenDetails: function () {
                $("#logonPanel").hide();
                var button = $("#lnkForgottenDetails");
                button.finish();
                button.removeAttr("style");
                button.attr("href", "#");
                button.attr("title", "logon");
                button.children("span").text("logon");
                button.button().off("click").on("click", function ()
                {
                    SEL.Logon.HideForgottenDetails();
                    return false;
                });
                
                $("#forgottenDetailsPanel").fadeIn();
            },
            SubmitForgottenDetails: function () {
                if (SEL.Common.ValidateForm("ForgottenDetails", SEL.Logon.ModalForgottenDetailsMessageDomID) === true) {
                    Spend_Management.svcLogon.RequestForgottenDetails($g(SEL.Logon.ModalForgottenDetailsEmailAddressDomID).value, SEL.Logon.HostName, SEL.Logon.SubmitComplete, SEL.Logon.SubmitFail);
                }
            },
            SubmitComplete: function (webServiceResponse)
            {
                var responseString = "";
                switch (parseInt(webServiceResponse, 0))
                {
                    case 1: responseString = SEL.Logon.ModalMessageEmailNotUnique; break;
                    case 2: responseString = SEL.Logon.ModalMessageEmailNotFound; break;
                    case 3: responseString = SEL.Logon.ModalMessageArchivedEmployee; break;
                    case 4: responseString = SEL.Logon.ModalMessageInactiveEmployee; break;
                    case 5: responseString = SEL.Logon.ModalMessageEmployeeDetailsSent; break;
                    case 6: responseString = SEL.Logon.ModalMessageEmployeeLocked; break;
                    default: responseString = SEL.Common.MessageGenericError; break;
                }
                $g(SEL.Logon.ModalForgottenDetailsMessageDomID).innerHTML = responseString;
            },
            SubmitFail: function (webServiceResponse)
            {
                $g(SEL.Logon.ModalForgottenDetailsMessageDomID).innerHTML = SEL.Common.MessageGenericError;
            },
            SetLogonControlFocus: function ()
            {
                if ($g(SEL.Logon.CompanyIDDomID) !== null && $g(SEL.Logon.UsernameDomID) !== null && $g(SEL.Logon.PasswordDomID) !== null)
                {
                    if ($g(SEL.Logon.CompanyIDDomID).value === "")
                    {
                        $g(SEL.Logon.CompanyIDDomID).focus();
                    }
                    else if ($g(SEL.Logon.UsernameDomID).value === "")
                    {
                        $g(SEL.Logon.UsernameDomID).focus();
                    }
                    else
                    {
                        $g(SEL.Logon.PasswordDomID).focus();
                    }
                }
            },
            ReInitializeWrapper: function (sliderObject) {
                if (($(window).width() <= 1280) || ($(window).width() < 1520 && $(window).height() > 801)) {
                    $('.sliderImage').addClass('sliderImage-resize');
                } else {
                    $('.sliderImage').removeClass('sliderImage-resize');
                }

                if (($(window).width() <= 1024) || ($("#bxSlider li").length < 1)) {
                    if (typeof sliderObject !== 'undefined') {
                        sliderObject.destroySlider();
                    }
                    $('#right').hide();
                    $('#left').removeClass('left-wrapper');
                } else {
                    $('#right').show();
                    $('#left').addClass('left-wrapper');
                    if (typeof sliderObject !== 'undefined') {
                        sliderObject.reloadSlider();
                    } 
                }

                $('.slider-container-ie .bx-viewport').attr('style', 'height:' + $(window).height() + 'px !important').addClass('slider-container-ie');
                SEL.Logon.AlignModals();

            },
            AlignModals: function () {
                var marginForLoginModal = ($('#left').width() - $('#modal-panel').width()) / 2;
                $('#modal-panel').css('margin-left', marginForLoginModal - 5 + 'px');
                marginForLoginModal = ($('#left').height() - $('#logonTopPanel').height()) / 2;
                $('#modal-panel').css('margin-top', marginForLoginModal + 'px');
            },
            PreviewRequestHandler: function () {
                //Disable events clicks on Logon page
                $('#btnLogon').click(function (e) {
                    e.preventDefault();
                });
                $('#lnkSelfRegistration').attr('onclick', 'return false;');
                $("#lnkForgottenDetails").unbind("click");

                var icon = sessionStorage.getItem('iconForTitle');
                var header = sessionStorage.getItem('LogonCategoryTitle');
                var headerColor = sessionStorage.getItem('LogonCategoryTitleColor');
                var title = sessionStorage.getItem('LogonHeader');
                var titleColor = sessionStorage.getItem('LogonHeaderColor');
                var copy = sessionStorage.getItem('LogonBody');
                var copyColor = sessionStorage.getItem('LogonBodyColor');
                var banner = sessionStorage.getItem('backGroundImage');
                var buttonText = sessionStorage.getItem('ButtonText');
                var buttonColor = sessionStorage.getItem('ButtonTextColor');
                var buttonBackground = sessionStorage.getItem('ButtonBackground');
                var link = sessionStorage.getItem('ButtonLink');
                var shouldDisplayButtons = 'visible';
                var shouldDisplayicon = 'iconHolderForBanner';
                var initialIcon = sessionStorage.getItem('initialIconImage');
                if (link === undefined || link == '' || link === null) {
                    shouldDisplayButtons = 'hidden';
                }
                

                if (banner === undefined || banner == '' || banner === null) {
                    banner = '../Logos/MarketingInformation/' + sessionStorage.getItem('initialBackGroundImage');
                } else {
                    banner = '/shared/images/logonimages/' + banner;
                }
                if ((icon === undefined || icon == '' || icon === null) && (initialIcon === undefined || initialIcon == '' || initialIcon === null)) {
                    shouldDisplayicon = 'hideElement';
                }
                else if (icon === undefined || icon == '' || icon === null) {
                    icon = '../Logos/MarketingInformation/icons/' + initialIcon;
                } else if (initialIcon === undefined || initialIcon == '' || initialIcon === null) {
                    icon = '/shared/images/logonimages/' + icon;
                }  
                var skeleton = '<ul id="bxSlider"><li><div class="contentHolder"><span><img id="icon" class="'+ shouldDisplayicon + '"' +
                'src="' + icon + '" /> </span><span class="content-title" style="color:#' + headerColor + ';">' +
                header + '</span><div class="content-description"><p class="title" style="color:#' + titleColor + '">' + title + '</p>' +
                '<p class="copy" style="color:#' + copyColor + ';">' + copy + '</p><a class="banner-button" href="' + link + '" target="_blank" '
                + 'style="color:#' + buttonColor + '; background-color:#' + buttonBackground + '; visibility:' + shouldDisplayButtons + ';">' + buttonText + '</a></div></div>' +
                '<img class="sliderImage lazy" src="' + banner + '" /></li></ul>';

                $('#previewRequest').html(skeleton);
                $('.ui-icon-closethick').click(function () {
                    sessionStorage.clear();
                });
            },
            PageLoadfunctionsForSliders: function () {
                if (!($("#bxSlider li").length >= 1) && window.location.search.indexOf('previewId') != -1) {
                    SEL.Logon.PreviewRequestHandler();
                }
               
                $('.preLoaderDiv').hide();
                //avoid duplications 
                function debouncer(func, timeout) {
                    var timeoutID, timeout = timeout || 50;
                    return function () {
                        var scope = this,
                         args = arguments;
                        clearTimeout(timeoutID);
                        timeoutID = setTimeout(function () {
                            func.apply(scope, Array.prototype.slice.call(args));
                        }, timeout);
                    }
                }

                //Initialize slider
                var sliderObject = $('#bxSlider').bxSlider({
                        controls: true,
                        infiniteLoop: true,
                        hideControlOnEnd: true,
                        randomStart: true,
                        startSlide: 0,
                        pager: ($("#bxSlider li").length > 1) ? true : false,
                        onSliderLoad: function () {                            
                                $("#bannerHolder").css("visibility", "visible");                           
                            if ($("#bxSlider li").length == 1) {
                                $("#right").addClass('reduce-width');
                                if($('.page-wrapper').width() > ($('#right').width() + $('#left').width() + 20))
                                {
                                    $('#bxSlider li').width($('#right').width() + ($('.page-wrapper').width()-($('#right').width() + $('#left').width())));
                                }
                            }                            
                            $('.slider-container-ie .bx-viewport').attr('style', 'height:' + $(window).height() + 'px !important').addClass('slider-container-ie');
                            SEL.Logon.AlignModals();
                            $(".contentHolder").each(function () {
                                var height = $('#right').height() - $(this).height() - $('#titlebar').height() + 40;
                                $(this).css('top', height/2);
                            });
                        }
                    });                    
                   
                   //Rebuild Slider with the same object
                    var alignWrapperOverResize;
                   // Resize the wrappers after pageload
                    this.alignWrapperOverResize = SEL.Logon.ReInitializeWrapper(sliderObject);
                    $(window).resize(debouncer(function (e) {
                        this.alignWrapperOverResize = undefined;
                        this.alignWrapperOverResize = SEL.Logon.ReInitializeWrapper(sliderObject);
                        //Check if the sliders width is correct.
                        setTimeout(function(){if ($('#right').width() > ($('#bxSlider li').width() + 25)) {
                            this.alignWrapperOverResize = undefined;
                            this.alignWrapperOverResize = SEL.Logon.ReInitializeWrapper(sliderObject);                                                   }
                        }, 200);
                }));
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
}());