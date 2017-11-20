CREATE FUNCTION [dbo].[getReportType] 
(
	@reportID uniqueidentifier
)
RETURNS tinyint
AS
BEGIN
	declare @count int
	set @count = (select count(reportid) from reportcolumns where reportid = @reportID)
	if @count = 0
		return 0
		
	set @count = (select count(reportid) from reportcolumns where reportid = @reportID and (funcsum = 1 or funcmax = 1 or funcmin = 1 or funcavg = 1 or funccount = 1))
	if @count = 0
		return 1
	else
		return 2

	return 0
END
