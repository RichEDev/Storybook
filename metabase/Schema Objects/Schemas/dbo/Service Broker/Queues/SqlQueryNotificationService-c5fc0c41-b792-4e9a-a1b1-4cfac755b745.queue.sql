CREATE QUEUE [dbo].[SqlQueryNotificationService-c5fc0c41-b792-4e9a-a1b1-4cfac755b745]
    WITH STATUS = ON, RETENTION = OFF, ACTIVATION (STATUS = ON, PROCEDURE_NAME = [dbo].[SqlQueryNotificationStoredProcedure-c5fc0c41-b792-4e9a-a1b1-4cfac755b745], MAX_QUEUE_READERS = 1, EXECUTE AS OWNER);

