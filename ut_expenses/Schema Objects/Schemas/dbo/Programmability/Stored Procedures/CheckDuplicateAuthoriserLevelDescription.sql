Create PROCEDURE [dbo].[CheckDuplicateAuthoriserLevelDescription] 
@AuthoriserLevelDetailId int,
@Description nvarchar(max)
As
BEGIN
SELECT Description FROM AuthoriserLevelDetails where AuthoriserLevelDetailId<>@AuthoriserLevelDetailId and Description=@Description
END