/*
** Pool Cars Summary
*/
function DeletePoolCar(carid)
{
    if (confirm('Are you sure you wish to permanently delete the selected pool vehicle and remove it from all associated accounts?'))
    {
        Spend_Management.svcPoolCars.DeletePoolCar(carid, DeletePoolCarComplete, errorMessage);
    }
    return;
}
function DeletePoolCarComplete(retVal)
{
    switch (retVal)
    {
        case 0:
            PopulatePoolCarGrid();
            break;
        case 1:
            SEL.MasterPopup.ShowMasterPopup("Cannot delete pool vehicle as it is associated to a current, submitted or previous expense item.", 'Message from ' + moduleNameHTML);
            break;
        case -10:
            SEL.MasterPopup.ShowMasterPopup("Cannot delete pool vehicle as it is associated to a GreenLight or user defined field record.", 'Message from ' + moduleNameHTML);
            break;
        default:
            break;
    }
}

function PopulatePoolCarGrid()
{
    Spend_Management.svcPoolCars.CreatePoolCarsGrid(PopulatePoolCarGridComplete, errorMessage);
}

function PopulatePoolCarGridComplete(data)
{
    if ($e(pnlPoolCarsGridID) === true)
    {
        $g(pnlPoolCarsGridID).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
    return;
}

/*
** Add/Edit Pool Car
*/
function AddUserToPoolCar()
{
    if (carid == 0)
    {
        SEL.MasterPopup.ShowMasterPopup("The pool vehicle details must be saved before you can assign users to it.", "Page Validation Message");
        return;
    }
    SEL.Grid.clearSelectAllOnGrid('gridPoolCarEmployees');
    $find(mdlAddUserID).show();
    return;
}

function SaveAddUsers()
{
    var employeeIDs = Array();
    var gridInputs = document.getElementById('tbl_gridPoolCarEmployees').getElementsByTagName('input');
    for (i = 1; i < gridInputs.length; i++)
    {
        if (gridInputs[i].checked == true)
        {
            var empID = parseInt(gridInputs[i].value);
            if (isNaN(empID) == false)
            {
                employeeIDs.push(empID);
            }
        }
    }
    Spend_Management.svcPoolCars.SaveUsersToCar(carID, employeeIDs, SaveAddUsersComplete, errorMessage);
    return;
}
function SaveAddUsersComplete(data)
{
    if ($e(pnlPoolCarUsersGridID) === true) {
        $g(pnlPoolCarUsersGridID).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
    $find(mdlAddUserID).hide();
    return;
}

function CancelAddUsers()
{
    $find(mdlAddUserID).hide();
    return;
}

function DeletePoolCarUser(employeeID)
{
    if (confirm("Are you sure you wish to remove the selected user's access to this pool vehicle?"))
    {
        Spend_Management.svcPoolCars.DeletePoolCarUser(carID, employeeID, DeletePoolCarUserComplete, errorMessage);
    }
    return;
}
function DeletePoolCarUserComplete(data)
{
    if ($e(pnlPoolCarUsersGridID) == true)
    {
        $g(pnlPoolCarUsersGridID).innerHTML = data[2];
        SEL.Grid.updateGrid(data[1]);
    }
    return;
}


/*
** Common
*/
function errorMessage(data)
{
    if (data["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(data["_message"], "Web Service Error");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(data, "Web Service Error");
    }
}
