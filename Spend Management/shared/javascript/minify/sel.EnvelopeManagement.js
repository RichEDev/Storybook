/*global SEL:false, $g:false, $:false, Spend_Management: false */
(function (sel) {
    var scriptName = "EnvelopeManagement";

    function execute() {
        sel.registerNamespace("SEL.EnvelopeManagement");

        sel.EnvelopeManagement = {
            Ids: {

                AccountId: null,
                EnvelopeId: null,
                EnvelopeNumber: null,
                EmployeeId: null,
                ClaimId: null,
                ClaimName: null

            },

            Elements: {

                EnvelopeTree: null,
                ClaimSelection: null,
                AssignmentButtons: null,
                DefaultCancelContainer: null,
                CancelButton: null,
                SearchButtonContainer: null,
                ConfirmModal: null,
                ConfirmButtons: null

            },

            General: {

                Init: function () {
                    
                    sel.EnvelopeManagement.Elements.AssignmentButtons = $("#AssignmentButtons");
                    sel.EnvelopeManagement.Elements.DefaultCancelContainer = $("#DefaultCancelContainer");
                    sel.EnvelopeManagement.Elements.CancelButton = sel.EnvelopeManagement.Elements.DefaultCancelContainer.find('input[type=submit]').parent();
                    sel.EnvelopeManagement.Elements.ClaimSelection = $("#ClaimSelection");
                    sel.EnvelopeManagement.Elements.SearchButtonContainer = $("#SearchButtonContainer");
                    sel.EnvelopeManagement.Elements.ConfirmButtons = $("#ConfirmButtons");

                    // make a modal of the confirm
                    sel.EnvelopeManagement.Elements.ConfirmModal = sel.Common.BuildJqueryDialogue("#ConfirmModal", 860, 130);

                    $(document).keydown(function (e) {
                        if (e.keyCode === 13) // enter
                        {
                            e.preventDefault();
                        }
                    });

                    // listen for the search click
                    sel.EnvelopeManagement.Elements.SearchButtonContainer.click(function () {
                        sel.EnvelopeManagement.Elements.DefaultCancelContainer.append(sel.EnvelopeManagement.Elements.CancelButton);
                    });

                    // listen for a radio button change
                    sel.EnvelopeManagement.Elements.ClaimSelection.on('change', 'input[type=radio]', null, function() {

                        var button = $(this);

                        // save the claim Id + name
                        sel.EnvelopeManagement.Ids.ClaimId = parseInt(button.val());
                        sel.EnvelopeManagement.Ids.ClaimName = button.parent().next().next().find('a').text();

                        // update UI
                        if (!isNaN(sel.EnvelopeManagement.Ids.ClaimId)) {
                            sel.EnvelopeManagement.Elements.AssignmentButtons.append(sel.EnvelopeManagement.Elements.CancelButton);
                            sel.EnvelopeManagement.Elements.AssignmentButtons.show();
                        }
                    });

                    // hide the ui elements
                    sel.EnvelopeManagement.General.ResetUi();

                    // init tree
                    sel.EnvelopeManagement.SourceTree.Init();
                    
                    return false;
                },

                // assign the selected envelope to an employee
                ShowConfirmationModal: function () {

                    // rewrite contents
                    $('#ConfirmEnvelopeNumber').text(sel.EnvelopeManagement.Ids.EnvelopeNumber);
                    $('#ConfirmClaimName').text(sel.EnvelopeManagement.Ids.ClaimName);
                    
                    // show
                    sel.EnvelopeManagement.Elements.ConfirmModal.dialog('open');

                    return false;
                },

                // assign the selected envelope to an employee
                HideConfirmationModal: function () {

                    // show
                    sel.EnvelopeManagement.Elements.ConfirmModal.dialog('close');

                    return false;
                },

                // assign the selected envelope to an employee
                AssignToClaim: function () {


                    // Call to fetch the recent claims
                    $.ajax({
                        url: "expenses/webServices/claims.asmx/AssignEnvelopeToClaim",
                        method: "POST",
                        data: '{ envelopeId: ' + sel.EnvelopeManagement.Ids.EnvelopeId + ', claimId: ' + sel.EnvelopeManagement.Ids.ClaimId + ' }',
                        contentType: 'application/json; charset=utf-8',
                        dataType: "json",
                        success: function () {

                            // remove node from tree and hide overlay
                            sel.EnvelopeManagement.Elements.EnvelopeTree.removeNode(sel.EnvelopeManagement.Ids.EnvelopeId);
                            sel.EnvelopeManagement.Elements.EnvelopeTree.rebuildTree();
                            sel.EnvelopeManagement.General.ResetUi();

                        },
                        error: function (xmlHttpRequest, textStatus, errorThrown) {
                            var reasonForFailure = JSON.parse(xmlHttpRequest.responseText).Message;
                            // show the text box.
                            sel.MasterPopup.ShowMasterPopup('There was an issue assigning this envelope to this claim:<br/>' + reasonForFailure + '.<span style="display:none;">' + errorThrown + '</span>',
                                'Message from ' + window.moduleNameHTML);
                        }
                    });

                    return false;
                },

                // gets the active node from the tree recursively
                GetActiveNode: function (nodes) {

                    var node;
                    for (var i = 0; i < nodes.length; i++) {

                        node = nodes[i];
                        if (node.isActive) return node;

                        if (node.children && node.children.length) {
                            node = sel.EnvelopeManagement.General.GetActiveNode(node.children);
                            if (node) return node;
                        }
                    }
                    return null;
                },

                // gets the active node from the tree recursively
                ResetUi: function () {

                    sel.ClaimSelector.ClearClaimant();
                    sel.ClaimSelector.ClearClaimName();
                    sel.ClaimSelector.HideSearchModal();
                    sel.EnvelopeManagement.Elements.AssignmentButtons.hide();
                    sel.EnvelopeManagement.Elements.ClaimSelection.hide();
                    sel.EnvelopeManagement.Elements.ConfirmModal.dialog('close');
                    sel.EnvelopeManagement.Elements.DefaultCancelContainer.append(sel.EnvelopeManagement.Elements.CancelButton);

                    return false;
                }

            },

            // the tree on the left, only for account admins - contains envelopes and receipts for identification
            SourceTree: {

                // init the initial tree and pass on the initial fetch
                Init: function () {

                    SEL.Data.Ajax({
                        url: "/webServices/receipts.asmx/GetEnvelopeSourceTree",
                        data: {
                            accountId: sel.EnvelopeManagement.Ids.AccountId
                        },
                        success: function(data) {
                            if (data.d) {
                                sel.EnvelopeManagement.Elements.EnvelopeTree = $('#SourceTree').easytree({
                                    allowActivate: true,
                                    data: data.d,
                                    stateChanged: sel.EnvelopeManagement.SourceTree.HandleStateChange
                                });
                            }
                        }
                    });

                    return false;
                },

                HandleStateChange:  function (nodes) {

                    $("#ReceiptPreview").html('');

                    var node = sel.EnvelopeManagement.General.GetActiveNode(nodes);

                    if (node == null) {

                        // hide the employee / claim finder
                        sel.EnvelopeManagement.General.ResetUi();
                        return true;
                    }

                    if (node.isFolder) {

                        // set the id
                        sel.EnvelopeManagement.Ids.EnvelopeId = node.id;
                        sel.EnvelopeManagement.Ids.EnvelopeNumber = node.text;

                        // show the other content
                        sel.ClaimSelector.ClearClaimant();
                        sel.ClaimSelector.ClearClaimName();
                        sel.EnvelopeManagement.Elements.ClaimSelection.show();
                        sel.EnvelopeManagement.Elements.SearchButtonContainer.append(sel.EnvelopeManagement.Elements.CancelButton);
                        return false;

                    } else {

                        // hide the other content
                        sel.EnvelopeManagement.General.ResetUi();

                        // Call to fetch the receipt from the cloud
                        $.ajax({
                            url: "webServices/receipts.asmx/GetReceiptFromCloudById",
                            method: "POST",
                            data: '{ "id": ' + node.id + ' }',
                            contentType: 'application/json; charset=utf-8',
                            dataType: "json",
                            success: function (data) {

                                data = JSON.parse(data.d);
                                var image = data.isImage
                                    ? $('<div/>').append($('<img/>').attr("src", data.url))
                                    : $('<div><a href="' + data.url + '" target="_blank"><img src="' + data.icon + '"></div><p>This receipt cannot be viewed here, click to download it instead.</p></a></div>');

                                $("#ReceiptPreview").html(image);
                            }
                        });

                        return false;
                    }
                }

            }  // end SourceTree

        }; // end SEL.EnvelopeManagement


    } // end execute()

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL));
