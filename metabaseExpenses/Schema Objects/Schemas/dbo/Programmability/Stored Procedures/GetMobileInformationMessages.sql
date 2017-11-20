CREATE PROCEDURE [dbo].GetMobileInformationMessages
AS
  BEGIN
      SELECT informationID,
             title,
             mobileInformationMessage
      FROM   information_messages
      WHERE  (deleted = 0
              AND mobileInformationMessage IS NOT NULL)
      ORDER  BY displayorder
  END  