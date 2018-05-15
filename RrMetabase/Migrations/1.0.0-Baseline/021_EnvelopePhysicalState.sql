-- <Migration ID="39e18e54-e22c-41d8-b93c-bac47f785008" />
GO
SET IDENTITY_INSERT [dbo].[EnvelopePhysicalState] ON 
GO
INSERT [dbo].[EnvelopePhysicalState] ([EnvelopePhysicalStateId], [Details]) VALUES (1, N'The envelope was unsealed')
GO
INSERT [dbo].[EnvelopePhysicalState] ([EnvelopePhysicalStateId], [Details]) VALUES (2, N'The envelope was torn')
GO
INSERT [dbo].[EnvelopePhysicalState] ([EnvelopePhysicalStateId], [Details]) VALUES (3, N'The envelope was rebagged by the courier')
GO
INSERT [dbo].[EnvelopePhysicalState] ([EnvelopePhysicalStateId], [Details]) VALUES (4, N'The envelope has some other form of defect')
GO
SET IDENTITY_INSERT [dbo].[EnvelopePhysicalState] OFF
GO
