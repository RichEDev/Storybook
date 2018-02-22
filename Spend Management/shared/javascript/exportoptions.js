function initialiseGrid(data) {
    var gridData = JSON.parse(data);
    $("#gridFlatFile").ejGrid({
        dataSource: gridData,
        columns: [
            { field: "reportcolumnid", visible: false },
            { field: "columnname", headerText: "Column", isPrimaryKey: true },
            { field: "fieldlength", headerText: "Length", editType: "numericedit" }
        ],
        editSettings: {
            allowEditing: true
        }
    });
}

function Save() {
    var fieldLengths = {};
    $(".e-row, .e-alt_row").each(function () {
        var fieldid = $(this).find(".e-rowcell:nth-child(1)").text();
        var length = $(this).find(".e-rowcell:nth-child(3)").text();
        fieldLengths[fieldid.toString()] = length;
    });
    $(".e-editedrow").each(function () {
        var length = $(this).find(".e-numerictextbox").val();
        var fieldid = $(this).find(".e-rowcell:nth-child(1) input").val();
        fieldLengths[fieldid.toString()] = length;
    });

    PageMethods.SaveExportOptions($("#optdelimitertab").is(':checked'), $("#txtdelimiter").val(), $("#cmbfooter").val(), new URL(window.location.href).searchParams.get("reportid"),
        $("#chkshowheadersexcel").is(':checked'), $("#chkshowheaderscsv").is(':checked'), $("#chkshowheadersflat").is(':checked'), $("#chkremovecarriagereturns").is(':checked'), $("#chkencloseinspeechmarks").is(':checked'), JSON.stringify(fieldLengths), CloseWindow
    );

}

function CloseWindow(result) {
    if (result) {
        window.close();
    }
}