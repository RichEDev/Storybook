$(document).ready(function () {

    PageMethods.GetGriDataSet(InitialiseGrid);

    function InitialiseGrid(data) {

        var gridData = JSON.parse(data);

        $("#tooltipsGrid").ejGrid({
            dataSource: gridData,
            columns: [
                { field: "edit", headerText: "<img src=\"/shared/images/icons/edit.png\" />", width: "30", allowSorting: false },
                { field: "page", headerText: "Page" },
                { field: "description", headerText: "Descrption" },
                { field: "text", headerText: "Text" }
            ],
            allowPaging: true,
            allowGrouping: true,
            allowSorting: true,
            groupSettings: { groupedColumns: ["page"] },
            allowTextWrap: true,
            textWrapSettings: { wrapMode: "both" }
        });

        $(".e-ungroupbutton").remove();
    }
});