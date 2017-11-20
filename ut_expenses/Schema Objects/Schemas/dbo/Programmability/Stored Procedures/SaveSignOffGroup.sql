CREATE PROCEDURE [dbo].[SaveSignOffGroup] @groupName    NVARCHAR(50),
                                  @description  NVARCHAR(500),
                                  @oneClickAuth BIT,
                                  @groupId      INT,
								  @mailClaimantWhenEnvelopeReceived BIT = NULL,
								  @mailClaimantWhenEnvelopeNotReceived	BIT = NULL,
                                  @modifiedOn   DATETIME,
                                  @modifiedBy   INT,
                                  @delegateID   INT
AS
  BEGIN
  
  DECLARE @ElementId INT; 
  SELECT @ElementId = elementID FROM [Elements] WHERE elementFriendlyName = 'Groups'
      IF @groupId = 0
        BEGIN
            INSERT INTO groups
                        (groupname,
                         [description],
                         oneclickauthorisation,
						 MailClaimantWhenEnvelopeReceived,
						 MailClaimantWhenEnvelopeNotReceived,
                         createdon,
                         createdby)
            VALUES      (@groupName,
                         @description,
                         @oneClickAuth,
						 @mailClaimantWhenEnvelopeReceived,
						 @mailClaimantWhenEnvelopeNotReceived,
                         @modifiedOn,
                         @modifiedBy);

            SET @groupId = Scope_identity();

            BEGIN
                EXEC Addinsertentrytoauditlog
                  @modifiedBy,
                  @delegateid,
                  @ElementId,
                  @groupId,
                  @groupname,
                  NULL;
            END
        END
      ELSE
        BEGIN
            DECLARE @oldGroupName NVARCHAR(255);
            DECLARE @oldDescription NVARCHAR(255);
            DECLARE @oldOneClickAuth BIT;
			DECLARE @oldMailClaimantWhenEnvelopeReceived BIT;
			DECLARE @oldMailClaimantWhenEnvelopeNotReceived BIT;

            BEGIN
                SELECT @oldGroupName = groupname,
                       @oldDescription = [description],
                       @oldOneClickAuth = oneclickauthorisation,
					   @oldMailClaimantWhenEnvelopeReceived = MailClaimantWhenEnvelopeReceived,
					   @oldMailClaimantWhenEnvelopeNotReceived = MailClaimantWhenEnvelopeNotReceived
                FROM   groups
                WHERE  groupid = @groupId
            END


		   BEGIN
				UPDATE groups
				SET    groupname =@groupName,
					   [description] = @description,
					   oneclickauthorisation = @oneClickAuth,
					   MailClaimantWhenEnvelopeReceived = @mailClaimantWhenEnvelopeReceived,
					   MailClaimantWhenEnvelopeNotReceived = @mailClaimantWhenEnvelopeNotReceived,
					   modifiedon= @modifiedOn,
					   modifiedby = @modifiedBy
				WHERE  groupid = @groupid		        
		  END
	BEGIN

		if (@oldGroupName <> @groupName) or (@oldGroupName is Null and @groupName is not NULL)
			exec addUpdateEntryToAuditLog @modifiedBy, @delegateID, @ElementId, @groupId, 'AE818689-4B20-40A4-B5BA-3F1AB8B523BB', @oldGroupName, @groupName, @groupName, null;
		if (@oldDescription <> @description) or (@oldDescription is Null and @description is not NULL) or (@oldDescription = '' and @description is not NULL)
			exec addUpdateEntryToAuditLog @modifiedBy, @delegateID, @ElementId, @groupId, '74A2EA70-017C-4C14-915A-C4CDF0A074D7', @oldDescription, @description, @groupName, null;
		if (@oldOneClickAuth <> @oneClickAuth) or (@oldOneClickAuth is Null and @oneClickAuth is not NULL)
			exec addUpdateEntryToAuditLog @modifiedBy, @delegateID, @ElementId, @groupId, 'C129F58A-4FDC-474E-B676-EC79707455B3', @oldOneClickAuth, @oneClickAuth, @groupName, null;
		
		if (@oldMailClaimantWhenEnvelopeReceived <> @mailClaimantWhenEnvelopeReceived) or (@oldMailClaimantWhenEnvelopeReceived is Null and @mailClaimantWhenEnvelopeReceived is not NULL)
			exec addUpdateEntryToAuditLog @modifiedBy, @delegateID, @ElementId, @groupId, null, @oldMailClaimantWhenEnvelopeReceived, @mailClaimantWhenEnvelopeReceived, @groupName, null;
		if (@oldMailClaimantWhenEnvelopeNotReceived <> @mailClaimantWhenEnvelopeNotReceived) or (@oldMailClaimantWhenEnvelopeNotReceived is Null and @mailClaimantWhenEnvelopeNotReceived is not NULL)
			exec addUpdateEntryToAuditLog @modifiedBy, @delegateID, @ElementId, @groupId, null, @oldMailClaimantWhenEnvelopeNotReceived, @mailClaimantWhenEnvelopeNotReceived, @groupName, null;
		
	END	
	END	
	RETURN @groupId
	END