Create PROCEDURE [dbo].[GetAllAuthoriserLevelDetails] 
As
BEGIN
SELECT AuthoriserLevelDetailId,Amount,Description FROM AuthoriserLevelDetails where Amount>=0 ORDER BY Amount
END