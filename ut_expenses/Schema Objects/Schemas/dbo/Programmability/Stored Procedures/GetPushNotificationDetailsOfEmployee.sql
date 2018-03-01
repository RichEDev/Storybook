CREATE PROCEDURE [dbo].[GetPushNotificationDetailsOfEmployee] ( @EmployeeIds IntPK READONLY)
AS
BEGIN
	
	/* Employee have to be Active on Device (shouldn't be logged out),Allownotification and Registered need be true */
    SELECT    
        ID,AllowNotifications,Registered,PushChannel,RegistrationId,RegisteredTag,[Platform],EmployeeId
    FROM 
        [MobileMetricData]  AS A
    INNER JOIN @EmployeeIds AS B ON (A.EmployeeId=B.c1)
    WHERE 
        Active=1 AND 
        AllowNotifications=1 AND 
        Registered=1;    
END
