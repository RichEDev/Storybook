<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalculationField.ascx.cs" Inherits="Spend_Management.shared.usercontrols.CalculationField" %>
<%@ Register Src="~/shared/usercontrols/treeView.ascx" TagName="treeView" TagPrefix="treeView" %>
<script language="javascript" type="text/javascript">

    function insertAtCaret(obj, string) {
        obj.focus();

        if (typeof (document.selection) != 'undefined') {
            var range = document.selection.createRange();

            if (range.parentElement() != obj)
                return;

            range.text = string;
            range.select();
        }
        else if (typeof (obj.selectionStart) != 'undefined') {
            var start = obj.selectionStart;

            obj.value = obj.value.substr(0, start) + string + obj.value.substr(obj.selectionEnd, obj.value.length);
            start += string.length;
            obj.setSelectionRange(start, start);
        }
        else
            obj.value += string;

        obj.focus();
    }



    function AddField(FieldID, Description, tableID, dataType) {
        var txtFormulaObject = document.getElementById('<% = txtFormula.ClientID %>');
        insertAtCaret(txtFormulaObject, "[" + Description + "]");
    }
</script>
<asp:Panel ID="pnlCalculatedField" runat="server" CssClass="formpanel" style="position: relative;">
    <div class="sectiontitle">Calculated Field</div>
    <asp:Panel ID="pnlTreeView" runat="server" style="position: absolute; top: 25; left: 0;">
            <div style="height: 200px; overflow: auto;">
                <treeView:treeView ID="TreeView" runat="server" OnClickMethod="AddField" />
            </div>
            <div style="height: 400px;">
                    
            </div>
    </asp:Panel>
    <asp:Panel ID="pnlRightSide" runat="server" style="position: absolute; right: 0; top: 25; width: 460px;">
    <asp:TextBox ID="txtFormula" runat="server" TextMode="MultiLine" Rows="8" style="width: 440px"></asp:TextBox>
    
    </asp:Panel>
</asp:Panel>

<script language="javascript" type="text/javascript">
var FunctionTypes = new Array();
var FunctionType;


