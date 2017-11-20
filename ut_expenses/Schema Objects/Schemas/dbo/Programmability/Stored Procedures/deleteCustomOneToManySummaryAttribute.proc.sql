CREATE PROCEDURE [dbo].[deleteCustomOneToManySummaryAttribute]
@attributeid int,
@CUemployeeID int,
@CUdelegateID int
AS

	delete from [customEntityFormFields] where attributeid = @attributeid
	delete from [customEntityAttributeSummaryColumns] where attributeid = @attributeid;
	delete from [customEntityAttributeSummary] where attributeid = @attributeid;
	delete from [customEntityAttributes] where attributeid = @attributeid;	

	return 0;


