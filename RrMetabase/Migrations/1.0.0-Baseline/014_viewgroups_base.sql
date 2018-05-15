-- <Migration ID="04678265-04d2-4eae-ab41-133151d793d4" />
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Suppliers', 1, CAST(N'2017-04-29T05:39:05.753' AS DateTime), N'c337dc48-b9ce-49c7-85ed-9a8f5d1f4ded', NULL, NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Employee Details', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Limits', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'3a092d76-1992-4b45-bef3-a0f6895dd282', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Licence Details', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'a95009e1-577b-47a9-a0a1-a3fcf47af740', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Budget Holders', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'a170f725-a52d-40f1-81cc-a587daf143b3', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Details', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Site Information', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'8b773675-376a-407b-90b6-ae90928d446d', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Tasks', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'8b75cb1c-d324-4de8-9429-bc10a9fe7a73', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Bank Details', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'5b0c3563-74f0-4df4-9520-be102258f7b6', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Contacts', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'e1cd3c87-2d44-4f70-81c4-c1d285d14fbb', NULL, NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'ESR_Elements', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'105391ff-d17a-4c03-8373-c4367e77dc88', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Cost Codes', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'28faebd9-b408-4fef-aacd-c51dbdf2076e', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Currencies', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'85480325-2b78-4150-b0a6-c545b19c36c8', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'NHS Trust Details', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'7d92b21c-7cad-4394-a4b5-c7d0529c2b74', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Employees', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'ae17d219-9d45-4728-975f-c8f64c919060', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Flags', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'6e936398-66d3-40f4-82b1-4380fea9b1b0', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Supplier Contacts', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'd67b7263-7cb0-4273-908b-cc848431f3a6', N'e1cd3c87-2d44-4f70-81c4-c1d285d14fbb', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Base Country', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'de1b5d1a-d205-4d0c-a01d-d03c936ab1a9', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Invoice Details', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'6261d738-e29f-45b1-a100-d213a89e5bdc', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Invoice Forecast Details', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'f34e689e-45a7-45fc-a544-d5a946ffca94', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Advances', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'b70f07eb-9c55-4bc4-9156-d6b8f31d9c1a', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Mileage Totals', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'b42ba4a2-8af7-4ffe-838a-da9ffe27a1e7', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Hotel Details', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'2dc4b0aa-be43-41bc-9bde-dbef033cf6fd', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier User Defined', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'd27782d8-79db-4118-bd4b-de97422a5250', N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Home Address', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'2b5cb880-07c9-4e7b-8e20-e2432d14fa67', N'e1cd3c87-2d44-4f70-81c4-c1d285d14fbb', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Financial Years', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'f6adda72-dcba-4815-80b2-6affd7a98a87', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Products', 1, CAST(N'2017-04-29T05:39:05.757' AS DateTime), N'4189fb7a-1c73-4e55-9f86-ea767dc49288', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Supplier Details', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'4506185f-8e2a-47af-8846-f15e31549c1f', N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Roles', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'd67f9c21-2bc1-4bc2-a0f2-f1febdcecac1', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Vehicle Journey Rate Categories', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'b33d2e55-e6de-4052-876c-f3014b57de9a', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Reason Details', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'e116a55e-e27d-4ee0-918c-f3f71c9b6acc', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Vehicle Details', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'1d37c13f-6c4d-47f3-9570-f63bb76baec1', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Team Details', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'dd4e62ae-4c5c-4cb8-b745-f875979da05c', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Purchase Card Statement Details', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'84b145db-9e29-42d5-854f-f9e7c2764435', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Expense Item Details', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Expense Categories', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'1c908bf0-b185-4f80-a7dc-fc8be8f78bf0', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Addresses', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'1429b72b-d2f7-4fa3-bfab-7b9a8bea1238', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Addresses (From)', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'7dac155a-8a03-4b63-b8c9-52f852a123b3', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Addresses (To)', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'40952260-9480-4247-88da-91c5a7f74ff2', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Organisations', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'8b6cd055-82f9-4bd1-a283-c1f936e8df19', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Fields', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'989ba3d6-1bae-4a49-a445-77300559f709', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Expense Item Bank Account', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'e99e4d5c-87c0-478e-9e34-6259967c786a', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Expense validation criteria', 1, CAST(N'2016-03-12T07:56:43.020' AS DateTime), N'aee571c5-f009-4e2e-a95f-f98d363c07f5', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Fuel Receipt To Mileage Allocation', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'872f06fa-f965-4cdd-b2c5-7978e315622b', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Vehicle Documents', 1, CAST(N'2017-01-14T06:43:59.820' AS DateTime), N'94b13f6b-350c-4c16-b238-b7afc2639b12', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Additional Fields', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'63dc9c60-9ee8-418a-9d36-a7ee1c9081f7', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Import History', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'81db7121-cdd1-440c-b7f9-06a104f091ea', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Financial Details', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'8883b13f-935a-4e2d-a0dc-09757fd86c08', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Product Information', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'bbe7ccf1-d4ae-42fa-bb22-116b89362069', N'4189fb7a-1c73-4e55-9f86-ea767dc49288', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'MOT Details', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'9c028814-8933-4f7e-82b3-12b07e1cbe21', N'1d37c13f-6c4d-47f3-9570-f63bb76baec1', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Allowance Details', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'c3961d74-4c23-41b1-8b7d-1864410c7851', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Address', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'8f6a113c-98f8-4e9a-b959-1e88dd2e95a9', N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Project Codes', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'7afe4093-3397-493a-bb17-206daa47fccc', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Item Roles', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'7f0116bf-e54c-4d76-a5bd-20a8fb7b3c6c', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Vehicle Journey Rate Date Ranges', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'090024f3-94d7-4eba-9c27-26248416f50f', N'b33d2e55-e6de-4052-876c-f3014b57de9a', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Team Members', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'bcf90890-e467-43db-b360-2e921ad622f6', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Contacts', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'633d1640-12cc-464d-b670-31947b2c588d', N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Insurance Details', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'7cfb81ed-9eaf-4403-ae02-32072319f7b0', N'1d37c13f-6c4d-47f3-9570-f63bb76baec1', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Details', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Document Merge Projects', 1, CAST(N'2017-04-29T05:39:05.760' AS DateTime), N'2b8cc292-8b71-4ba3-b5fb-358ebb0b01fc', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Hotel Ratings', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'624b68fc-3980-4da3-93d0-371298f5411b', N'2dc4b0aa-be43-41bc-9bde-dbef033cf6fd', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Monthly Exchange Rates', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'00a2cbc1-9e1d-40d1-8af0-3bccd2e6c5ac', N'85480325-2b78-4150-b0a6-c545b19c36c8', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'P11d Information', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'b6d3966e-9464-4904-abad-3cd9bb0df121', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Contact Notes', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'46d08b6c-3a69-4e8b-8f5a-3f4b99d7d08b', N'e1cd3c87-2d44-4f70-81c4-c1d285d14fbb', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Flag Details', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'2ce2e7b2-a2fd-493d-8f51-44ff20446590', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Invoices', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'baee7645-1bcc-489f-b748-479db2ba2d79', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Version Registry', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'46febe71-4f0e-4893-8452-48b9814c172e', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Groups', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'7ddec7c9-7fb9-415a-ac3e-4ad4dcf52a41', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Departments', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'b3944147-7201-4d64-b4aa-4b9ac513ab22', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Credit Card Statement Details', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'6b00602d-1887-41b3-b881-4bb6eadaa778', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Odometer Readings', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'b3f02d08-3cf1-436a-880d-4bd8cf68bd9f', N'1d37c13f-6c4d-47f3-9570-f63bb76baec1', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Logs', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'b4a74510-bf30-4180-af2d-576669317c4b', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Cost Code/Department Breakdown', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'f7a30707-922a-481d-921e-57bdcf90d62d', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Allowances', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'6b4f1b8d-3432-4f42-838e-5f1cd879bf19', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Signoff Stages', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'8f716b5b-849b-4a74-8a96-63b968d55659', N'7ddec7c9-7fb9-415a-ac3e-4ad4dcf52a41', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Product User Defined', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'86ab26a9-8a3b-42c8-a6f8-641f8e20d224', N'4189fb7a-1c73-4e55-9f86-ea767dc49288', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Invoice Notes', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'd7cc7c4d-e42d-4f76-a232-66b9515424b7', N'6261d738-e29f-45b1-a100-d213a89e5bdc', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Access Roles', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'520f60a0-447e-4bf3-970e-6899773e016a', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'ESR Assignments', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'd4a84f09-3350-4604-b795-6e1dff6aa2a4', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Notes', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'b8d6ff59-6bc4-4540-92da-6e46213707fb', N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Contract Product Details', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'30528183-190f-44b2-9b01-6e587610a01d', N'4189fb7a-1c73-4e55-9f86-ea767dc49288', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Base Currency', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'00bcd3dc-c8f3-4513-b173-6ef3a833ae55', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Static Exchange Rates', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'c708ecb0-c554-41f1-9ff1-7082b2994f4b', N'85480325-2b78-4150-b0a6-c545b19c36c8', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Merge Mappings', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'ea2e4e52-c9aa-4d03-9166-72daaffe7dad', N'2b8cc292-8b71-4ba3-b5fb-358ebb0b01fc', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Contract Details', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'dc7ecc3a-0fed-4ec4-bf89-733a0739152d', N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Expense Items', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'a533e264-e5cd-41f4-8c86-748ec20e8745', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Corporate Card Statement Details', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'0deb447a-04e7-4c3b-9e31-79285354b317', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Invoice Forecast Breakdown', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'003de161-83fd-4645-884a-7a102b0739f2', N'f34e689e-45a7-45fc-a544-d5a946ffca94', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Departments', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'ff45f7f4-a477-4320-b330-7b10eff10e87', N'b3944147-7201-4d64-b4aa-4b9ac513ab22', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Tax Details', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'fa3f2e82-9988-4830-b79a-809c6eeba062', N'1d37c13f-6c4d-47f3-9570-f63bb76baec1', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Status', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'5f88d280-1d68-4ee8-9495-81a36f104607', N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Claim Details', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'61abc192-648f-42aa-ae88-8338372c6088', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Group Stages', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'b1d35534-2b65-43da-8ecd-8453764d0e5b', N'7ddec7c9-7fb9-415a-ac3e-4ad4dcf52a41', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Subcats', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'e52e3ec6-7011-4dcd-b7ee-846737aa5985', N'a533e264-e5cd-41f4-8c86-748ec20e8745', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Category', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'b1dce8cb-a994-463e-b3a2-854a1fb2e68f', N'23207c47-fcdb-4f56-9c74-ad0dfbb9dc87', NULL, N'SUPPLIER_CAT_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Expenses', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'30c1b52d-a5b4-4671-ad84-86256b2d0f41', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Product Details', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'43ae37fa-a26e-407d-8370-89a9e0681785', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Purchase Orders', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'ce67aa41-7fca-404b-a06d-8cf74181f04f', NULL, NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Holidays', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'81e66c5e-eac8-427e-af21-8f1ad83fc3ca', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Business Address', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'c839a5d4-dd42-47d7-b253-93b3b2c7be42', N'e1cd3c87-2d44-4f70-81c4-c1d285d14fbb', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Audience', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'9909428c-55ef-4e60-9d2a-95c257ae2f77', N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Recharge Clients', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'21aa8fd7-0a71-4e8a-9b09-95d02887b8c8', N'4189fb7a-1c73-4e55-9f86-ea767dc49288', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Mileage Information', 2, CAST(N'2017-04-29T05:39:05.767' AS DateTime), N'f7dcb555-bad2-4508-a682-966f5164658f', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Recharge Templates', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'c7dba7df-dade-4d94-a75b-975b7be57c47', N'4189fb7a-1c73-4e55-9f86-ea767dc49288', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract History', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'affc971f-ce2f-460b-a14b-9ff637f6b0cf', N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contact Details', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'e349c02e-a8f7-4290-be97-a4d8c15ba89d', N'69ebf30d-82ac-4948-a0cb-a08d6fe51b0e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Product Notes', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'006ad70c-0b90-4c92-b440-9ef514b820e7', N'43ae37fa-a26e-407d-8370-89a9e0681785', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Costcodes', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'c7cc7e3e-0bda-4524-889d-af983abb008a', N'28faebd9-b408-4fef-aacd-c51dbdf2076e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Invoice Status History', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'8fcf14f8-5152-422c-98db-a93b6d17fc08', N'6261d738-e29f-45b1-a100-d213a89e5bdc', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Range Based Exchange Rates', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'a465fca1-9658-445d-b118-aca3035bb3e0', N'85480325-2b78-4150-b0a6-c545b19c36c8', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Recharge Payments', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'b2f10dd0-c1ee-4035-8294-c242a3019ff0', N'4189fb7a-1c73-4e55-9f86-ea767dc49288', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Invoice Product Details', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'91052253-20b4-46a5-bfb2-c3a93e4a0f68', N'6261d738-e29f-45b1-a100-d213a89e5bdc', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Credit Card Statement Items (Barclaycard)', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'41d39689-dea7-4310-8925-c68d17c025f3', N'6b00602d-1887-41b3-b881-4bb6eadaa778', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Purchase Card Items (RBS)', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'07931f4e-e79c-4011-991c-c6a14c6f6d85', N'84b145db-9e29-42d5-854f-f9e7c2764435', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Savings', 2, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'2ecad6aa-ca72-4b36-bfff-c70f6c37125f', N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Notes', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'b59ffb61-7aa1-482e-bafb-c86302d5aaa2', N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'General Information', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'62d5c13d-6baa-4bfb-80c0-d6db9ce1345e', N'a5e9e91b-3854-42b3-b7d4-faa354ea9a3d', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract User Defined', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'e3d202ea-e524-4931-a92c-d898c392de32', N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields vehicles', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'35089197-77a7-4e5a-9f1a-c46c002f74fd', N'1d37c13f-6c4d-47f3-9570-f63bb76baec1', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Merge Mapping Table', 3, CAST(N'2017-04-29T05:39:05.783' AS DateTime), N'728b0ce9-6bca-4d32-88b2-a0986c14ddf0', N'ea2e4e52-c9aa-4d03-9166-72daaffe7dad', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Merge Mapping Fields', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'406d69d6-e709-45d5-959c-bbe509401465', N'ea2e4e52-c9aa-4d03-9166-72daaffe7dad', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Service Details', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'4caafa56-08ae-4d30-9875-c7af5616ccca', N'1d37c13f-6c4d-47f3-9570-f63bb76baec1', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Invoice Details', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'2b793bde-2333-4a0c-aa76-eaf558a31174', N'baee7645-1bcc-489f-b748-479db2ba2d79', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Project Codes', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'446baf2d-d547-4422-8222-ec99c757024c', N'7afe4093-3397-493a-bb17-206daa47fccc', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Supplier Contact User Defined', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'a066d986-c3d9-4420-9f70-f0e4b0695802', N'633d1640-12cc-464d-b670-31947b2c588d', NULL, N'SUPPLIER_PRIMARY_TITLE')
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Contract Notification', 2, CAST(N'2017-04-29T05:39:05.780' AS DateTime), N'4cf60124-36e0-465f-839a-f74b6915ba95', N'c9171733-7eb3-4c6f-b087-3364701b614b', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Expense item Countries', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'c9e7bac0-1487-40f1-81ae-0a5d4a23fefc', N'a533e264-e5cd-41f4-8c86-748ec20e8745', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Merge Sources', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'bb7170ac-a32e-49e8-9c00-0bb04679720e', N'2b8cc292-8b71-4ba3-b5fb-358ebb0b01fc', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Vehicle Journey Rate Thresholds', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'8f51f510-5ca0-448d-923d-0f9e07702ca4', N'090024f3-94d7-4eba-9c27-26248416f50f', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Product Details', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'9a4cbc59-1d60-4947-a000-15409d2f65a7', N'43ae37fa-a26e-407d-8370-89a9e0681785', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Version Registry History', 2, CAST(N'2017-04-29T05:39:05.777' AS DateTime), N'9887cb89-a512-4dec-813a-1f9ac550f266', N'46febe71-4f0e-4893-8452-48b9814c172e', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Template User Defined', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'9d278826-a4d1-4bac-a50e-26557fbc00bb', N'c7dba7df-dade-4d94-a75b-975b7be57c47', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Expense Item Vat Rates', 1, CAST(N'2017-04-29T05:39:05.763' AS DateTime), N'0992af52-bf38-4e9e-ba09-3ed19ab97159', N'a533e264-e5cd-41f4-8c86-748ec20e8745', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Fields Claims', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'f59f255c-f917-47bb-b1fc-4ebec80aea2b', N'61abc192-648f-42aa-ae88-8338372c6088', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Transaction Details (RBS Purchase Card)', 2, CAST(N'2017-04-29T05:39:05.773' AS DateTime), N'b95f4e31-c6a0-494e-8ce4-5907f7c069bd', N'0deb447a-04e7-4c3b-9e31-79285354b317', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Merge Mapping Static', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'affc4a70-2ff1-428b-bbb8-5d680736f912', N'ea2e4e52-c9aa-4d03-9166-72daaffe7dad', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Transaction Details (RBS Credit Card)', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'a291c93f-0da2-4b89-be1f-644dcc276a78', N'0deb447a-04e7-4c3b-9e31-79285354b317', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Product Licences', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'30eac209-26ec-48ea-bbd2-67d20129c123', N'43ae37fa-a26e-407d-8370-89a9e0681785', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'User Defined Product Licence Fields', 3, CAST(N'2017-04-29T05:39:05.787' AS DateTime), N'7be07c31-b682-4bd1-81ad-1462a2fb9223', N'30eac209-26ec-48ea-bbd2-67d20129c123', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Claim History', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'92f8df16-fc32-4102-bdca-7654fa79d0f3', N'61abc192-648f-42aa-ae88-8338372c6088', NULL, NULL)
GO
INSERT [dbo].[viewgroups_base] ([groupname], [level], [amendedon], [viewgroupid], [parentid], [alias], [relabel_param]) VALUES (N'Product User Defined', 2, CAST(N'2017-04-29T05:39:05.770' AS DateTime), N'2eb39bf0-ca84-45be-9512-7a416d6bdb9a', N'43ae37fa-a26e-407d-8370-89a9e0681785', NULL, NULL)
GO