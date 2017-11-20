(function () {
    var scriptName = "AccessRole";
    function execute() {
        SEL.registerNamespace("SEL.AccessRole");

        SEL.AccessRole =
        {
            SelectedFields: new Array(),
            RemovedFields: new Array(),
            ExclusionType: null,
            OnPageLoad: true,
            ChangeReportableFieldsWrapper: function (selectedExclusion) {
                if (selectedExclusion === "2") {
                    $(".reportableTables").hide();
                    $(".reportingFields").hide();
                } else {
                    setTimeout(function () {
                        if ($(".reportableTables").is(":hidden")) {
                            $(".reportableTables").show();
                            $(".reportingFields").show();
                            if (SEL.AccessRole.OnPageLoad){ $(".ddlProductArea").trigger("change");}
                            SEL.AccessRole.OnPageLoad = false;
                        }
                        }, 500);

                }
            },
            LoadReportableFieldsLogic: function (accessRoleId) {
                SEL.AccessRole.ChangeReportableFieldsWrapper(SEL.AccessRole.ExclusionType);
                SEL.AccessRole.GroupReportSource();
                $('.ddlProductArea').change(function (e) {
                    e.preventDefault();
                    SEL.AccessRole.LoadFieldsForReportingTables($(".ddlProductArea option:selected").val(), accessRoleId);
                });

                $('.ddlExclusionType').change(function (e) {
                    e.preventDefault();
                    var selectedExclusion = $(".ddlExclusionType option:selected").val();
                    SEL.AccessRole.ExclusionType = selectedExclusion;
                    SEL.AccessRole.ChangeReportableFieldsWrapper(selectedExclusion);
                });

                 
                $(".reportingFields").on("change", "input[type='checkbox']", function () {
                    if (this.id === "selectAllReportableFields") {
                        if (this.checked) {
                            $(".reportingFields [id^='tbl_ReportableFields_'] input[type='checkbox']").trigger("change").prop("checked", true);
                        } else {
                            $(".reportingFields [id^='tbl_ReportableFields_'] input[type='checkbox']").trigger("change").prop("checked", false);
                        }
                    }
                    else {
                        var ifTheElementExists = SEL.AccessRole.RemovedFields.indexOf(this.value);
                        if (ifTheElementExists !== -1) {
                            SEL.AccessRole.RemovedFields.splice(ifTheElementExists, 1);
                        }

                        ifTheElementExists = SEL.AccessRole.SelectedFields.indexOf(this.value);

                        if (ifTheElementExists !== -1) {
                            SEL.AccessRole.SelectedFields.splice(ifTheElementExists, 1);
                        }

                        if (this.checked) {
                            SEL.AccessRole.SelectedFields.push(this.value);
                        } else {
                            SEL.AccessRole.RemovedFields.push(this.value);
                        }
                    }
                });
            },
            LoadFieldsForReportingTables: function (tableId, accessRoleId) {
                Spend_Management.svcAccessRoles.PopulateReportingFields(tableId, accessRoleId, SEL.AccessRole.RefreshGrid);

            },

            GroupReportSource: function () {
                var groups = {};
                var selector = ".ddlProductArea";
                var value = $(selector).val();

                $(selector + " option[data-category]").each(function () {
                    groups[$.trim($(this).attr("data-category"))] = true;
                });
                $.each(groups, function (c) {
                    $(selector + " option[data-category='" + c + "']").wrapAll('<optgroup label="' + c + '">');
                });

                $(selector).val(value);
            },

            RefreshGrid: function (data) {
                $(".reportingFields").html(data[1]);
                $.when(SEL.Grid.updateGrid(data[0])).done(function () {
                    SEL.AccessRole.SelectedFields.forEach(function (fieldId) {
                        $("#tbl_ReportableFields_" + fieldId + " input[type='checkbox']").attr('checked', true);
                    });

                    SEL.AccessRole.RemovedFields.forEach(function (fieldId) {
                        $("#tbl_ReportableFields_" + fieldId + " input[type='checkbox']").attr('checked', false);
                    });
                });

            }


        }
    }
    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}());