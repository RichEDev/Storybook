CREATE FUNCTION dbo.RechargeItemIsOffline(@RA_Id int, @curDate datetime)
RETURNS int
AS
BEGIN
DECLARE @isOffline int
SET @isOffline = (
SELECT COUNT(*) FROM recharge_servicedates WHERE @curDate BETWEEN [OfflineFrom] AND [OnlineFrom] AND [RechargeId] = @RA_Id
)

RETURN @isOffline
END
