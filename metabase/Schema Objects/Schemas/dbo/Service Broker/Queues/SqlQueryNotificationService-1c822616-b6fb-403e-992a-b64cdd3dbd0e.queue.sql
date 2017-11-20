CREATE QUEUE [dbo].[SqlQueryNotificationService-1c822616-b6fb-403e-992a-b64cdd3dbd0e]
    WITH STATUS = ON, RETENTION = OFF, ACTIVATION (STATUS = ON, PROCEDURE_NAME = [dbo].[SqlQueryNotificationStoredProcedure-1c822616-b6fb-403e-992a-b64cdd3dbd0e], MAX_QUEUE_READERS = 1, EXECUTE AS OWNER);

