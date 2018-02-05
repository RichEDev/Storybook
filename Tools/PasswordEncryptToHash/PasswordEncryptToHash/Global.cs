namespace PasswordEncryptToHash
{
    public static class Global
    {
        public static string EmployeeUpdate =
            "UPDATE emp  SET emp.password = logitem, emp.passwordMethod = 5 FROM employees as emp inner join @logitem as newPassword on newPassword.reasonID = emp.employeeid;";
        public static string PreviousPasswordUpdate =
            "UPDATE emp  SET emp.password = logitem, emp.passwordMethod = 5 FROM previouspasswords as emp inner join @logitem as newPassword on newPassword.reasonID = emp.employeeid AND newPassword.elementID = emp.[order];";
    }
}
