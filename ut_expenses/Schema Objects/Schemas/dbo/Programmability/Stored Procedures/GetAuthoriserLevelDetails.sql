Create PROCEDURE [dbo].[GetAuthoriserLevelDetails] 
@AuthoriserLevelDetailId int
As
BEGIN
SELECT AuthoriserLevelDetailId,Amount,Description FROM AuthoriserLevelDetails where AuthoriserLevelDetailId=@AuthoriserLevelDetailId
END