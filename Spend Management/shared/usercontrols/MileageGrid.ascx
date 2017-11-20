<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MileageGrid.ascx.cs" Inherits="Spend_Management.shared.usercontrols.MileageGrid" %>


    <table id="mileagegridtable">
    <thead>
        <tr>
            <td>&nbsp;</td>
            <td>From</td>
            <td>To
                <input type="hidden" id="mileagegridtable_numrows" name="mileagegridtable_numrows" value="0"/>
            </td>
            <!--
            <td>Distance Travelled</td>
            <td>Recommended Distance</td>
            <td>Passengers:</td>
            <td>Heavy/Bulky Equipment?</td>
                -->
        </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    <span data-field="mileagerownum" style="display: none; width: 50px"></span>
                </td>
                <td>
                    <input type="text" data-from="true" placeholder="Search..." data-search="true"/>
                    <input type="hidden" data-from="true" data-address-id="true"/>
                </td>
                <td>
                    <input type="text" data-to="true" placeholder="Search..." data-search="true"/>
                    <input type="hidden" data-to="true" data-address-id="true"/>
                </td>
                <td>
                    <div class="loading" ></div>
                    <span data-field="recommendeddistance"></span>
                </td>
            </tr>
        </tbody>
</table>

<script>

    function isLastMileageGridRowBlank() {
        var lastTr = $("#mileagegridtable tr:last");
        var isBlank = true;
        $("input[data-search]", lastTr).each(function () {
            if ($(this).val()) isBlank = false;
        });
        return isBlank;
    }

    function calculateMileage(fromAddressId, toAddressId, tr) {
        var resultHolder = $("span[data-field='recommendeddistance']", tr);
        var loadingDiv = $(".loading", resultHolder.closest("td"));
        loadingDiv.show();
        SEL.Data.Ajax({
            data: { "fromAddressId": fromAddressId, "toAddressId": toAddressId },
            methodName: "GetCustomOrRecommendedDistance",
            serviceName: "svcAddresses",
            success: function (data) {
                resultHolder.text(data.d);
                loadingDiv.hide();
            }
        });
    }

    function mileageGridAddressSet() {
        var tr = $(this).closest("tr");
        var from = $("input[data-from][type='hidden']", tr);
        var to = $("input[data-to][type='hidden']", tr);
        //are they both filled in?
        if (from.val() && to.val() && !isLastMileageGridRowBlank()) {
            var newTr = $("#mileagegridtable tr:last").clone(); //make a duplicate of the last row
            $("input", newTr).val("");//... and clear it out
            $("span[data-field='mileagerownum']", tr).hide();
            newTr.insertAfter($("#mileagegridtable tr:last")); //... and insert it into the table
            setAddressInputNames();
            initializeAddressPickerOnMileageGridControls($("input[data-search='true']", newTr));

            $("span[data-field='mileagerownum']", tr).show(); //make the row number marker in this row visible
            $("#mileagegridtable_numrows").val($("span[data-field='mileagerownum']:visible").length);//set the row num counter to be however many rows have been filled in (in the whole table)

            calculateMileage(from.val(), to.val(), tr);
        } else if ($(this).attr("data-from") === "true" && !to.val()) {
            $("input[data-to][data-search]", tr).focus();
        }
    }

    function initializeAddressPickerOnMileageGridControls(controls) {
        controls.address();
    }

    $(function () {
        setAddressInputNames();
        initializeAddressPickerOnMileageGridControls($("#mileagegridtable input[data-search='true']"));
        $("#mileagegridtable").on("change", "input[data-address-id='true']", mileageGridAddressSet);
    });

    function setAddressInputNames() {
        $("#mileagegridtable tbody tr").each(function (i) {
            var tr = this;
            var fromId = "mileagegrid_fromid_" + i;
            var toId = "mileagegrid_toid_" + i;
            $("input[data-from='true'][data-search='true']", tr).attr("rel", fromId);
            $("input[data-from='true'][data-address-id='true']", tr).attr("id", fromId).attr("name", fromId);
            $("input[data-to='true'][data-search='true']", tr).attr("rel", toId);
            $("input[data-to='true'][data-address-id='true']", tr).attr("id", toId).attr("name", toId);
            $("span[data-field='mileagerownum']", tr).text(i + 1);
        });
    }
</script>
