CREATE PROCEDURE [dbo].[getOrganisations]
	@OrganisationID int = null
AS
	SELECT
		organisations.OrganisationID,
		organisations.OrganisationName,
		organisations.ParentOrganisationID,
		organisations.Comment,
		organisations.Code,
		organisations.IsArchived,
		organisations.CreatedOn,
		organisations.CreatedBy,
		organisations.ModifiedOn,
		organisations.ModifiedBy,
		organisations.PrimaryAddressID,
		addresses.AddressName,
		addresses.Line1,
		addresses.City,
		addresses.Postcode
	FROM
		dbo.organisations
			LEFT JOIN dbo.addresses on organisations.PrimaryAddressID = addresses.AddressID
	WHERE
		organisations.OrganisationID = @OrganisationID
		or @OrganisationID is null
		or @OrganisationID = 0
	ORDER BY
		organisations.OrganisationName

RETURN 0
