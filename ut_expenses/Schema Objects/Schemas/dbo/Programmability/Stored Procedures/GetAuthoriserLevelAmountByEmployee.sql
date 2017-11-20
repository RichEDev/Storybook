Create PROCEDURE [dbo].[GetAuthoriserLevelAmountByEmployee] 
@EmployeeId int
As
BEGIN
SELECT Amount from AuthoriserLevelDetails where AuthoriserLevelDetailId=(select AuthoriserLevelDetailId from employees where EmployeeId=@EmployeeId)
END
