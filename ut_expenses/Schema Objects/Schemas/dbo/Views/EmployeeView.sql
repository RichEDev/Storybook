CREATE VIEW [dbo].[EmployeeView]
AS
SELECT        employeeid, firstname + ' ' + surname + ' (' + username + ')' AS EmployeeName
FROM            dbo.employees

GO