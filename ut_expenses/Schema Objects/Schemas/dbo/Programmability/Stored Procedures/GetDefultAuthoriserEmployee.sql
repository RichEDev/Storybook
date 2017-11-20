Create PROCEDURE [dbo].[GetDefultAuthoriserEmployee] 
As
BEGIN
Declare @AuthoriserLevelId int
select @AuthoriserLevelId= AuthoriserLevelDetailId from AuthoriserLevelDetails where Amount<0 
select  employeeid, firstname + ' ' + surname + ' (' + username + ')' AS EmployeeName from employees e where AuthoriserLevelDetailId= @AuthoriserLevelId
END
