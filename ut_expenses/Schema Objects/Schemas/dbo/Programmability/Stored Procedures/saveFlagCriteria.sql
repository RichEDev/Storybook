-- Stored Procedure

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE saveFlagCriteria 
	@flagId int,
	@condition tinyint,
	@value1 nvarchar(1000),
	@value2 nvarchar(1000),
	@andor tinyint, 
	@order int, 
	@fieldid uniqueidentifier,
	@groupnumber tinyint,
	@joinviaid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    insert into flagCustomCriteria (flagid, condition, value1, value2, andor, [order], fieldid, groupnumber, joinViaID) values (@flagId, @condition, @value1, @value2, @andor, @order, @fieldid, @groupnumber, @joinviaid)
END