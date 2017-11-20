(function (moduleNameHtml) {
    var scriptName = "helpAndSupportTickets";

    function execute() {
        SEL.registerNamespace("SEL.HelpAndSupportTickets");
        SEL.HelpAndSupportTickets =
        {
            TicketType: null,

            Identifiers:
            {

            },

            Dom:
            {
                
            },

            Page:
            {
                Validators: {
                    Disclaimer: function (sender, e) {
                        e.IsValid = $(".checkboxAgreement input:checkbox").is(":checked");
                    }
                }
            },

            Setup: function (ticketType) {

                this.TicketType = ticketType;

                // client side validation check on the search/comment/attachment form
                $(".ticketForm input[type=submit], .ticketCommentForm input[type=submit]").on("click", function (event) {
                    if (!validateform()) {
                        event.preventDefault();
                    }
                });

                // attachment download behaviour
                $(".ticketAttachments a").on("click", function (event) {
                    event.preventDefault();

                    var attachmentId = $(this).data("attachment");
                    var ticketId = $(this).data("ticket");

                    SEL.HelpAndSupportTickets.DownloadAttachment(ticketId, attachmentId);
                });

                SEL.Common.SetTextAreaMaxLength();
            },

            DownloadAttachment: function (ticketId, attachmentId) {
                var frame = $("#attachmentDownloadFrame");

                // create the frame if it's not in the DOM yet
                if (!frame.length) {
                    frame = $("<iframe>").attr("id", "attachmentDownloadFrame");
                    $("body").append(frame);
                }

                if (this.TicketType === "SalesForce") {
                    frame.attr("src", "shared/getDocument.axd?supportTicketSalesForceAttachmentId=" + attachmentId + "&supportTicketSalesForceId=" + ticketId);
                } else if (this.TicketType === "Internal") {
                    frame.attr("src", "shared/getDocument.axd?supportTicketAttachmentId=" + attachmentId + "&supportTicketId=" + ticketId);
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