-- <Migration ID="93b6995e-6d43-4c10-87c3-6a8f9b342e4f" />
GO
SET IDENTITY_INSERT [dbo].[ExpenseValidationThresholds] ON 
GO
INSERT [dbo].[ExpenseValidationThresholds] ([ThresholdId], [Label], [LowerBound], [UpperBound]) VALUES (1, N'Business reasons', NULL, NULL)
GO
INSERT [dbo].[ExpenseValidationThresholds] ([ThresholdId], [Label], [LowerBound], [UpperBound]) VALUES (2, N'VAT up to £25', CAST(0.00 AS Decimal(18, 2)), CAST(24.99 AS Decimal(18, 2)))
GO
INSERT [dbo].[ExpenseValidationThresholds] ([ThresholdId], [Label], [LowerBound], [UpperBound]) VALUES (3, N'VAT up to £250', CAST(25.00 AS Decimal(18, 2)), CAST(249.99 AS Decimal(18, 2)))
GO
INSERT [dbo].[ExpenseValidationThresholds] ([ThresholdId], [Label], [LowerBound], [UpperBound]) VALUES (4, N'VAT over £250', CAST(250.00 AS Decimal(18, 2)), CAST(999999999.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[ExpenseValidationThresholds] OFF
GO
