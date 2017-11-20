CREATE PROCEDURE [dbo].[saveTrust]
      @trustID int,
      @expTrustID int,
      @AccountID int,
      @trustName nvarchar(150),
      @trustVPD nvarchar(3),
      @ftpAddress nvarchar(100),
      @ftpUsername nvarchar(100),
      @ftpPassword nvarchar(100),
      @archived bit
AS 

      IF (@trustID = 0)
            BEGIN
                  INSERT INTO NHSTrustDetails (expTrustID, AccountID, trustName, trustVPD, ftpAddress, ftpUsername, ftpPassword, archived, createdOn) VALUES (@expTrustID, @AccountID, @trustName, @trustVPD, @ftpAddress, @ftpUsername, @ftpPassword, @archived, getutcdate());
                  SET @trustID = scope_identity();          
            END
      ELSE
            BEGIN
                  UPDATE NHSTrustDetails SET trustName=@trustName, trustVPD=@trustVPD, ftpAddress=@ftpAddress, ftpUsername=@ftpUsername, ftpPassword=@ftpPassword, archived=@archived, modifiedOn = getutcdate() WHERE expTrustID = @expTrustID AND AccountID = @AccountID;
            END

      RETURN @trustID;
