
CREATE PROCEDURE [dbo].[DeleteCardCompany] 
(
	@ID INT, 
	@UserId INT,
	@DelegateID int
)
AS
DECLARE @company nvarchar(50);

	select @company = companyName from cardCompanies where cardCompanyID = @ID;

	delete from cardCompanies where cardCompanyID = @ID;

	exec addDeleteEntryToAuditLog @UserId, @DelegateID, 160, @ID, @company, null;
