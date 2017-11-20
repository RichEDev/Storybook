function showTooltip(evt, controlID, helpID)
{
    var e = (evt) ? evt : window.event;

    if (window.event) {
        e.cancelBubble = true;
    } else {
        e.stopPropagation();
    }
    
    populateTooltip(helpID);
    $find('ctl00_popuptooltip')._popupBehavior._parentElement = document.getElementById(controlID);
    $find("ctl00_popuptooltip").showPopup();
}

function populateTooltip(helpID)
{
    var behaviour = $find('ctl00_dynpopup');
    if (behaviour)
    {
        behaviour.populate(helpID);
    }
}

function showNoMasterPageTooltip(evt, controlID, helpID) {

    var e = (evt) ? evt : window.event;

    if (window.event) {
        e.cancelBubble = true;
    } else {
        e.stopPropagation();
    }
    
    populateNoMasterPageTooltip(helpID);
    $find('popuptooltip')._popupBehavior._parentElement = document.getElementById(controlID);
    $find("popuptooltip").showPopup();
}

function populateNoMasterPageTooltip(helpID) {
    var behaviour = $find('dynpopup');
    if (behaviour) {
        behaviour.populate(helpID);
    }
}
