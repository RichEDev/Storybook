CREATE PROCEDURE [dbo].[UpdateFilterRule] 

@filterId INT,
@enabled BIT

AS

UPDATE filter_rules SET enabled = @enabled WHERE filterid = @filterId