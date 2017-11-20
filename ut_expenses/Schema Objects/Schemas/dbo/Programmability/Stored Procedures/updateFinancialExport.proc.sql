CREATE  PROCEDURE [dbo].[updateFinancialExport] (
@financialexportid int, 
@applicationtype tinyint,
 @reportid uniqueidentifier,
  @modifiedby int, 
  @automated bit, 
  @exporttype tinyint, 
  @NHSTrustID int, 
  @employeeID INT, 
  @delegateID INT, 
  @preventNegativePayment BIT,
  @expeditePaymentReport BIT) 	
AS
BEGIN
	SET NOCOUNT ON;

	declare @oldapplicationtype tinyint;
	declare @oldreportid uniqueidentifier;
	declare @oldautomated bit;
	declare @oldexporttype tinyint;
	declare @oldNHSTrustID int;
	declare @oldexpeditePaymentReport bit;
	select @oldapplicationtype = applicationtype, @oldreportid = reportid, @oldautomated = automated, @oldexporttype = exporttype, @oldNHSTrustID = NHSTrustID, @oldexpeditePaymentReport= ExpeditePaymentReport from financial_exports where financialexportid = @financialexportid;

    -- Insert statements for procedure here
	update financial_exports set applicationtype = @applicationtype, modifiedby = @modifiedby, modifiedon = getDate(), automated = @automated, exporttype = @exporttype, reportid = @reportid, NHSTrustID = @NHSTrustID, PreventNegativeAmountPayable = @preventNegativePayment , ExpeditePaymentReport = @expeditePaymentReport  where financialexportid = @financialexportid;
	
	if @oldapplicationtype <> @applicationtype
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 32, @financialexportid, '024431ac-cf85-41ef-aff0-b21899423c42', @oldapplicationtype, @applicationtype, null, null;
	if @oldreportid <> @reportid
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 32, @financialexportid, '02b64267-2979-4201-89b2-63f35f1ab3ac', @oldreportid, @reportid, null, null;
	if @oldautomated <> @automated
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 32, @financialexportid, 'a7a6d496-1fdf-4e4e-a123-9720c93b46fb', @oldautomated, @automated, null, null;
	if @oldexporttype <> @exporttype
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 32, @financialexportid, '40d277a1-92f9-4e39-a016-c54fca99e29d', @oldexporttype, @exporttype, null, null;
	if @oldNHSTrustID <> @NHSTrustID
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 32, @financialexportid, 'b678ac87-cf0d-4602-8be0-facc4d1876ec', @oldNHSTrustID, @NHSTrustID, null, null;
	if @oldexpeditePaymentReport <> @expeditePaymentReport 
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 32, @financialexportid, '56C06056-E7AB-420F-BC9D-892501B65CE4' , @oldexpeditePaymentReport , @expeditePaymentReport , null, null; 
END

