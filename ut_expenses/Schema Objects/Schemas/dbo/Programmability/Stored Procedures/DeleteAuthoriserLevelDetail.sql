
Create PROCEDURE [dbo].[DeleteAuthoriserLevelDetail]
@AuthoriserLevelDetailId int,
@IsAuthoriserLevelAssigned bit,
@employeeID INT,
@delegateID INT
As
BEGIN
if	@IsAuthoriserLevelAssigned=1
Begin
update employees set  AuthoriserLevelDetailId=null where AuthoriserLevelDetailId= @AuthoriserLevelDetailId
End
delete AuthoriserLevelDetails where AuthoriserLevelDetailId= @AuthoriserLevelDetailId
declare @recordTitle nvarchar(50)
SET @recordTitle = 'Authoriser level Record (' + CAST(@AuthoriserLevelDetailId as nvarchar(5)) +')';
EXEC addDeleteEntryToAuditLog @employeeID, @delegateID, 192, @AuthoriserLevelDetailId, @recordTitle, NULL;
return @@ROWCOUNT
END