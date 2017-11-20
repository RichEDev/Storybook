Create PROCEDURE [dbo].[UpdateEmployeeForDefaultAuthoriser] 
@EmployeeId int
As
BEGIN
Declare @AuthoriserLevelId int
select @AuthoriserLevelId= AuthoriserLevelDetailId from AuthoriserLevelDetails where Amount<0 
update employees set AuthoriserLevelDetailId=null where AuthoriserLevelDetailId= @AuthoriserLevelId
update employees set AuthoriserLevelDetailId= @AuthoriserLevelId where employeeid=@EmployeeId
return @@RowCount
END