function validateItem(id, valtype, item) {
    var error;
    var valchars;
    //var txtbox = eval("Form1." + id);
    var txtbox = document.getElementById(id);
    if (document.getElementById(id) == null) {
        return true;
    }
    var valid = true;

    switch (valtype) {
        case 1: //number
            valchars = "Valid characters are the numbers 0-9 and a full stop (.)";
            if (checkNum(txtbox.value) == false) {
                valid = false;
            }
            break;
        case 2: //int
            valchars = "Valid characters are the numbers 0-9";
            if (checkInt(txtbox.value) == false) {
                valid = false;
            }
            break;
        case 3: //time
            valchars = "Valid characters are the numbers 0-9 and a colon (:)";
            if (checkTime(txtbox.value) == false) {
                valid = false;
            }
            break;
        case 4: //date
            valchars = "Valid characters are the numbers 0-9 and a forward slash (/)";

            if (checkDate(txtbox.value) == false) {
                valid = false;
            }
    }


    if (valid == false) {
        error = "Please enter a valid value for " + item
        error += "\n" + valchars;
        alert(error);

        txtbox.focus();
        return false
    }

    return true;

}

function checkNum(data) {
    var numbers = "-0123456789.";
    var i = count = 0;
    var dec = ".";
    var space = " ";

    for (i = 0; i < data.length; i++)
        if (data.substring(i, i + 1) == space)
        return false;

    for (i = 0; i < data.length; i++)
        if (numbers.indexOf(data.substring(i, i + 1)) == "-1")
        return false;

    for (i = 0; i < data.length; i++)
        if (data.substring(i, i + 1) == dec) count++;
    if (count > 1) return false;

    return true;
}

function checkInt(data) {
    var numbers = "0123456789";
    var i = count = 0;
    var dec = ".";
    var space = " ";

    for (i = 0; i < data.length; i++)
        if (data.substring(i, i + 1) == space)
        return false;

    for (i = 0; i < data.length; i++)
        if (numbers.indexOf(data.substring(i, i + 1)) == "-1")
        return false;

    for (i = 0; i < data.length; i++)
        if (data.substring(i, i + 1) == dec) count++;
    if (count > 1) return false;

    return true;
}

function checkDate(data) {

    var sData = new String(data);
    //sData = data;

    //dd/mm/yyyy
    var a = new Date();

    var month;
    var day;
    var year;
    var daylen = new Number();
    var isLeapyear = new Boolean(false);

    if (sData.length == 0) {
        return true;
    }
    if (sData.length != 10) {
        return false;
    }

    if (sData.substr(2, 1) != "/" || sData.substr(5, 1) != "/") {
        return false;
    }
    if (checkInt(sData.substr(0, 2)) == false) {
        return false;
    }

    if (checkInt(sData.substr(3, 2)) == false) {
        return false;
    }

    if (checkInt(sData.substr(6, 4)) == false) {
        return false;
    }

    month = new Number(sData.substr(3, 2));
    if (month < 1 || month > 12) {
        return false;
    }
    day = new Number(sData.substr(0, 2));
    var year = new Number(sData.substr(6, 4));

    if (year < 1990) {
        return false;
    }
    switch (month.toString(10)) {
        case "1":
        case "3":
        case "5":
        case "7":
        case "8":
        case "10":
        case "12":
            daylen = 31;
            break;
        case "2":


            if ((year % 4) == 0 || (year % 400) == 0) {
                isLeapyear = true;
            }
            else {
                isLeapyear = false;
            }
            if (isLeapyear == false) {
                daylen = 28;
            }
            else {
                daylen = 29;
            }
            break;

        case "4":
        case "6":
        case "9":
        case "11":
            daylen = 30;
            break;
        default:
            daylen = -1;



    }

    if (day < 1 || day > daylen) {
        return false;
    }

    return true;
}

function checkTime(data) {

    var sData = new String(data);
    var minutes;
    var hours;
    //hh:mm

    if (sData.length != 5) {
        return false;
    }

    if (sData.substr(2, 1) != ":") {
        return false;
    }

    if (checkInt(sData.substr(0, 2)) == false) {
        return false;
    }

    if (checkInt(sData.substr(3, 2)) == false) {
        return false;
    }


    hours = new Number(sData.substr(0, 2));
    if (hours < 0 || hours > 23) {
        return false;
    }

    minutes = new Number(sData.substr(3, 2));
    if (minutes < 0 || minutes > 59) {
        return false;
    }
    return true;
}