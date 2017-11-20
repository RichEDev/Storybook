Create PROC DeleteCustomMenu
@custommenuid INT
AS
BEGIN
SET NOCOUNT ON;
;WITH temp as(
    SELECT CustomMenuId FROM CustomMenu
    WHERE CustomMenuId = @CustomMenuId

    UNION ALL

    SELECT c.CustomMenuId FROM CustomMenu c
    INNER JOIN temp x ON c.ParentMenuId = x.CustomMenuId
)
DELETE CustomMenu WHERE CustomMenuId IN (SELECT CustomMenuId  FROM temp)
END