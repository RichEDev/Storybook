$(document).ready(function () {
    var width = $(window).width();
    var height = $(window).height();
    if ((width < 950)) {
        loadCSS("mediaQueryMobileStylesheet",
            "/static/styles/expense/css/mobileMediaQuery.css");
    }

    $('.col-md-6:odd').addClass('row_end');
    //Disable horizontal scroll only in menu item pages
    $('body').css('overflow-x','hidden');
    menuStabalize();
    $(window).resize(function () {
        clearTimeout(window.menuPageResizeFinished);
        window.menuPageResizeFinished = setTimeout(function () {
        menuStabalize();
        }, 200);
       
    });
    function menuStabalize() {
        var outerHeight = 0;
        outerHeight = $('.col-md-12').height() + 20;
        $(".content-wrapper").height(outerHeight + 'px');

        width = $(window).width(), height = $(window).height();

        if (((width <= 1024) && (height <= 768)) || ((height <= 1024) && (width <= 768))) {
            $('.col-md-6').each(function () {
                var lettersCount = $(this).find('.media-body p').text().length;
                var data = $(this).find('.media-body p').text().toString();

                if ((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent))&&(window.matchMedia("(orientation: portrait)").matches)) {
                    if (lettersCount > 140) {
                        data = data.substring(0, 140);
                        $(this).find('.media-body p').text(data + '...');
                    }
                } else {
                    if (lettersCount > 165) {
                        $(this).find('.well').height('auto');
                        var autoHeight = $(this).find('.well').height();
                        if ($(this).hasClass('row_end') && ($(this).prev().find('.well').height() < autoHeight)) {
                            $(this).prev().find('.well').height(autoHeight + 10);
                            $(this).find('.well').height(autoHeight + 10);
                        }
                        else if (!$(this).hasClass('row_end')) {
                            $(this).next().find('.well').height(autoHeight + 10);
                            $(this).find('.well').height(autoHeight + 10);
                        }

                    }
                }

            });
        } else {
            {
                $('.col-md-6').each(function () {
                    var lettersCount = $(this).find('.media-body p').text().length;
                    if (lettersCount > 235) {
                        $(this).find('.well').height('auto');
                        var autoHeight = $(this).find('.well').height();
                        if ($(this).hasClass('row_end') && ($(this).prev().find('.well').height() < autoHeight)) {
                            $(this).prev().find('.well').height(autoHeight + 10);
                            $(this).find('.well').height(autoHeight + 10);
                        } else if (!$(this).hasClass('row_end')) {
                            $(this).next().find('.well').height(autoHeight + 10);
                            $(this).find('.well').height(autoHeight + 10);
                        }
                    }
                });
            }
        }
    }

});

