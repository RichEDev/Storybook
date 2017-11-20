(function (SEL) {
	"use strict";
	
	var scriptName = "Colours";
	function execute() {
		SEL.registerNamespace("SEL.Colours");
		SEL.Colours = {
			/* IDs and Variables */

			/* Messages */
			MessageTitle: "Colour Details",
			RestoreWarning: "This will restore all of the colours back to their default values.\nAre you sure you want to do this?",
			RestoreError: "Sorry, an error occurred while restoring the default colours",

			/* Methods */
			Restore: function () {
				if (window.confirm(SEL.Colours.RestoreWarning)) {

					Spend_Management.svcColours.Restore(function () {

						window.location.reload();

					}, function () {

						SEL.MasterPopup.ShowMasterPopup(SEL.Colours.RestoreError, SEL.Colours.MessageTitle);

					});
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
} (SEL));
