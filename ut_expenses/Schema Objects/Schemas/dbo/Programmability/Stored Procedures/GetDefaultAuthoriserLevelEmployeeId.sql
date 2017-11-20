Create PROCEDURE [dbo].[GetDefaultAuthoriserLevelEmployeeId] 
As
BEGIN

SELECT employeeid from employees where AuthoriserLevelDetailId=(select AuthoriserLevelDetailId from AuthoriserLevelDetails where Amount=-1)
END