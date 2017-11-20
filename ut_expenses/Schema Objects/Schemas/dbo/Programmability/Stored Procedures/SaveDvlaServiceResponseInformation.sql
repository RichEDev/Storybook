
Create PROCEDURE [dbo].SaveDvlaServiceResponseInformation
   @employeeId INT,    
   @responseCode NVARCHAR(150)   
AS
BEGIN

DECLARE    @username NVARCHAR(50)
DECLARE @OldResponseCode NVARCHAR(30);
SELECT @OldResponseCode=DvlaResponseCode, @username= username FROM employees      
        WHERE employeeid = @employeeid;  

UPDATE employees
        SET DvlaResponseCode = @responseCode           
        WHERE employeeid = @employeeid;
        Exec addUpdateEntryToAuditLog @employeeId,'',25,@employeeId,'58EFBC2F-EE31-4611-BC98-4348A35204DB',@OldResponseCode,@responseCode,@username,null         
        END		
