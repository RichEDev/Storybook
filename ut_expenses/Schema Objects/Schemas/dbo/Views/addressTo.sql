CREATE VIEW [dbo].[addressesTo]
	AS SELECT a.AddressID AS addressToId,
			CASE WHEN al.label IS NOT NULL AND al.label <> '' THEN al.label 
				WHEN a.AddressName IS NOT NULL AND a.addressname <> '' THEN a.addressname + ', ' + a.Line1 + ', ' + a.Postcode
				ELSE a.Line1 + ', ' + a.Postcode END AS addressTo,
			a.Archived
			FROM dbo.addresses AS a
			LEFT OUTER JOIN dbo.addressLabels AS al ON a.AddressID = al.AddressID AND al.IsPrimary = 1 AND al.EmployeeID IS NULL