/*
* #############
* Math
* #############
*/
FunctionType = new Array();
FunctionTypes.push(FunctionType);
FunctionType.push("ABS", "Returns the absolute value of a number. The absolute value is the number without it&#34;s sign.", "The number is the real number of which you want the absolute value.", "ABS(2) - Absolute value of 2 (2).", "[number]", "Math");
FunctionType.push("CEILING", "Returns number rounded, away from zero, to the nearest multiple of significance.", "", "CEILING(2.5,1) - Rounds 2.5 up to the nearest multiple of 1 (3).", "[number], [multiple]", "Math");
FunctionType.push("FLOOR", "Rounds the number down, towards zero, to the nearest multiple of significance.", "", "FLOOR(1.5, 1) - Rounds 2.5 down to the nearest multiple of 1 (2)", "[number], [multiple]", "Math");
FunctionType.push("ODD", "Returns number rounded up to nearest odd integer.", "If number is already odd, no rounding occurs.", "ODD(1.5) - Rounds 1.5 up to the nearest odd integer (3).", "[number]", "Math");
FunctionType.push("EVEN", "Returns number rounded up to nearest even integer.", "If number is already even, no rounding occurs.", "EVEN(1.5) - Rounds 1.5 up to the nearest even integer (2).", "[number]", "Math");
FunctionType.push("INT", "Returns number rounded down to the nearest integer.", "The number is the real number you want to round down to an integer.", "INT(8.9) - Rounds 8.9 down (8).", "[number]", "Math");
FunctionType.push("MOD", "Returns the the remainder after number is divided by divisor.", "Number is the number for which you want to find the remainder. Divisor is the number by which you want to divide number.", "MOD(3,2) - Remainder of 3/2 (1).", "[number], [divisor]", "Math");
FunctionType.push("POWER", "Returns the result of a number raised to a power.", "Number is the base number, it can be any real number. Power is the exponent to which the base number is to be raised. Equivalent of &#34;^&#34;.", "POWER(5,2) - 5 squared (25).", "[number], [power]", "Math");
FunctionType.push("PRODUCT", "Multiples all the numbers given as arguments and returns the product.", "Up to 30 arguments may be entered.", "PRODUCT(5,30) - Multiplies the numbers (150).", "[arguments]", "Math");
FunctionType.push("ROUND", "Rounds a number to a specified number of digits.", "Number is the number you wish to round. Num_digits specifies the number of digits you wish to round Number to.", "ROUND(2.15, 1) - Rounds 2.15 to one decimal place (2.2).", "[number], [num_digits]", "Math");
FunctionType.push("SQRT", "Returns a positive square root.", "Number is the number for which you want the square root. Number cannot be negative.", "ROUND(2.15, 1) - Rounds 2.15 to one decimal place (2.2).", "[number]", "Math");
FunctionType.push("SUM", "Adds all the numbers passed in the arguments.", "Numbers, logical values and text representations of numbers are counted.", "SUM(3,2) - Adds 3 and 2 (5).", "[arguments]", "Math");
FunctionType.push("TRUNC", "Truncates a number to an integer by removing the fractional part of the number.", "Number is the number you want to truncate. Num_digits is a number specifying the precision of the truncation. Default value of Num_digits is zero.", "TRUNC(8.9) - Integer part of 8.9 (8)", "[number], {Num_digits}", "Math");
/*
* #############
* Date & Time
* #############
*/
FunctionType = new Array();
FunctionTypes.push(FunctionType);
FunctionType.push("DATE", "Returns the number of ticks representing a particular date in the server&#34;s local timezone. This function should be used to represent a date to other functions.", "The Year argument can be one to 4 digits between 1 and 9999. Month is a number representing the month between 1 and 12. Day is a number representing the day of the month between 1 and the number of days in the month specified.", "YEAR(DATE(2008,7,4)) - Returns the Gregorian calendar year of the day July 4th 2008, produced by this function (2008).", "[year], [month], [day]", "Date & Time");
FunctionType.push("DATEVALUE", "Returns the number of ticks of the date represented in text. Use DATEVALUE to convert a date represented in text into ticks that can be passed to other functions.", "Date_text is the text representing a given date. For example &#34;30-Jan-2006&#34; is a quoted text string representing a date. The day can only appear before the month in Date_text if the month is in text (i.e, &#34;Jan&#34; in the first format) to avoid ambiguity that can arise in the second format.", "MONTH(DATEVALUE(&#34;15-Apr-2008&#34;)) - Returns the month number of April 15th 2008 (4).", "[Date_text]", "Date & Time");
FunctionType.push("TIME", "Returns a time value in ticks for a particular time based on it&#34;s number of hours, minutes and seconds. Use TIME to create durations and time periods to be added to other date/time values also represented in ticks.", "Hour is a number from 0 (zero) to 23 representing the hour, 24-Hour clock rules apply. Minute is a number from 0 to 59 representing minutes past the hour. Second is a number from 0 to 59 representing the number of seconds past the minute.", "SECOND(TIME(14,50,5)) - Returns the seconds portion of the time value (5).", "[Hour], [Minute], [Second]","Date & Time");
FunctionType.push("TIMEVALUE", "Returns a number of ticks representing the time portion of a text string representing the time. Time values can serve as a duration that can be added to other time or data/time values also represented in ticks.", "Time_text is a text string within quotation marks that represents a time. Examples of acceptable formats include &#34;5:55PM&#34; and &#34;17:55&#34;. If AM/PM is not specified, time is assumed to be AM. Specification of seconds is optional. A time seperator is madatory (&#34;17:00&#34; is acceptable, &#34;1700&#34; is not). Do not seperate AM or PM with periods (A.M or P.M), this will return an error.", "HOUR(TIMEVALUE(&#34;17:00&#34;)) - Returns the hours portion of the time&#34;s text representation (17).", "[Time_text]", "Date & Time");
FunctionType.push("DAYS360", "Returns the number of days between two specified dates based on a 360-day year (twelve 30-day months) used in some accounting applications. Use this function when computing payments if your accounting system is based on twelve 30-day months.", "Start_date and End_date are the two dates between which you want to know the number of days (based on a 360-day year). If Start_date occurs after End_date a negative will be returned. Dates should be generated by functions representing date/time values as ticks. Method is an optional argument indicating whether to use the European method of computation instead of the U.S. NASD method, each produce slightly different answers when Start_date or End_date occurs on the 31st day of a month. The default method is U.S. NASD (FALSE) when omitted.", "DAYS360(DATE(2005,1,1),DATE(2005,1,31),&#34;TRUE&#34;) - Calculates the number of days on an accounting calendar between January 1st 2005 and January 31st 2005 using the European method which treats January 31st as January 30th (29). Using the U.S. NASD method would produce a different answer (30).", "[Start_date], [End_date], {Method}", "Date & Time");
FunctionType.push("YEAR", "Returns the year portion of a date as a whole number randing from 1 to 9999.", "Ticks is the number of ticks representing a date/time value.", "YEAR(DATE(2007,1,31)) - - Returns the year portion of the date (2007).", "[Ticks]", "Date & Time");
FunctionType.push("MONTH", "Returns the month portion of a date as a whole number randing from 1 to 12.", "Ticks is the number of ticks representing a date/time value.", "MONTH(DATE(2007,1,31)) - - Returns the month portion of the date (1).", "[Ticks]", "Date & Time");
FunctionType.push("DAY", "Returns the day portion of a date as a whole number.", "Ticks is the number of ticks representing a date/time value.", "DAY(DATE(2007,1,31)) - - Returns the day portion of the date (31).", "[Ticks]", "Date & Time");
FunctionType.push("HOUR", "Returns the hour portion of a time as a whole number.", "Time is the time from which to obtain the hour.", "HOUR(&#34;14:50:05&#34;) - Returns the hour portion of the time (14).", "[Time]", "Date & Time");
FunctionType.push("MINUTE", "Returns the minute portion of a time as a whole number.", "Time is the time from which to obtain the minute.", "MINUTE(&#34;14:50:05&#34;) - Returns the minute portion of the time (50).", "[Time]", "Date & Time");
FunctionType.push("SECOND", "Returns the second portion of a time as a whole number.", "Time is the time from which to obtain the second.", "SECOND(&#34;14:50:05&#34;) - Returns the second portion of the time (5).", "[Time]", "Date & Time");
FunctionType.push("TODAY", "Returns the ticks representing today&#34;s date, in server local time. The returned date will have no time component.", "", "MONTH(TODAY()) - Returns the current month (" + DateTime.Now.Month.ToString() + ").", "", "Date & Time");
FunctionType.push("NOW", "Returns the ticks representing the date and time now, in server local time.", "", "HOUR(NOW()) - Returns the current hour (" + DateTime.Now.Hour.ToString() + ").", "", "Date & Time");

