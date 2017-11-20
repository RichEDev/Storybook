
--Create claims view
CREATE VIEW [dbo].[claims]
AS
SELECT     dbo.claims_base.*, CASE WHEN paid = 0 THEN
                          (SELECT     COUNT(*)
                            FROM          savedexpenses_current
                            WHERE      claimid = claims_base.claimid AND savedexpenses_current.primaryitem = 1) WHEN paid = 1 THEN
                          (SELECT     COUNT(*)
                            FROM          savedexpenses_previous
                            WHERE      claimid = claims_base.claimid AND savedexpenses_previous.primaryitem = 1) END AS noitems, CASE WHEN paid = 0 THEN
                          (SELECT     MAX(date)
                            FROM          savedexpenses_current
                            WHERE      claims_base.claimid = savedexpenses_current.claimid) WHEN paid = 1 THEN
                          (SELECT     MAX(date)
                            FROM          savedexpenses_previous
                            WHERE      claims_base.claimid = savedexpenses_previous.claimid) END AS maxdate, CASE WHEN paid = 0 THEN
                          (SELECT     MIN(date)
                            FROM          savedexpenses_current
                            WHERE      claims_base.claimid = savedexpenses_current.claimid) WHEN paid = 1 THEN
                          (SELECT     MIN(date)
                            FROM          savedexpenses_previous
                            WHERE      claims_base.claimid = savedexpenses_previous.claimid) END AS mindate, CASE WHEN paid = 0 THEN
						  (SELECT     SUM(round(total,2))
                            FROM          savedexpenses_current
                            WHERE      claims_base.claimid = savedexpenses_current.claimid) WHEN paid = 1 THEN
                          (SELECT     SUM(round(total,2))
                            FROM          savedexpenses_previous
                            WHERE      claims_base.claimid = savedexpenses_previous.claimid) END AS total
FROM         dbo.claims_base
