Create PROCEDURE [dbo].[GetLineManagerOfAnEmployee] 
@EmployeeId int
As
BEGIN
SELECT linemanager from employees where employeeid=@EmployeeId 
END
