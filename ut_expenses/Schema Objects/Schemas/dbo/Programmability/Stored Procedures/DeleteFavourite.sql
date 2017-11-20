CREATE PROCEDURE [dbo].[DeleteFavourite]
	@FavouriteID int = NULL,
	@UserID int,
	@DelegateID int
AS
	DELETE FROM [dbo].[Favourites]
	WHERE [FavouriteID] = @FavouriteID
		
	IF @userid > 0
	BEGIN
		exec addDeleteEntryToAuditLog @UserID, @DelegateID, 38, @FavouriteID, 'Deleted', null;
	END

	RETURN 1;

GO