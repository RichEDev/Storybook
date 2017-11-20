CREATE PROCEDURE [dbo].[APIdeleteEsrPerson]
@EsrPersonId bigint 
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
 -- UPDATE foreign key references
 UPDATE employees SET ESRPersonId = NULL WHERE ESRPersonId = @EsrPersonId;
 UPDATE ESRAddresses SET ESRPersonId = NULL WHERE ESRPersonId = @EsrPersonId;
 UPDATE ESRPhones SET ESRPersonId = NULL WHERE ESRPersonId = @EsrPersonId;
 UPDATE ESRVehicles SET ESRPersonId = NULL WHERE ESRPersonId = @EsrPersonId;
 UPDATE esr_assignments SET ESRPersonId = NULL WHERE ESRPersonId = @EsrPersonId;
 UPDATE esr_assignments SET SupervisorPersonId = null WHERE SupervisorPersonId = @EsrPersonId;

 -- delete record
 delete from [dbo].[ESRPersons] where ESRPersonId = @EsrPersonId;
END