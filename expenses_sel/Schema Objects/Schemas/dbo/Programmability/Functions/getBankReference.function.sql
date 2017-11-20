
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[getBankReference] ()
RETURNS nvarchar(50)
AS
BEGIN
	declare @bankreference nvarchar(50)
	set @bankreference=(select bankreference from company_bankdetails)
	return @bankreference
END

