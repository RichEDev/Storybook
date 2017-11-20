/// <reference path="jquery-1.10.2.js" />
/// <reference path="jquery-ui-1.9.2.js" />

var lastSelectedLetter = "";
var lastLetterIndex = -1;
var currentTargetIds;
var currentBeginningLetters;
var apiListHeaders;

function animateToTarget(target)
{
    target = $(target);
    if (target) {
        apiListHeaders.next(":visible").hide();
        $('html, body').stop().animate({ scrollTop: (target.offset().top - 40 - target.height()) }, 500);
        target.next().toggle(true, 'blind');
    }
    return target;
}

function calculateNearestTarget(keyEvent)
{
    var letter = String.fromCharCode(keyEvent.keyCode);

    if (letter && currentBeginningLetters.length)
    {
        var i = lastLetterIndex;
        var fi = currentBeginningLetters.indexOf(letter);
        var li = currentBeginningLetters.lastIndexOf(letter) + 1;
        if (fi != -1)
        {
            i = (letter != lastSelectedLetter) ? fi : (li - lastLetterIndex > 0) ? lastLetterIndex + 1 : li;
            if (i == li) i = fi;
            
            var targetId = currentTargetIds.get(i);
            var target = $('header.api-list-header#' + targetId);
            target.click();
        }

        lastLetterIndex = i;
        lastSelectedLetter = letter;
    }
}


jQuery(document).ready(function () {
    
    apiListHeaders = $('header.api-list-header');
    currentTargetIds = apiListHeaders.map(function () { return this.id; });
    currentBeginningLetters = currentTargetIds.map(function () { return this.substring(0, 1); }).toArray();

    apiListHeaders.click(function(event) {
        event.preventDefault();
        var target = animateToTarget(this);
        window.location.hash = target.attr('id');
    });

    var linkWasOpened = window.location.hash;
    if (linkWasOpened) {
        animateToTarget(linkWasOpened);
    }

    $("a.help-page-api-element-link").click(function (event) {
        var href = $(this).attr("href");
        var lastHashIndex = href.lastIndexOf("#");
        var page = href.substring(0, lastHashIndex);
        if (window.location.pathname.indexOf(page) < 0) {
            return;
        }
        
        event.preventDefault();
        var fragment = href.substring(lastHashIndex);
        var target = animateToTarget(fragment);
        window.location.hash = target.attr('id');
    });

    $(this).keyup(function(e) { calculateNearestTarget(e); });

    $('#api-tabs').tabs();
});


