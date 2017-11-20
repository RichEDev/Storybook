/// <summary>
/// Broadcast Messages
/// </summary>        
(function () {
    var scriptName = "BroadcastNotesMessages";
    function execute() {
        SEL.registerNamespace("SEL.BroadcastNotesMessages;");
        SEL.BroadcastNotesMessages =
        {
            Message: { Broadcast: 1, Note: 2 },
            BroadcastMessageDivDomID: null,
            NotesMessageDivDomID: null,
            _GetDomObject: function (messageType) {
                if (message === SEL.BroadcastNotesMessages.Message) {
                    return SEL.BroadcastNotesMessages.BroadcastMessageDivDomID;
                }
                else {
                    return SEL.BroadcastNotesMessages.NotesMessageDivDomID;
                }
            },
            _ShowMessage: function (noteID, broadcastID) {

            },
            _CloseMessage: function () {

            },
            GetBroadcastMessage: function () {

            },
            GetBroadcastMessageComplete: function () {

            },
            GetNotesMessage: function () {

            },
            GetNotesMessageComplete: function () {

            }
        };
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}
)();