-- <Migration ID="104900bd-09ca-4211-8272-00ffc5b04dc8" />
GO
SET IDENTITY_INSERT [dbo].[menu_structure_base] ON 
GO
INSERT [dbo].[menu_structure_base] ([menuid], [menu_name], [parentid]) VALUES (1, N'Home', NULL)
GO
INSERT [dbo].[menu_structure_base] ([menuid], [menu_name], [parentid]) VALUES (2, N'Administrative Settings', 1)
GO
INSERT [dbo].[menu_structure_base] ([menuid], [menu_name], [parentid]) VALUES (3, N'Base Information', 2)
GO
INSERT [dbo].[menu_structure_base] ([menuid], [menu_name], [parentid]) VALUES (4, N'Tailoring', 2)
GO
INSERT [dbo].[menu_structure_base] ([menuid], [menu_name], [parentid]) VALUES (5, N'Policy Information', 2)
GO
INSERT [dbo].[menu_structure_base] ([menuid], [menu_name], [parentid]) VALUES (6, N'User Management', 2)
GO
INSERT [dbo].[menu_structure_base] ([menuid], [menu_name], [parentid]) VALUES (7, N'System Options', 2)
GO
SET IDENTITY_INSERT [dbo].[menu_structure_base] OFF
GO
