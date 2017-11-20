Create PROCEDURE [dbo].[GetAuthoriserLevelIdByEmployee] 
@EmployeeId int
As
BEGIN
SELECT AuthoriserLevelDetailId from employees where employeeid=@EmployeeId
END