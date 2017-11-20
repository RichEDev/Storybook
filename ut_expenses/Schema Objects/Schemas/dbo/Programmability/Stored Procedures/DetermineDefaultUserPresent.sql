Create PROCEDURE [dbo].[DetermineDefaultUserPresent]
AS
BEGIN
 declare @AuthoriserLevelDetailId int 
 if EXISTS (select employeeid from employees where AuthoriserLevelDetailId is not null) 
 begin
 set @AuthoriserLevelDetailId=(select AuthoriserLevelDetailId from AuthoriserLevelDetails where Amount=-1)
  if EXISTS ( select employeeid from employees where AuthoriserLevelDetailId =@AuthoriserLevelDetailId )
  begin
   select  CAST(1 AS BIT)
  end
  else
  begin
  select  CAST(0 AS BIT)
  end
 end
 else
 begin
 select  CAST(1 AS BIT)
 end
END
