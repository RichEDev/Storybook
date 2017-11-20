
CREATE PROCEDURE [dbo].[SaveEmployeeDvlaLookUpInformation]  
   @employeeId INT,  
   @responseCode NVARCHAR(6),  
   @dvlaLookupDate DATETIME    
AS  
BEGIN  
 

   DECLARE    @username NVARCHAR(50)  
	DECLARE @OldResponseCode NVARCHAR(30);
	DECLARE  @OldDvlaLookupDate DATETIME ;
    SELECT @OldResponseCode=DvlaResponseCode,@OldDvlaLookupDate=DvlaLookupDate, @username= username FROM employees      
        WHERE employeeid = @employeeid;  

		IF((LTRIM(RTRIM(@responseCode)) <> '' AND  LTRIM(RTRIM(@responseCode)) NOT IN('601','602')))
		BEGIN
		    SET @dvlaLookupDate=@OldDvlaLookupDate
        END

        UPDATE employees  
           SET DvlaLookUpDate = @dvlaLookupDate  
           ,DvlaResponseCode = (CASE  WHEN (LTRIM(RTRIM(@responseCode)) ='') THEN NULL ELSE  (@responseCode) END )   		              
        WHERE employeeid = @employeeid;  

		
	IF((ISNULL(@OldResponseCode,'')<>LTRIM(RTRIM(@responseCode))) OR @OldResponseCode IS NULL)
	BEGIN
	  IF(@OldResponseCode is NULL AND LTRIM(RTRIM(@responseCode)) ='')
	         EXEC addInsertEntryToAuditLog @employeeid,@employeeid,25,@employeeid,'DVLA lookup completed successfully',NULL;
      ELSE IF(@OldResponseCode<>'' AND LTRIM(RTRIM(@responseCode)) ='')
	  BEGIN
	         EXEC addInsertEntryToAuditLog @employeeid,@employeeid,25,@employeeid,'DVLA lookup completed successfully',NULL;
	         Exec addUpdateEntryToAuditLog @employeeId,'',25,@employeeId,'58EFBC2F-EE31-4611-BC98-4348A35204DB',@OldResponseCode,'',@username,null 
			 END
     ELSE IF((LTRIM(RTRIM(@responseCode)))<>'' AND ISNULL(@OldResponseCode,'') ='')
	         Exec addUpdateEntryToAuditLog @employeeId,'',25,@employeeId,'58EFBC2F-EE31-4611-BC98-4348A35204DB',@OldResponseCode,@responseCode,@username,null 
	  ELSE IF((ISNULL(@OldResponseCode,'')<>'') AND LTRIM(RTRIM(@responseCode)) <>'')
	         Exec addUpdateEntryToAuditLog @employeeId,'',25,@employeeId,'58EFBC2F-EE31-4611-BC98-4348A35204DB',@OldResponseCode,@responseCode,@username,null 
    END
END

