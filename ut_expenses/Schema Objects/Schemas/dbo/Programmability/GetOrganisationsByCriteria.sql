CREATE PROCEDURE [dbo].[GetOrganisationsByCriteria] @OrganisationName VARCHAR(25) = NULL
	,@Comment VARCHAR(25) = NULL
	,@Code VARCHAR(25) = NULL
	,@Line1 VARCHAR(25) = NULL
	,@City VARCHAR(25) = NULL
	,@PostCode VARCHAR(25) = NULL
	,@Archived BIT = false
AS
BEGIN
	SELECT   [OrganisationID]
			,[OrganisationName]
			,[ParentOrganisationID]
			,[Comment]
			,[Code]
			,[IsArchived]
			,[CreatedOn]
			,[CreatedBy]
			,[ModifiedOn]
			,[ModifiedBy]
			,[PrimaryAddressID]
			,[Line1]
			,[City]
			,[Postcode]
			,[AddressName]
	FROM vwOrganisation
	WHERE (
			@OrganisationName IS NULL
			OR OrganisationName LIKE @OrganisationName + '%'
			)
		AND (
			@Comment IS NULL
			OR Comment LIKE @Comment + '%'
			)
		AND (
			@Code IS NULL
			OR Code LIKE @Code + '%'
			)
		AND (
			@Line1 IS NULL
			OR AddressName LIKE @Line1 + '%'
			)
		AND (
			@City IS NULL
			OR City LIKE @City + '%'
			)
		AND (
			@PostCode IS NULL
			OR PostCode LIKE @PostCode + '%'
			)
		AND (
			@Archived IS NULL
			OR IsArchived = @Archived
			)
END