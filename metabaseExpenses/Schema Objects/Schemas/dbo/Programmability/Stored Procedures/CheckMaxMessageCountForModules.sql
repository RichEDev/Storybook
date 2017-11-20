CREATE PROCEDURE [dbo].[CheckMaxMessageCountForModules]  
@messageId INT
AS  
  BEGIN
  IF @messageId=0
  BEGIN
  SELECT    Cast(m.brandName AS VARCHAR)   AS modules
    FROM MessageModuleBase mmb 
	INNER JOIN moduleBase m on m.moduleID=mmb.ModuleId
	WHERE mmb.moduleid in(  
SELECT mm.moduleid FROM LogonMessages  
LM INNER JOIN MessageModuleBase MM ON MM.MessageId=LM.MessageId 
WHERE  LM.Archived=0
GROUP BY mm.moduleid  
HAVING COUNT(mm.messageid)>2)  GROUP BY m.brandName 
  END
  ELSE
  BEGIN
  SELECT    Cast(m.brandName AS VARCHAR)   AS modules
    FROM MessageModuleBase mmb 
	INNER JOIN moduleBase m on m.moduleID=mmb.ModuleId
	WHERE mmb.moduleid in(  
SELECT mm.moduleid FROM LogonMessages  
LM INNER JOIN MessageModuleBase MM ON MM.MessageId=LM.MessageId 
WHERE  LM.Archived=0
GROUP BY mm.moduleid  
HAVING COUNT(mm.messageid)>2)  and messageid=@messageId
  END
  END