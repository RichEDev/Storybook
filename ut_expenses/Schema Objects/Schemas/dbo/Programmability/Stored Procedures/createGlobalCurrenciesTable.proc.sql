-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[createGlobalCurrenciesTable] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if exists(Select * From tempdb.dbo.sysobjects Where name = '#global_currencies')
    begin
	drop table #global_currencies
	end
	create table #global_currencies (globalcurrencyid int, label nvarchar(500), alphacode nvarchar(50), numericcode nvarchar(50), symbol nvarchar(50));
END
