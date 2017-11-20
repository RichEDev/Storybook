function checkStatus()
{
    if (!validateform('tasks'))
    {
        return false;
    }
    var returnVal = true;

    switch (statusID)
    {
        case "2":
            if (!confirm("Postponing the task will halt any notifications for the task.\nClick save to confirm"))
            {
                returnVal = false;
            }
            break;
        case "3":
            if (!confirm("Cancelling a task will archive it.\nClick save to confirm"))
            {
                returnVal = false;
            }
            break;
        case "4":
            if (!confirm("You are about to set the task as completed. This will close the task.\nClick save to confirm"))
            {
                returnVal = false;
            }
            break;
        default:
            break;
    }

    return returnVal;
}

function setStatus()
{
    var cmbStatus = document.getElementById(cmbStatusID);

    statusID = cmbStatus[cmbStatus.selectedIndex].value;
}

function validateTaskStartDate(sender, args)
{
    var dtTaskStart = document.getElementById(dtTaskStartID);
    var dtTaskDue = document.getElementById(dtTaskDueID);
    var dtTaskEnd = document.getElementById(dtTaskEndID);

    if (dtTaskStart.value !== '')
    {
        var startDate = getDate(dtTaskStart.value);

        if (dtTaskDue.value !== '')
        {
            var dueDate = getDate(dtTaskDue.value);

            if (startDate > dueDate)
            {
                args.IsValid = false;
                return;
            }
        }

        if (dtTaskEnd.value !== '')
        {
            var endDate = getDate(dtTaskEnd.value);

            if (startDate > endDate)
            {
                args.IsValid = false;
                return;
            }
        }
    }
}

function validateTaskDueDate(sender, args)
{
    var dtTaskStart = document.getElementById(dtTaskStartID);
    var dtTaskDue = document.getElementById(dtTaskDueID);
    var dtTaskEnd = document.getElementById(dtTaskEndID);

    if (dtTaskDue.value !== '')
    {
        var dueDate = getDate(dtTaskDue.value);

        if (dtTaskStart.value !== '')
        {
            var startDate = getDate(dtTaskStart.value);

            if (dueDate < startDate)
            {
                return args.IsValid = false;
            }
        }

        if (dtTaskEnd.value !== '')
        {
            var endDate = getDate(dtTaskEnd.value);

            if (dueDate > endDate)
            {
                return args.IsValid = false;
            }
        }
    }
}

function validateTaskEndDate(sender, args)
{
    var dtTaskStart = document.getElementById(dtTaskStartID);
    var dtTaskDue = document.getElementById(dtTaskDueID);
    var dtTaskEnd = document.getElementById(dtTaskEndID);

    if (dtTaskEnd.value !== '')
    {
        var endDate = getDate(dtTaskEnd.value);

        if (dtTaskStart.value !== '')
        {
            var startDate = getDate(dtTaskStart.value);

            if (endDate < startDate)
            {
                return args.IsValid = false;
            }
        }

        if (dtTaskDue.value !== '')
        {
            var dueDate = getDate(dtTaskDue.value);

            if (endDate < dueDate)
            {
                return args.IsValid = false;
            }
        }
    }
}

function getDate(strDate)
{
    var day = parseInt(strDate.substring(0, 2), 10);
    var month = parseInt(strDate.substring(3, 5), 10);
    var year = parseInt(strDate.substring(6, 10), 10);
    var date = new Date(year, month, day);

    return date;
}