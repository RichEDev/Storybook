
Create PROCEDURE [dbo].[GetAuthoriserLimitByAuthoriserLevelId] 
@AuthoriserLevelDetailId int
As
BEGIN
select amount from AuthoriserLevelDetails where AuthoriserLevelDetailId=@AuthoriserLevelDetailId
END