CREATE PROCEDURE SaveAllowancesToSubcat(@SubcatId     INT,
                               @AllowanceIds INTPK readonly)
AS
  BEGIN
      DELETE FROM subcats_allowances
      WHERE  subcatid = @SubcatId

      INSERT INTO subcats_allowances
                  (subcatid,
                   allowanceid)
      SELECT @SubcatId,
             c1
      FROM   @AllowanceIds
  END  