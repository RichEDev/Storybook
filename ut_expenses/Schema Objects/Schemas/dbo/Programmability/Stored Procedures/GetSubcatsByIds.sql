CREATE PROCEDURE GetSubcatsByIds (@SubcatIds INTPK READONLY)
AS
  BEGIN
      SELECT subcatid,
             subcat
      FROM   dbo.subcats
      WHERE  subcatid IN (SELECT *
                          FROM   @SubcatIds)
      ORDER  BY subcat
  END  