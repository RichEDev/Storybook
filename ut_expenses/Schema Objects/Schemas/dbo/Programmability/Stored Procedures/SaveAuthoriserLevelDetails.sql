CREATE  PROCEDURE [dbo].[SaveAuthoriserLevelDetails]
	  @AuthoriserLevelDetailId int,
      @Amount [decimal](18, 2),
	  @Description NVARCHAR(MAX),
	  @employeeID INT,
	  @delegateID INT
	      AS
BEGIN
declare @record nvarchar(50)
 if	@AuthoriserLevelDetailId=0
 begin
  insert into AuthoriserLevelDetails(Amount,Description) values(@Amount,@Description)
  SET  @AuthoriserLevelDetailId = SCOPE_IDENTITY();
  SET @record = 'Authoriser level Record (' + CAST(@AuthoriserLevelDetailId as nvarchar(5)) +')';
  exec addInsertEntryToAuditLog @employeeID, @delegateID, 192, @AuthoriserLevelDetailId, @record, null;
 End
 Else
 begin

 declare @oldAmount [decimal](18, 2);
 declare @oldDescription NVARCHAR(MAX);
 select @oldAmount = Amount, @oldDescription = Description from AuthoriserLevelDetails  where AuthoriserLevelDetailId= @AuthoriserLevelDetailId

 update AuthoriserLevelDetails set Amount =@Amount,Description= @Description where AuthoriserLevelDetailId= @AuthoriserLevelDetailId

 SET @record = 'Authoriser level Record (' + CAST(@AuthoriserLevelDetailId as nvarchar(5)) +')';
 if @oldAmount <> @Amount
      exec addUpdateEntryToAuditLog @employeeID, @delegateID, 192, @AuthoriserLevelDetailId, '78FB2B48-7462-48FE-9E1E-A0D3E4A4FB4C', @oldAmount, @Amount, @record, null;
 if @oldDescription <> @Description
      exec addUpdateEntryToAuditLog @employeeID, @delegateID, 192, @AuthoriserLevelDetailId, '01CE94CA-7F9C-449D-8E31-8EA01F14AE21', @oldDescription, @Description, @record, null;
 End
 return @AuthoriserLevelDetailId
End


