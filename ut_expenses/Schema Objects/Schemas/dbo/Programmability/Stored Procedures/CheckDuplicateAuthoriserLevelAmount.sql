Create PROCEDURE [dbo].[CheckDuplicateAuthoriserLevelAmount] 
@AuthoriserLevelDetailId int,
@Amount [decimal](18, 2)
As
BEGIN
SELECT Amount FROM AuthoriserLevelDetails where AuthoriserLevelDetailId<>@AuthoriserLevelDetailId and Amount=@Amount
END