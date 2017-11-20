
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[addFinancialExport] (@applicationtype tinyint, @reportid uniqueidentifier, @automated bit, @createdby int, @exporttype tinyint, @NHSTrustID int, @employeeID INT, @delegateID INT,@expeditePaymentReport BIT)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @entityID int;
	declare @recordtitle nvarchar(2000);
	declare @reptName nvarchar(1000);
	select @reptName = reportname from reports where reportid = @reportid;
	set @recordtitle = (select 'Financial Export ' + @reptName);

    -- Insert statements for procedure here
	insert into financial_exports (applicationtype, reportid, automated, createdby, exporttype, NHSTrustID, ExpeditePaymentReport) values (@applicationtype, @reportid, @automated, @createdby, @exporttype, @NHSTrustID, @expeditePaymentReport);
	set @entityID = scope_identity();

	exec addInsertEntryToAuditLog @employeeID, @delegateID, 32, @entityID, @recordtitle, null;

	return @entityID;
END
