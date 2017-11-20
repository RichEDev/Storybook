CREATE VIEW [dbo].[vwOrganisation]
AS
SELECT     TOP (100) PERCENT dbo.organisations.OrganisationID, dbo.organisations.OrganisationName, dbo.organisations.ParentOrganisationID, dbo.organisations.Comment, dbo.organisations.Code, 
                      dbo.organisations.IsArchived, dbo.organisations.CreatedOn, dbo.organisations.CreatedBy, dbo.organisations.ModifiedOn, dbo.organisations.ModifiedBy, dbo.organisations.PrimaryAddressID, 
                      dbo.addresses.Line1, dbo.addresses.City, dbo.addresses.Postcode, dbo.addresses.AddressName
FROM         dbo.organisations LEFT OUTER JOIN
                      dbo.addresses ON dbo.organisations.PrimaryAddressID = dbo.addresses.AddressID
ORDER BY dbo.organisations.OrganisationName