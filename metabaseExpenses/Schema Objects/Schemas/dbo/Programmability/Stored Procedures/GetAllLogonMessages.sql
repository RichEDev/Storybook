CREATE PROCEDURE GetAllLogonMessages 
AS
BEGIN
SELECT [MessageId]
      ,[CategoryTitle]
      ,[CategoryTitleColourCode]
      ,[HeaderText]
      ,[HeaderTextColourCode]
      ,[BodyText]
      ,[BodyTextColourCode]
      ,[BackgroundImage]
      ,[Icon]
      ,[ButtonText]
      ,[ButtonLink]
      ,[ButtonForeColour]
      ,[ButtonBackGroundColour]
      ,[Archived]
      ,[CreatedBy]
      ,[CreatedOn]
      ,[ModifiedBy]
      ,[ModifiedOn]
  FROM [dbo].[LogonMessages]
  END