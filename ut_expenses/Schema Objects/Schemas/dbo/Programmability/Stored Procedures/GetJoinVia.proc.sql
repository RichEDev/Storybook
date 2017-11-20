CREATE PROCEDURE [dbo].[GetJoinVia]
AS
	SELECT [joinVia].[joinViaID], [joinVia].[joinViaDescription], [joinVia].[joinViaAS], [relatedID], [relatedType], [order] FROM [dbo].[joinViaParts]
	INNER JOIN [dbo].[joinVia] ON [joinVia].[joinViaID] = [joinViaParts].[joinViaID]