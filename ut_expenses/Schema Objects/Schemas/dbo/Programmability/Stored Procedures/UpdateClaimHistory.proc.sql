CREATE PROCEDURE [dbo].[UpdateClaimHistory]
 @claimId INT,
 @stage INT,
 @comment NVARCHAR(4000),
 @datestamp DATETIME,
 @createdOn DATETIME,
 @employeeId INT = NULL,
 @refnum nvarchar(4000) = NULL
AS
BEGIN
 SET NOCOUNT ON;
 INSERT INTO claimhistory (claimid, stage, comment, datestamp, employeeid, createdon, refnum) 
 VALUES (@claimId, @stage, @comment, @datestamp, @employeeId, @createdOn, @refnum)
 END
GO


