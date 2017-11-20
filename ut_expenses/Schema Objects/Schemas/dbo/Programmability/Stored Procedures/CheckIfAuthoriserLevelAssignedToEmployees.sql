Create PROCEDURE [dbo].[CheckIfAuthoriserLevelAssignedToEmployees]
@AuthoriserLevelDetailId int
As
BEGIN
select employeeid from employees where AuthoriserLevelDetailId=@AuthoriserLevelDetailId
END