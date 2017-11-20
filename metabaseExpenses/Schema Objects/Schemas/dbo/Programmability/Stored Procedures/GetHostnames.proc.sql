CREATE  PROCEDURE [dbo].[GetHostnames] 
AS
BEGIN
SET NOCOUNT ON;
 
SELECT hostname, moduleID FROM hostnames;
END