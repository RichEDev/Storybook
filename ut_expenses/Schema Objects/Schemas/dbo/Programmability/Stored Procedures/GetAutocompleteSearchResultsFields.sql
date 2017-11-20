
CREATE PROCEDURE [dbo].[GetAutocompleteSearchResultsFields]
@attributeId int
as
select DISTINCT fieldid from [CustomEntityAttributeLookupSearchResultFields] where attributeId = @attributeId