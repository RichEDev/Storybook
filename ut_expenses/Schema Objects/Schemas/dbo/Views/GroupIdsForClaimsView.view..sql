CREATE VIEW [dbo].[GroupIdsForClaimsView]
AS
SELECT DISTINCT CASE (
   MAX(CASE itemtype
     WHEN 1
      THEN 1
     ELSE 0
     END) + MAX(CASE itemtype
     WHEN 2
      THEN 3
     ELSE 0
     END) + MAX(CASE itemtype
     WHEN 3
      THEN 5
     ELSE 0
     END)
   )
  WHEN 1
   THEN (e.groupid)
  WHEN 3
   THEN CASE 
     WHEN e.groupidcc IS NULL
      THEN (e.groupid)
     ELSE (e.groupidcc)
     END
  WHEN 4
   THEN CASE 
     WHEN e.groupidcc IS NULL
      THEN (e.groupid)
     ELSE (e.groupidcc)
     END
  WHEN 5
   THEN CASE 
     WHEN e.groupidpc IS NULL
      THEN (e.groupid)
     ELSE (e.groupidpc)
     END
  WHEN 6
   THEN CASE 
     WHEN e.groupidpc IS NULL
      THEN (e.groupid)
     ELSE (e.groupidpc)
     END
  WHEN 8
   THEN (e.groupid)
  WHEN 9
   THEN (e.groupid)
  END AS groupid
 ,cb.claimid
FROM dbo.savedexpenses AS se
INNER JOIN dbo.claims_base AS cb ON cb.claimid = se.claimid
INNER JOIN dbo.employees AS e ON cb.employeeid = e.employeeid
WHERE
 (cb.paid = 0)
 AND (cb.submitted = 1)
 AND (cb.checkerid IS NULL)
GROUP BY e.groupid
 ,e.groupidcc
 ,e.groupidpc
 ,cb.claimid
 