/*
* #############
*Logical
* #############
*/
FunctionType = new Array();
FunctionTypes.push(FunctionType);
FunctionType.push("AND", "Returns TRUE if all its argument are TRUE; returns FALSE if one or more argument is false.", "The arguments must evaluate to logical values such as TRUE or FALSE.", "AND(2+2=4,2+3=5) - All arguments are TRUE (TRUE).", "[argument 1], [argument 2]", "Logical");
FunctionType.push("OR", "Returns TRUE if any of its argument are TRUE; returns FALSE if all of its arguments are false.", "The arguments must evaluate to logical values such as TRUE or FALSE.", "OR(2+2=5,2+3=8) - All arguments are FALSE (FALSE).", "[argument 1], [argument 2]", "Logical");
FunctionType.push("TRUE", "Returns the logical value TRUE.", "You can also type the logical value TRUE in an expression; it will automatically be interpreted as a logical TRUE.", "N/A", "", "Logical");
FunctionType.push("FALSE", "Returns the logical value FALSE.", "You can also type the logical value FALSE in an expression; it will automatically be interpreted as a logical FALSE.", "N/A", "", "Logical");
FunctionType.push("IF", "Returns one value if a condition you specify evaluates to TRUE and another value if it evaluates to FALSE.", "Logical_test is any value or expression that can be evaluated to TRUE or FALSE.", "IF(100>50, &#34;Over Budget&#34;, &#34;OK&#34;) - Checks whether 100 is above 50, and returns specified text (Over Budget).", "[Logical_test], [When_true], [When_false]", "Logical");
FunctionType.push("NOT", "Reverses the value of its argument. Use NOT when you want to make sure a value is not equal to one particular value.", "[Logic_value] is a value or expression that can be evaluated to TRUE or FALSE. If [Logic_value] is TRUE, NOT returns false; If [Logic_value] is FALSE, NOT returns TRUE.", "NOT(TRUE) - Reverses TRUE (FALSE).", "[Logic_value]", "Logical");
/*
* #############
* Statistical
* #############
*/
FunctionType = new Array();
FunctionTypes.push(FunctionType);
FunctionType.push("AVG", "Returns the average (arithmetic mean avereage) of the arguments, Number1, Number2, ... are 1 to 30 numeric arguments for which you want the average..", "Arguments that can not be evaluated to a numeric value are ignored.", "AVG(1337, 12, 99, 881, 300) - Returns the average of the numbers &#34;1337, 12, 99, 881, 300&#34; (525.8).", "[arguments]", "Statistical");
FunctionType.push("MEDIAN", "Returns the middle (arithmetic median average) of the given numbers. The median is the middle of a set of numbers; that is, half the numbers have values that are greater then the median , and half have values that are less.", "Arguments that can not be evaluated to a numeric value are ignored.", "MEDIAN(1,2,3,4,5,6) - Median of the given numbers in the list (3.5).", "[arguments]", "Statistical");
FunctionType.push("MIN", "Returns the smallest number in a set of values", "If the arguments contain no numbers MIN returns 0 (zero).", "MIN(10,2) - Smallest of the numbers (2)", "[arguments]", "Statistical");
FunctionType.push("MAX", "Returns the largest number in a set of values", "If the arguments contain no numbers MAX returns 0 (zero).", "MAX(27,2) - Largest of the numbers (27)", "[arguments]", "Statistical");
FunctionType.push("STDEV", "Estimates standard deviation based on a sample.", "STDEV assumes that its arguments are a sample of the population. The standard deviation is calculated using the the &#34;unbiased&#34; or &#34;n-1&#34; method. Logical values such as TRUE or FALSE and text are ignored.", "STDEV(1345, 1301, 1368, 1322, 1370, 1318, 1350, 1303, 1299) - Standard deviation (27.46391572)", "[arguments]", "Statistical");
/*
* #############
* Text & Data
* #############
*/
FunctionType = new Array();
FunctionTypes.push(FunctionType);
FunctionType.push("ADDRESS", "Get the cell value from the row and column references. If the cell value you are referencing is a text value you will need to enclose the ADDRESS function in single quotes ", "Row Reference is the referenced row number. Column Reference is the referenced column number", "ADDRESS(1, 3)", "[Row Reference],[Column Reference]", "Text & Data");
FunctionType.push("CHAR", "Returns the character specified by a number. Use CHAR to translate code page numbers you might get from files on other types of computers into characters.", "Number is the number between 1 and 255 specifying which character you want. The character is from the character set used by the server.", "CHAR(65) - Displays the 65th character in the set (A).", "[number]", "Text & Data");
FunctionType.push("CODE", "Returns a numeric code for the first character in a text string. The returned code corresponds to the character set used by your computer.", "Text is the text for which you want the code of the first character.", "CODE(&#34;A&#34;) - Displays the numeric code for A (65).", "[Text]", "Text & Data");
FunctionType.push("CONCATENATE", "Joins several text strings into one text string.", "The &#34;&&#34; operator can be used instead of CONCATENATE to join text items.", "CONCATENATE(&#34;Joe&#34;, &#34; &#34;, &#34;went&#34;, &#34; &#34;, &#34;to the shop&#34;) - This concatenates a sentence from the data above (Joe went to the shop).", "[arguments]", "Text & Data"); 
FunctionType.push("COLUMN", "Gets the current column number.", "", "COLUMN()", "", "Text & Data");
FunctionType.push("FIND", "Finds one text string (find_text) within another text string (within_text) and returns the starting position of find_text within within_text.", "find_text is the text you wish to find. within_text is the text containing the string you wish to find.", "FIND(&#34;M&#34;, &#34;Mad Max&#34;) - Position of the first M in the string (1).", "[find_text], [within_text], {start_pos}", "Text & Data");
FunctionType.push("LEN", "Returns the number of characters in a text string.", "Text is the text whose length you want to find. Spaces count as character.", "LEN(&#34;One&#34; - Returns the length of the given string (3).", "[Text]", "Text & Data");
FunctionType.push("LOWER", "Converts all uppercase letters in a text string to lowercase.", "Text is the text you want to convert to lowercase. LOWER does not change characters in text that are not letters.", "LOWER(&#34;ChEese&#34; - Returns the lowercase representation of the string (cheese).", "[Text]", "Text & Data");
FunctionType.push("UPPER", "Converts all lowercase letters in a text string to uppercase.", "Text is the text you want to convert to uppercase. UPPER does not change characters in text that are not letters.", "UPPER(&#34;ChEese&#34; - Returns the uppercase representation of the string (CHEESE).", "[Text]", "Text & Data");
FunctionType.push("MID", "Returns a specific number of characters from a text string, starting at the position you specify, based on the number of characters you specify.", "Text is the text string containing the characters you want to extract. Start_num is the position of the first character you want to extract in text. Num_chars specifies the number of characters you want MID to return from Text.", "MID(&#34;Fluid Flow&#34;,1,5) - Five characters from the string starting at the first character (Fluid).", "[Text], [Start_num], {Num_chars}", "Text & Data");
FunctionType.push("RIGHT", "Returns the last character or characters in a text string, based on the number of characters you specify.", "Text is the string containing the characters you want to extract. Num_chars specifies the numbers of characters you wish to extract.", "RIGHT(&#34;The Price&#34;,5) - Returns the last 5 characters in the string (Price).", "[Text], [Num_chars]", "Text & Data");
FunctionType.push("LEFT", "Returns the first character or characters in a text string, based on the number of characters you specify.", "Text is the string containing the characters you want to extract. Num_chars specifies the numbers of characters you wish to extract.", "LEFT(&#34;The Price&#34;,3) - Returns the first 3 characters in the string (The).", "[Text], [Num_chars]", "Text & Data");
FunctionType.push("ROW", "Gets the current row number.", "", "ROW()", "", "Text & Data");
FunctionType.push("TEXT", "Converts a value to text in a specific number format.", "Text  is a numeric value, Format_text is a numeric format as a text string enclosed in quotation marks.", "TEXT(&#34;£1,000&#34;) - Number equivalent of text string (1000).", "[Value],[Format Text]", "Text & Data");
FunctionType.push("TRIM", "Removes all spaces from text except for single spaces between words. Use TRIM on text that may have irregular spacing.", "Text is the text from which you want spaces removed.", "TRIM(&#34; First Quarter Earnings &#34;) - Removes leading and trailing spaces (First Quarter Earnings).", "[Text]", "Text & Data");
FunctionType.push("REPLACE", "Replaces part of a text string, based on the number of characters you specify with a different text string.", "Old_text is the text in which you want to replace some old characters. Start_num is the position of the character in Old_text that you want to replace with New_text. Num_chars is the number of old characters that you want to REPLACE with New_text", "REPLACE(&#34;abcdefghijk&#34;,6,5,&#34;*&#34;) - Replaces 5 characters starting with the 6th character (abcde*k).", "[Old_text], [Start_num], [Num_chars], [New_text]", "Text & Data");
</script>