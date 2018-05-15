-- <Migration ID="9cb2a48d-d49b-4c46-a672-f4d6d5f514b7" />
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Claims to Employee_Currencies', CAST(N'2014-05-31T06:40:10.880' AS DateTime), N'865350cb-002b-4826-b45e-93d50e48105b', N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'11fb020f-06c9-4fc9-bcb5-761218fa702f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employees to EsrOrganisations', CAST(N'2014-11-29T06:48:16.460' AS DateTime), N'7ec6044c-2f3d-4f3b-a348-38426c460bbc', N'618db425-f430-4660-9525-ebab444ed754', N'30e0686e-702b-425e-8935-c700414fdcac')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employees to EsrOrganisations', CAST(N'2014-11-29T06:48:16.467' AS DateTime), N'7ec6044c-2f3d-4f3b-a348-38426c460bbc', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0fa546bc-86d2-4a41-908f-b817b3dbb222')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'MileageThresholdRates to VehicleEngineTypes', CAST(N'2015-06-27T05:51:16.217' AS DateTime), N'c288643c-ec84-4433-8067-98f34e66f2aa', N'2b6b725f-94f5-4918-82ab-7502d3e6193b', N'c5929482-8c49-4a7c-be9e-745e232a3777')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to allowances', CAST(N'2015-07-25T06:59:37.980' AS DateTime), N'68a1116c-b8e7-45d9-824b-acfe82c25c54', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'a38bb3f9-80eb-4ebe-96cb-d2e63beee01a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'flags to financial year definitions', CAST(N'2015-07-25T06:59:38.357' AS DateTime), N'81a44d23-210a-4997-9b9d-0c62ab29be62', N'3b9be907-1839-459a-8499-24b12e839bbb', N'b44959cd-9cf9-44f7-83b1-ef276c872b46')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to bank accounts', CAST(N'2015-08-06T21:14:17.053' AS DateTime), N'4aa56947-597a-4c91-99c8-5645561c6d01', N'618db425-f430-4660-9525-ebab444ed754', N'23163ee3-c0bd-48c2-9539-92c5e3d359d4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to bank accounts', CAST(N'2015-08-06T21:14:17.057' AS DateTime), N'4aa56947-597a-4c91-99c8-5645561c6d01', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'fa091826-6110-4093-8008-48361bfb1fa5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to ExpenseValidationResultCriteria', CAST(N'2016-03-12T07:56:43.083' AS DateTime), N'0265ee34-cef3-44f3-869c-b8d25c0b1018', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0265ee34-562d-4b95-8c0c-82e581e65846')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to claimhistory', CAST(N'2016-03-12T07:56:43.100' AS DateTime), N'c8dbf7c8-6ed7-4872-be06-5b3af3b02c5a', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'b02cc126-b432-489d-8fd6-b085a68439b5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to ExpenseSignofftype', CAST(N'2016-03-12T07:56:43.337' AS DateTime), N'0b5c1133-e7dc-4146-913c-b6f30efed776', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0b5c1133-562d-4b95-8c0c-82e581e65846')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to ExpenseVatValidationResult', CAST(N'2016-03-12T07:56:43.467' AS DateTime), N'0cc6c4e6-052f-43e4-8dd2-cfba93822c1a', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0cc6c4e6-562d-4b95-8c0c-82e581e65846')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'SavedExpensesFuelReceiptToMileageAllocations to savedexpenses', CAST(N'2016-03-12T07:56:43.510' AS DateTime), N'c736e67b-f57e-493c-a5ad-26f08413ca29', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f02a03ae-c1fd-4f86-a7cd-8dd7fd2bdb2d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to Supplier Addresses', CAST(N'2016-06-11T05:55:26.577' AS DateTime), N'e82fcac8-82c2-4055-aa19-203a00193307', N'bcd8272e-3d91-42f1-b6d2-fb79f27176db', N'e6d29bb4-2967-4622-9dc4-d2ddce8f3095')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'cars to employees', CAST(N'2017-01-14T06:43:59.713' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'f2c6878b-c82f-4a8a-9bd0-bc426103d134')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to VehicleDocumentsView', CAST(N'2017-03-11T07:46:56.890' AS DateTime), N'c3069aaf-37a8-487b-a06f-77778c2a3f38', N'618db425-f430-4660-9525-ebab444ed754', N'1309abb2-afb6-41e1-b535-c2cfc947fd67')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'VehicleDocumentsView to employees', CAST(N'2017-03-11T07:46:56.907' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'c3069aaf-37a8-487b-a06f-77778c2a3f38', N'fa8b56dc-8e4b-47c2-9cd2-f0c4cc4afb66')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'cars to VehicleDocumentsView', CAST(N'2017-03-30T17:19:01.930' AS DateTime), N'c3069aaf-37a8-487b-a06f-77778c2a3f38', N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'e65a67e8-b597-4f48-9d4e-0150bca24e1d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Global Currencies', CAST(N'2017-04-29T05:39:50.323' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'bea80624-38f4-4631-940b-1e6c32c68da5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Global Currencies', CAST(N'2017-04-29T05:39:50.323' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'f2baa891-65c4-4b01-88a9-c2dc25f80c5c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Global Currencies', CAST(N'2017-04-29T05:39:50.323' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'299ff396-6947-4d46-a4df-1983cd311a77', N'64d893ac-f576-4102-b90e-be301d6851b5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Financial Status', CAST(N'2017-04-29T05:39:50.323' AS DateTime), N'2ce5601d-6223-4269-b993-0e8aeb345a55', N'299ff396-6947-4d46-a4df-1983cd311a77', N'5a7a8f84-f5dc-405a-8295-38dffbcc5247')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Supplier Addresses', CAST(N'2017-04-29T05:39:50.323' AS DateTime), N'e82fcac8-82c2-4055-aa19-203a00193307', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'cc03be1e-d96b-43ef-a0f6-98eee0b48ff8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Global Currencies', CAST(N'2017-04-29T05:39:50.323' AS DateTime), N'b1e9111b-aa3c-4cc0-ad99-6c9e4b3e70fc', N'299ff396-6947-4d46-a4df-1983cd311a77', N'772a3e08-06e7-4478-8cb7-8a576f040ef7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employees to Global Countries', CAST(N'2017-04-29T05:39:50.327' AS DateTime), N'b1e9111b-aa3c-4cc0-ad99-6c9e4b3e70fc', N'618db425-f430-4660-9525-ebab444ed754', N'1f9d0215-cff3-49a2-8227-34c6bfbbf719')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employees to Global Currencies', CAST(N'2017-04-29T05:39:50.327' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'618db425-f430-4660-9525-ebab444ed754', N'bf337ffd-3a77-46e8-ac7a-a148fc7f5e0c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'esr_assingments to employees', CAST(N'2017-08-12T05:26:09.217' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'bf9aa39a-82d6-4960-bfef-c5943bc0542d', N'fb4adb84-12b8-46e9-a5ae-c091882a10cd')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to mobile metric data', CAST(N'2017-11-11T07:12:28.697' AS DateTime), N'cb2f8e2c-dfcb-41fd-94f6-27f9db3a811c', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'c3179223-2410-4980-a7cb-8a752c2522ec')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'ESR Positions to ESR Organisations', CAST(N'2018-03-10T06:24:22.030' AS DateTime), N'7ec6044c-2f3d-4f3b-a348-38426c460bbc', N'26777f45-1d24-4623-9f18-8639e7d6227f', N'14ba5d3e-9ec3-40ac-963d-de243025ed05')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Rolesubcats to subcats', CAST(N'2018-04-14T06:48:35.420' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'0123e0c5-5e68-4911-a062-9a6967d33beb', N'9f2e5bae-5ad6-40cb-95a6-34b8dbd62d7e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employee transactions to employee corporate cards', CAST(N'2013-07-22T22:46:18.980' AS DateTime), N'9f3aa3ed-481d-499a-89b3-f1c8aefa61e5', N'e66e950f-1fd2-4e60-acbb-09e5cff97dde', N'4d5d8cd8-6ea3-4fcb-9ea1-65b2badb9251')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedexpenses to savedexpensesgrid', CAST(N'2013-07-22T22:46:19.023' AS DateTime), N'64421bb4-88ee-40c4-b87b-c37548f2a780', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'e0592108-20c9-4b5c-ba22-0967f459fbc7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedexpenses to employee_transactions', CAST(N'2013-07-22T22:46:19.040' AS DateTime), N'e66e950f-1fd2-4e60-acbb-09e5cff97dde', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'53bbd476-cd96-4293-a7c8-dba97590b2bb')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'claims to employees', CAST(N'2013-07-22T22:46:19.040' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'c907e9a6-9974-4c30-a7ee-5b800bf3b97c', N'dc5f9552-7aae-4e87-8ef9-5201f979d023')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'unallocated claims to employees', CAST(N'2013-07-22T22:46:19.040' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'a7d77b58-a204-4b57-9823-757e13941eaf', N'10b60db4-c0d3-4d33-a950-e98c4a807593')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'EmployeeHomeAddresses To AddressEsrAllocation.EsrAddressId', CAST(N'2013-10-26T07:09:52.310' AS DateTime), N'f80427d2-8c79-4293-b959-25e9a12eb9d0', N'5026ddae-5042-45b6-a1cd-6c6ab5ff8204', N'75429343-f175-4b3e-9b9d-20117af803e5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'EmployeeWorkAddresses to AddressEsrAllocation.EsrLocationID', CAST(N'2013-10-26T07:18:33.750' AS DateTime), N'f80427d2-8c79-4293-b959-25e9a12eb9d0', N'bf306b4f-9d76-4dd2-a1c9-0f73bb31ccd7', N'fabe4448-6b9f-42af-97f4-73fbe9de2d98')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'addresses to global_countries', CAST(N'2013-12-08T00:00:00.000' AS DateTime), N'b1e9111b-aa3c-4cc0-ad99-6c9e4b3e70fc', N'a3713e93-0abe-4eac-9533-6a22aa4c1f62', N'58bbf048-ce76-4516-8419-bad6a0b2ea30')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employee Home Addresses To Addresses', CAST(N'2014-02-22T06:27:01.553' AS DateTime), N'a3713e93-0abe-4eac-9533-6a22aa4c1f62', N'5026ddae-5042-45b6-a1cd-6c6ab5ff8204', N'32bd3173-d1f8-478c-bb48-609fadc6902d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'AddressesLocations to Address', CAST(N'2013-10-09T00:00:00.000' AS DateTime), N'a3713e93-0abe-4eac-9533-6a22aa4c1f62', N'd9ad4cc8-23e0-44f6-8fdf-c2da26d567bd', N'8d8d9958-abf1-41c4-9d32-1294188da0e9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employee Work Addresses To Addresses', CAST(N'2014-02-22T06:27:01.673' AS DateTime), N'a3713e93-0abe-4eac-9533-6a22aa4c1f62', N'bf306b4f-9d76-4dd2-a1c9-0f73bb31ccd7', N'b4ab7d63-00ee-4a45-b9ea-10b241e6d2ac')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to organisations', CAST(N'2013-12-06T00:00:00.000' AS DateTime), N'7bdaf84e-a373-4008-83d1-9e18aaa47f8e', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'ab557452-6191-4df7-85dc-3599268ac9d9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to addressesFrom', CAST(N'2013-12-09T00:00:00.000' AS DateTime), N'46f1778e-4ab9-4b0e-92ec-a713a79fc333', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'36adef7d-1af4-4a2e-99cd-92c17740dde9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to addresses', CAST(N'2013-12-06T00:00:00.000' AS DateTime), N'691cc01f-fee7-4b24-bce1-49290e272284', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'5e3f5c1e-1ce8-467a-8a50-aa33c1c2b14d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Organisations to Addresses (Primary Address)', CAST(N'2013-12-10T00:00:00.000' AS DateTime), N'a3713e93-0abe-4eac-9533-6a22aa4c1f62', N'7bdaf84e-a373-4008-83d1-9e18aaa47f8e', N'853b18a0-21da-4f74-aca3-fe35053afde6')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'organisations to global_countries', CAST(N'2014-01-03T00:00:00.000' AS DateTime), N'b1e9111b-aa3c-4cc0-ad99-6c9e4b3e70fc', N'7bdaf84e-a373-4008-83d1-9e18aaa47f8e', N'0be5b23e-c524-461c-8b1e-9b41cc1b6c7c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'flags to item roles', CAST(N'2015-07-25T06:59:38.357' AS DateTime), N'db7d42fd-e1fa-4a42-84b4-e8b95c751bda', N'3b9be907-1839-459a-8499-24b12e839bbb', N'bb6e999e-43f8-4adb-8afd-b363e50f0554')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'flags to subcats', CAST(N'2015-07-25T06:59:38.360' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'3b9be907-1839-459a-8499-24b12e839bbb', N'dd0b40ce-ca99-4c47-9143-c030dc119703')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'flags to fields', CAST(N'2015-07-25T06:59:38.360' AS DateTime), N'5b32610e-35db-492a-b6d1-5f392ca4c040', N'3b9be907-1839-459a-8499-24b12e839bbb', N'b34f1db2-8d32-4f9c-ade0-896acaf18fba')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedexpenses to Categories', CAST(N'2008-10-29T10:21:43.977' AS DateTime), N'75c247c2-457e-4b14-bbec-1391cd77fb9e', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'189bb87c-e5fc-47d6-b3ff-c6328b447d2e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Saved expenses to pdcats', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'5d2d9191-83ea-4ed5-8a46-0aabb8190392', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0dfdd7b2-6a1d-4985-8245-79f7262ee72e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Saved expenses to subcats', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'8b2a2279-9222-47e4-bf0b-f88e8e9ca112')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Saved expenses to countries', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'a8f0f676-ad52-4234-9c65-59ba0e67daa8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to currencies', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'850422ea-ad71-4cef-b6af-227933bf8065', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'6b39164a-3d5b-4ed9-9995-b860268f9fb7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to floats', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'afa3b8f4-7148-4eff-acc0-f77113392f0c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to reasons', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'83077e08-fe7d-4c1a-a306-be4327c349c1', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'7aa63ff1-cf22-41ee-bd7d-a3b3a3648532')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'subcats to savedexpenses', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'96b22ca5-d882-4417-8782-a7f2c46ce2af')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'subcats to countries', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'6178159f-8ca1-40a1-bd3a-307a10fb9dd3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'claims to employees', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'e1c99f3d-37b7-46b4-aad4-abebac8324e6')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'claims to costcodes via employees', CAST(N'2008-10-29T10:21:42.850' AS DateTime), N'02009e21-aa1d-4e0d-908a-4e9d73ddfbdf', N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'8ba5046e-b9f8-4158-adbe-5cad88c57325')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'claims to departments via employees', CAST(N'2008-10-29T10:21:42.900' AS DateTime), N'a0f31cb0-16bb-4ace-aaea-69a7189d9599', N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'a6dfa10b-4b01-4c3d-b05a-417c7bc10465')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'claims to employees - userdefined', CAST(N'2008-10-29T10:21:43.007' AS DateTime), N'972ac42d-6646-4efc-9323-35c2c9f95b62', N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'cc249b91-9e14-4215-b751-2bdf2f7d8f0d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'claims to groups via employees', CAST(N'2008-10-29T10:21:42.930' AS DateTime), N'd6ab6ff4-0ec4-4996-8566-458b816adc0d', N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'5f37e577-9dac-4da9-a282-69a72724aa71')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to employees', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'21d9c710-a176-4022-89c2-fbebae5869f1')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to claims', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'9dd0d247-f7a4-49de-8405-516aa8f8e30a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'subcats to categories', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'75c247c2-457e-4b14-bbec-1391cd77fb9e', N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'5dfc9eb2-6c1f-45bc-bab7-fb07204015df')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'subcats to pdcats', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'5d2d9191-83ea-4ed5-8a46-0aabb8190392', N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'2459e1ba-42fa-44ec-859c-7256171ff630')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to groups', CAST(N'2008-10-29T10:21:42.930' AS DateTime), N'd6ab6ff4-0ec4-4996-8566-458b816adc0d', N'618db425-f430-4660-9525-ebab444ed754', N'ec6b91ee-db10-481b-ba13-fa662375813a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to accessRoles', CAST(N'2009-07-17T17:50:57.157' AS DateTime), N'12ded231-b220-4acb-a51d-896c52ff8979', N'618db425-f430-4660-9525-ebab444ed754', N'239eddff-82e9-4a39-a5a1-60436d691df1')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to costcodes', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'02009e21-aa1d-4e0d-908a-4e9d73ddfbdf', N'618db425-f430-4660-9525-ebab444ed754', N'a66a2cb1-d27b-4ac4-bcdb-27aaaa543156')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to departments', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'a0f31cb0-16bb-4ace-aaea-69a7189d9599', N'618db425-f430-4660-9525-ebab444ed754', N'b0c5a887-1e4c-4710-a56c-c1819c27b28e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to holidays', CAST(N'2008-10-29T10:21:42.930' AS DateTime), N'a6ff86d3-808f-406f-9dd6-e21b7b9a8d67', N'618db425-f430-4660-9525-ebab444ed754', N'1300b296-1ab2-4903-a08f-e217d418656c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to employees-userdefined', CAST(N'2008-10-29T10:21:43.007' AS DateTime), N'972ac42d-6646-4efc-9323-35c2c9f95b62', N'618db425-f430-4660-9525-ebab444ed754', N'077ab336-5608-4cac-9c0d-36beedba5495')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to floats', CAST(N'2008-10-29T10:21:42.930' AS DateTime), N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'618db425-f430-4660-9525-ebab444ed754', N'326ffc3d-cf93-4fe0-aaa3-73bd496a7126')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to cars', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'618db425-f430-4660-9525-ebab444ed754', N'e395f4b6-758a-48cf-9884-da9d4afa80a3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to car', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0533d739-9f40-4802-b413-efc234eb79d8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to groups', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'd6ab6ff4-0ec4-4996-8566-458b816adc0d', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'94c51a55-fcd3-469a-9bd6-243d2deef7e0')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to savedexpenses-costcodes', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'bf13d307-6f4a-40cb-906d-72530f64791e', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'844076c4-ce9c-4b44-879b-a56dc71f30e3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to costcodes', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'02009e21-aa1d-4e0d-908a-4e9d73ddfbdf', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3f818185-5621-4663-8ec2-e20eab61d375')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to departments', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'a0f31cb0-16bb-4ace-aaea-69a7189d9599', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3a10b382-7bee-495d-8e2a-85bba1da75a9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to employees_userdefined', CAST(N'2008-10-29T10:21:43.007' AS DateTime), N'972ac42d-6646-4efc-9323-35c2c9f95b62', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'ab3af55d-d87c-4663-83a7-c7f30c754b59')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to savedexpenses_flags', CAST(N'2008-10-29T10:21:43.023' AS DateTime), N'ca5762a9-bdad-45a6-97f4-f86614fc5595', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'76f0f8aa-f536-4054-ae91-6fe96bb31391')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to claims_userdefined', CAST(N'2009-11-26T06:31:58.643' AS DateTime), N'f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'014589b0-e677-4a50-8896-2964bebc73a8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to userdefinedSubcats', CAST(N'2009-11-26T06:31:58.643' AS DateTime), N'82f57980-92c9-4a4b-b76d-2e8485c0bb41', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'89195f10-8a70-47ee-be3e-c23c7bb42642')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to mileage_totals', CAST(N'2008-10-29T10:21:43.023' AS DateTime), N'6323a27f-c737-4799-844a-680a8c663701', N'618db425-f430-4660-9525-ebab444ed754', N'45654c34-a076-4ef2-b3e8-6bf6bf4dd32d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to project_codes', CAST(N'2008-10-29T10:21:43.023' AS DateTime), N'e1ef483c-7870-42ce-be54-ecc5c1d5fb34', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'b5f43d55-7dea-474a-8207-c1be2dd05b5c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to odometer_readings', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'956dba2c-ea66-454e-ba53-d5fa1eeb82b7', N'618db425-f430-4660-9525-ebab444ed754', N'b4229d8e-b368-4b30-a434-a2e93c0e2382')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to Categories', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'75c247c2-457e-4b14-bbec-1391cd77fb9e', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'431993bd-6e48-4fd8-be0b-bde62a2b1082')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to pdcats', CAST(N'2008-10-29T10:21:42.947' AS DateTime), N'5d2d9191-83ea-4ed5-8a46-0aabb8190392', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'4e7b5ef2-a331-4c2d-8a50-deab66de28cf')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to subcats', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'c54c2890-da43-4069-8c40-4f2438e18c5c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to countries', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'eba4c334-4285-4d14-abbb-1839c08f543f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to currencies', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'850422ea-ad71-4cef-b6af-227933bf8065', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'2023a744-ae21-4920-b20f-acf976ca8a66')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to reasons', CAST(N'2008-10-29T10:21:42.947' AS DateTime), N'83077e08-fe7d-4c1a-a306-be4327c349c1', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'ea8ac1d2-86a1-4c99-be51-3edcc714568d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to employees', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'6d92faa7-b255-4bc9-89ab-a0c3078db104')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to claims', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'cb8bf037-2098-4a58-8677-1a8f3c1ce533')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to car', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'8f5054fa-ab6a-48ee-87c4-01adeb699d6f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to groups', CAST(N'2008-10-29T10:21:42.930' AS DateTime), N'd6ab6ff4-0ec4-4996-8566-458b816adc0d', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'02dde9b4-43ac-44e2-81e5-1d87a1fe5474')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to savedexpenses-costcodes', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'bf13d307-6f4a-40cb-906d-72530f64791e', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'b0fe8da1-008e-4de8-85da-5fff938f554a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to costcodes', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'02009e21-aa1d-4e0d-908a-4e9d73ddfbdf', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'562542cd-d478-4dfc-8a84-7a1f0f251604')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to departments', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'a0f31cb0-16bb-4ace-aaea-69a7189d9599', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'254b8375-946f-4359-814d-21b4d002a876')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to employees_userdefined', CAST(N'2008-10-29T10:21:43.007' AS DateTime), N'972ac42d-6646-4efc-9323-35c2c9f95b62', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'bbf4de9d-d319-420d-8c71-81d607af371c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to savedexpenses_flags', CAST(N'2008-10-29T10:21:43.023' AS DateTime), N'ca5762a9-bdad-45a6-97f4-f86614fc5595', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'785a18e5-b386-4454-a744-0aa8950883ae')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to claims_userdefined', CAST(N'2009-11-26T06:31:58.643' AS DateTime), N'f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'869b5a9b-11ef-4cfa-a9d1-e59ac6759af4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to subcats_userdefined_values', CAST(N'2009-11-26T06:31:58.643' AS DateTime), N'82f57980-92c9-4a4b-b76d-2e8485c0bb41', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'897eed9e-6d20-4558-abc6-7598c6d75283')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Advances to project_codes', CAST(N'2008-10-29T10:21:43.023' AS DateTime), N'e1ef483c-7870-42ce-be54-ecc5c1d5fb34', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'b382266c-d16d-4e00-8ddb-ae86d770a405')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses - odometer readings', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'956dba2c-ea66-454e-ba53-d5fa1eeb82b7', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f7af8efb-2746-45e5-9f15-86ad463ad30b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to hotels', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'05bce98e-b6ec-44bf-a93d-a5039d838a11', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'19f706f3-d1f4-4bd5-ad58-bc19c53fa2da')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to hotel_reviews', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'63d77470-745f-4421-b169-cb86bf6f3557', N'f11254ce-b162-4b53-8645-f287143d7a98', N'ea884fba-ced6-4cce-ba91-57ddfafa372c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to savedexpenses', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f11254ce-b162-4b53-8645-f287143d7a98', N'232af8a0-43d6-4f70-806b-d45f024bf01f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to savedexpenses_flags', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'ca5762a9-bdad-45a6-97f4-f86614fc5595', N'f11254ce-b162-4b53-8645-f287143d7a98', N'c3aababc-96c4-49cf-84ac-96598786c1fc')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to savedexpenses_costcodes', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'bf13d307-6f4a-40cb-906d-72530f64791e', N'f11254ce-b162-4b53-8645-f287143d7a98', N'30d46173-4b3e-4b44-8cc9-65d6df3d2741')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to savedexpenses_userdefined', CAST(N'2009-11-26T06:31:58.643' AS DateTime), N'65394331-792e-40b8-af8b-643505550783', N'f11254ce-b162-4b53-8645-f287143d7a98', N'7aefb54e-0b12-4cac-ad4b-ea0a348ad118')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to subcats', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'f11254ce-b162-4b53-8645-f287143d7a98', N'07fe2922-4379-40ad-9141-3e1db59c7421')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to subcats_userdefined_values', CAST(N'2009-11-26T06:31:58.643' AS DateTime), N'82f57980-92c9-4a4b-b76d-2e8485c0bb41', N'f11254ce-b162-4b53-8645-f287143d7a98', N'c4926619-1b0d-4ed3-9d8c-fdaaf0d98b39')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to claims', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'f11254ce-b162-4b53-8645-f287143d7a98', N'84e8c95c-e7a5-4979-abc0-8c97e80c400a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to countries', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'f11254ce-b162-4b53-8645-f287143d7a98', N'3f6139c5-10d8-4b51-a911-dcd7486b25ac')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to currencies', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'850422ea-ad71-4cef-b6af-227933bf8065', N'f11254ce-b162-4b53-8645-f287143d7a98', N'89c41cb5-07e7-4c66-b1ba-8a3d75390623')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to reasons', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'83077e08-fe7d-4c1a-a306-be4327c349c1', N'f11254ce-b162-4b53-8645-f287143d7a98', N'dce7dcf0-3408-4aea-a4c1-63910c11f9f4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to employees', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'f11254ce-b162-4b53-8645-f287143d7a98', N'7859dcea-6883-497e-8aad-3007fa542f04')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to employees_userdefined', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'972ac42d-6646-4efc-9323-35c2c9f95b62', N'f11254ce-b162-4b53-8645-f287143d7a98', N'7e078639-1b76-4178-8bee-878cb0a00449')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to groups', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'd6ab6ff4-0ec4-4996-8566-458b816adc0d', N'f11254ce-b162-4b53-8645-f287143d7a98', N'82ca9bab-32a3-4afe-8132-31dc5e1d4fc4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to cars', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'f11254ce-b162-4b53-8645-f287143d7a98', N'23554fd5-453f-4c2c-919b-e809d8990520')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotes to costcodes', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'02009e21-aa1d-4e0d-908a-4e9d73ddfbdf', N'f11254ce-b162-4b53-8645-f287143d7a98', N'050afce2-102f-41e8-b293-a98bf4ff6102')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to departments', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'a0f31cb0-16bb-4ace-aaea-69a7189d9599', N'f11254ce-b162-4b53-8645-f287143d7a98', N'df378d9f-c637-49ec-a47b-fea87864c3aa')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'hotels to project codes', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'e1ef483c-7870-42ce-be54-ecc5c1d5fb34', N'f11254ce-b162-4b53-8645-f287143d7a98', N'0fee3718-b5f5-4d21-b016-60761dd21c11')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to rolesubcats', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'0123e0c5-5e68-4911-a062-9a6967d33beb', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'2004b1ef-6f50-4f29-9696-5385695db4f4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to card_statements', CAST(N'2009-11-26T06:31:58.647' AS DateTime), N'19931aa1-6287-4bd5-936c-1d7add367bf8', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'8b0461a9-d8b6-44e6-8475-6d905d2ab2b5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to project codes', CAST(N'2008-10-29T10:21:43.023' AS DateTime), N'e1ef483c-7870-42ce-be54-ecc5c1d5fb34', N'618db425-f430-4660-9525-ebab444ed754', N'f136b252-8e9a-4e3c-9dd5-7446cdaefe16')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to hotel_reviews', CAST(N'2008-10-29T10:21:43.040' AS DateTime), N'63d77470-745f-4421-b169-cb86bf6f3557', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'0e4b5f75-cd39-4209-92e7-20e085e9cac2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'countries to countrysubcats', CAST(N'2008-10-29T10:21:42.883' AS DateTime), N'eb45d0c0-0c45-462b-87b1-5d5c1b4f06ef', N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'6c1aa872-04bb-48d1-9446-bd7ea3ccfdb8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'countries to subcats', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'b06ac4e2-30f1-4cee-8fc9-2f224c9b2fc2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'budget holders to employees', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'e740e6dc-ec3e-4a19-810b-eac6abb7782f', N'53676e49-c211-4f0c-9827-346be6a666fd')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'groups to stages', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'8d50e31c-1f2d-4628-b207-4cd37e67f0c0', N'd6ab6ff4-0ec4-4996-8566-458b816adc0d', N'e28b6493-aa49-4c7f-9140-3e292756377c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'teams to employees', CAST(N'2008-10-29T10:21:42.990' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'fa495951-4d06-49ad-9f85-d67f9eff4a27', N'c8ee2aca-b3c5-4c07-84d4-24fbe19f1525')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'holidays to employees', CAST(N'2008-10-29T10:21:42.930' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'a6ff86d3-808f-406f-9dd6-e21b7b9a8d67', N'f5c957f8-191e-4546-aaa2-9d2be09c8cb3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'currencies to currencymonths', CAST(N'2008-10-29T10:21:42.883' AS DateTime), N'b19b43dd-084e-48ac-ac39-dfcf0e9599e0', N'850422ea-ad71-4cef-b6af-227933bf8065', N'2e1e4d5a-f4be-467e-8665-47271876767e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'currencies to monthly exchange rates', CAST(N'2008-10-29T10:21:43.053' AS DateTime), N'a1654a8d-e118-409f-9e57-4e17e54f346f', N'850422ea-ad71-4cef-b6af-227933bf8065', N'cd345fbb-95b6-4165-9d3e-f6c9cdc2f910')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'currencies to range currencyranges', CAST(N'2008-10-29T10:21:43.070' AS DateTime), N'deeae449-10da-4058-a4cc-b2a69d566466', N'850422ea-ad71-4cef-b6af-227933bf8065', N'fc0f4fd0-4d11-4259-8faa-22b951f14332')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'currencies to range_exchangerates', CAST(N'2008-10-29T10:21:43.070' AS DateTime), N'8e398b47-b867-4c36-8c2b-ac403abb2992', N'850422ea-ad71-4cef-b6af-227933bf8065', N'6c099634-0223-4ad5-8070-15746d8aabd2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'advances to savedexpenses', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'a23e1d57-22e7-41f3-9731-3c0066665dd1')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to claims', CAST(N'2008-10-29T10:21:42.913' AS DateTime), N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'618db425-f430-4660-9525-ebab444ed754', N'ba658355-2216-471e-bb52-a05bba5ae3dd')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to company details', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'cff57ca2-cd30-4feb-a9b9-a5cc9deb7357', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'86485f62-5684-410c-aba1-e0ec74e593db')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to company_bankdetails', CAST(N'2008-10-29T10:21:42.977' AS DateTime), N'c061eead-5e9e-4228-bbad-3873233fe225', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'5b45b04b-685b-4769-8445-1eff09dee619')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to employee_currencies', CAST(N'2008-10-29T10:21:43.087' AS DateTime), N'865350cb-002b-4826-b45e-93d50e48105b', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3599e163-c72c-414d-b3ff-d28998e6c074')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to global_currencies', CAST(N'2008-10-29T10:21:43.100' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'be9f2098-5052-4ff9-a498-87c037a81fc7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to global_currencies', CAST(N'2008-10-29T10:21:43.100' AS DateTime), N'b1e9111b-aa3c-4cc0-ad99-6c9e4b3e70fc', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'852e67d1-aa28-4aa2-af6f-e552435a688d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to savedexpenses_journey_steps', CAST(N'2008-10-29T10:21:43.100' AS DateTime), N'383db525-4559-4861-a7fc-b488ed9d13f0', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'b3f4ede7-78cf-4366-8752-95a1214bb8e7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Suppliers to supplier contact view', CAST(N'2008-11-11T09:41:02.807' AS DateTime), N'bcd8272e-3d91-42f1-b6d2-fb79f27176db', N'299ff396-6947-4d46-a4df-1983cd311a77', N'dbe33fa1-118e-4676-9050-69822e8d35f4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Suppliers to supplier categories', CAST(N'2008-11-11T09:41:02.823' AS DateTime), N'a9c7d7b7-ebed-4a25-bc5d-69d507afbe75', N'299ff396-6947-4d46-a4df-1983cd311a77', N'6f14b33b-571c-4ca6-9d7b-97501338c8ae')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Suppliers to supplier status', CAST(N'2008-11-11T09:41:02.823' AS DateTime), N'e8cde388-ef35-4349-b685-9d45da385ef1', N'299ff396-6947-4d46-a4df-1983cd311a77', N'08256a68-ad60-45d1-a439-812c39cc55d9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Suppliers to supplier addresses', CAST(N'2008-11-11T14:48:47.150' AS DateTime), N'e82fcac8-82c2-4055-aa19-203a00193307', N'299ff396-6947-4d46-a4df-1983cd311a77', N'b302859b-6746-4519-8551-e003d7aefe08')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'countries to global_countries', CAST(N'2009-03-04T12:16:15.940' AS DateTime), N'b1e9111b-aa3c-4cc0-ad99-6c9e4b3e70fc', N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'76cd7f8c-3530-407d-bd62-db9e683d7ec7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'countrysubcats to subcats', CAST(N'2009-03-07T17:36:30.457' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'eb45d0c0-0c45-462b-87b1-5d5c1b4f06ef', N'228c6a1d-b9f8-47e6-8f5c-36aee044199a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'countrysubcats to countries', CAST(N'2009-03-09T10:54:49.277' AS DateTime), N'0dc9ca2b-74c7-4c9b-ad1c-a66ae55f979d', N'eb45d0c0-0c45-462b-87b1-5d5c1b4f06ef', N'f0b7e563-2b02-4f03-a576-ecf8b6ab97fb')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'currencies to global_currencies', CAST(N'2009-03-17T11:29:01.423' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'850422ea-ad71-4cef-b6af-227933bf8065', N'8bf0e0d9-e614-4d72-938e-d1af179ebb16')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'currencymonths to currencies', CAST(N'2009-04-14T18:56:02.853' AS DateTime), N'850422ea-ad71-4cef-b6af-227933bf8065', N'b19b43dd-084e-48ac-ac39-dfcf0e9599e0', N'db0d88f9-85c3-4134-a7a2-f6739640f6df')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'currencyranges to currencies', CAST(N'2009-04-14T18:56:49.420' AS DateTime), N'850422ea-ad71-4cef-b6af-227933bf8065', N'deeae449-10da-4058-a4cc-b2a69d566466', N'2c5e6b9e-23d6-4193-a677-21be1441351b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract Category', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'20133759-fdb8-40d5-bd52-82450124168a', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'35403f3f-aa25-47c6-bca4-c77e7cf33a27')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract Status', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'8ceaa3fa-4b2c-4846-b988-22cc7e643d94', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'b5134a87-875a-42e1-9a94-159b719a27d4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract Type', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'53418d93-5c7b-4b14-b222-1f6bcbe59840', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'a2b77807-e6b2-4711-a504-328390dd1e0b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Inflator Metrics', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'85c8555c-0172-4feb-aa59-85f8607e4253', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'df5702ce-d40a-4015-92f0-81f48183bed0')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Term Types', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'3a5d47d7-7dcf-4388-b2f3-d385304eecac', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'a41f9cfd-3195-4f09-8b98-31b4a67b5740')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract History', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'8d9bea63-fe0a-4dba-85b7-6a13f3761039', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'150c1b81-62a9-4f0d-9cae-e5bf4e04459b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract Notes', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'7d3465ec-5101-4bf8-9c09-cd9568bb1224', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'34bba3b5-37b7-4738-b9d0-4845dd0e46ba')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract Notification', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'7d460193-893e-47ba-a9dc-f7ae0c330c71', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'ed6dcb44-7231-41a6-becf-7293350926ce')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Staff Details', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'7501abca-b6e0-4821-a2df-1ecfb3bb3d14', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'260bc87d-1148-4670-9c62-b257b5fb66f0')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Invoice Frequency', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'f6f78056-aea7-4089-b0dd-d39512aab2da', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'dcb52a34-d253-4854-9b2c-3ba91d043086')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Supplier Details', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'299ff396-6947-4d46-a4df-1983cd311a77', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'c63165e2-1390-47de-9be0-2a67c4b6debb')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract Products', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'f83a33b0-3666-4423-a768-9919d11b68fa')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Invoice Details', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'74477541-68d3-42eb-8b4b-eaa30a545125', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'7eb715ca-48cc-4f09-9d03-b36da89df633')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Invoice Forecasts', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'9c73b5da-ec86-438f-8513-524b6ab08c57', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'fa0dfac1-28b4-42d5-ae75-f1e98e0c39b8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Savings', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'bc23258a-a807-490a-b26c-be7514f4f965', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'59dc9e97-a213-47d8-8c2c-81e34eaf91e7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Products to Contract Product Information', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'90661a9b-eb58-40c1-8ba6-85b1c417ba3d', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'f87baa98-aa58-45ef-8422-edfd84e195e5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Sales Tax', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'e9734332-1e62-43c5-a8f2-14b518c87542', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'187a32ba-5dac-4930-a986-6bf7e051679d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Units', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'6ac80b0f-9c1f-43ea-9aed-6eb7104a7a89', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'02533820-314a-4e1d-9861-37d56532551a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Product Details', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'df81f60e-328b-4db4-82ae-d8da0a9126ba')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Product Category', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'fdc07b2d-1253-4b6d-a7e6-242242d958bc', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'ba4ee857-6bc1-4f3f-8448-99c2824be97b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Invoice Status', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'27f00143-8058-4108-a0ec-ac73d6964382', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'af7ffc4e-c37a-45e2-a55d-717681815300')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Sales Tax', CAST(N'2009-01-29T12:48:24.530' AS DateTime), N'e9734332-1e62-43c5-a8f2-14b518c87542', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'bfb4e07c-444c-4391-a365-7982216eeb1b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to Contact Notes', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'6fc17779-5a48-4eca-bdd1-c390a356350e', N'd13e1d64-5eb0-49eb-9dae-7c5a6a2ffd7a', N'7f8c6a8b-0ed0-453a-9561-3b2c94afe2fa')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Invoice Notes', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'41ac6048-f1b1-4347-a57f-87e755a2dc78', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'68e9967a-e47d-4076-af93-61e2c6815bab')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Annual_Cost_Summary View', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'884a43f6-f730-46b1-9458-346518707c3e', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'50eb1270-b342-4d01-978f-c9d17db207f2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Product Details', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'4531b8b3-19fd-44e3-b72e-1f8bffc03b04')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Product Information', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'90661a9b-eb58-40c1-8ba6-85b1c417ba3d', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'f2caab95-ca16-4f78-8ec9-8471b471bf0b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Codes_ProductCategory', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'fdc07b2d-1253-4b6d-a7e6-242242d958bc', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'16afb420-4578-42ae-9538-6b73734185df')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Product Category', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'fdc07b2d-1253-4b6d-a7e6-242242d958bc', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'2a779b59-27b9-495f-a979-aced3b8be991')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Contract Product Details', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'187121b2-4f19-4d1e-a5c4-8dcf2fa10b7a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Recharge Payments', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'59695c3d-614d-4396-a0e6-6409d902875a', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'82101254-c239-4945-a21b-b8804ef6bed9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Products to Recharge Payments', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'59695c3d-614d-4396-a0e6-6409d902875a', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'632fec00-d242-4490-a64c-9dbadc70ae1e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Products to Contract Details', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'2295fa3a-4247-4928-a65a-5096285bb297')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Product Notes', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'e8a273ca-dd71-4b50-8d6b-ba5eb6c83af1', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'ff1937ea-1291-4805-a62d-9913cab94788')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Products to Product Notes', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'e8a273ca-dd71-4b50-8d6b-ba5eb6c83af1', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'96c85724-5a49-45db-9d08-d026789bf237')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Contract Details', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'0b045615-b04a-4b0d-b54a-d91b3d9197f2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Forecasts to Contract Details', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'9c73b5da-ec86-438f-8513-524b6ab08c57', N'ec288752-a902-4f63-85df-7a076797e777')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Forecasts to Contract Product Details', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'9c73b5da-ec86-438f-8513-524b6ab08c57', N'2c1094a8-f74c-468f-908a-330952fd2969')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Licence Renewal Type', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'6f291ba0-d13e-43db-bbea-3afbceda0570', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'a4e1b590-7950-43f5-8c84-ad388f7b2a1b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Licence Renewal Type', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'6f291ba0-d13e-43db-bbea-3afbceda0570', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'b2c9c689-ab66-4e21-8387-b51a2194298a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Licence Renewal Type', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'6f291ba0-d13e-43db-bbea-3afbceda0570', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'7690ec93-d795-4d2b-a6a1-5fb214ec9089')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Staff Details', CAST(N'2009-01-29T12:48:24.543' AS DateTime), N'7501abca-b6e0-4821-a2df-1ecfb3bb3d14', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'228455da-6b93-4275-a697-6378280b3a9a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Invoice Status Type', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'27f00143-8058-4108-a0ec-ac73d6964382', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'36d37fd9-bf8d-4e40-a983-a74edeb34f15')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Primary Note Category', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'5498efdd-3f8c-4e51-8acf-39e5afc33d40', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'e67f3992-51fb-4bdd-90bf-43f072fc3824')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Secondary Note Category', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'b0e75fa5-e039-43cc-a56c-6e43946f444f', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'a8d16254-15c8-405c-b4db-df09c917c17b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Product Licences', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'32d1e316-d501-47b2-9bb2-aa70646e3231', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'51d80686-014e-487e-a216-60de434f31aa')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Primary Note Category', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'5498efdd-3f8c-4e51-8acf-39e5afc33d40', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'8e232348-65f8-485b-9bec-9415f1b4926f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Secondary Note Category', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'b0e75fa5-e039-43cc-a56c-6e43946f444f', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'b173383c-d40b-45da-9af8-b2915d5b62ff')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to Primary Note Category', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'5498efdd-3f8c-4e51-8acf-39e5afc33d40', N'd13e1d64-5eb0-49eb-9dae-7c5a6a2ffd7a', N'908d1888-f48a-4c87-b1f4-9b790d17b010')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to Secondary Note Category', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'b0e75fa5-e039-43cc-a56c-6e43946f444f', N'd13e1d64-5eb0-49eb-9dae-7c5a6a2ffd7a', N'8a7f7107-7992-4754-a643-ad2927985b61')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Units', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'6ac80b0f-9c1f-43ea-9aed-6eb7104a7a89', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'4475b069-7a66-43e4-9e6f-e5da2c82b790')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Product Licences', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'32d1e316-d501-47b2-9bb2-aa70646e3231', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'c0ee30b2-7341-4528-884a-0bffd70b7a43')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Product Licences', CAST(N'2009-01-29T12:48:24.560' AS DateTime), N'32d1e316-d501-47b2-9bb2-aa70646e3231', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'08b0575a-aecb-4830-96e3-b1bcc8399dfe')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Contract Types', CAST(N'2009-03-04T15:38:53.327' AS DateTime), N'53418d93-5c7b-4b14-b222-1f6bcbe59840', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'0b9c0384-bee9-40c1-a385-789061d1678e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Contract Categories', CAST(N'2009-03-04T15:38:53.327' AS DateTime), N'20133759-fdb8-40d5-bd52-82450124168a', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'd329bb44-f401-4bcb-9ae8-07c3a4c77378')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Document Merge Projects to Document Merge Sources', CAST(N'2009-03-04T15:38:53.327' AS DateTime), N'be95ceac-68c2-47af-b2ea-799303ab45ed', N'1d3ef4de-3f3f-4e23-bc29-779f502cca9a', N'8ef9ecbe-984e-4fd5-bf47-20fd7ab8d7a7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Document Merge Sources to Reports', CAST(N'2009-03-06T11:54:53.247' AS DateTime), N'7bc82041-8455-4e7b-9773-008e06adf7c1', N'be95ceac-68c2-47af-b2ea-799303ab45ed', N'c84b8afe-8d90-4379-b512-77c1749cbd24')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Purchase Orders to Suppliers', CAST(N'2009-03-04T15:34:33.860' AS DateTime), N'299ff396-6947-4d46-a4df-1983cd311a77', N'4efee2c2-4404-4370-8844-26a1fd602edb', N'f9f76ea3-ae5a-41d9-bd3c-109dcd2c74b3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'purchaseOrders to employees', CAST(N'2009-03-14T18:13:24.827' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'4efee2c2-4404-4370-8844-26a1fd602edb', N'68013e21-785a-48bc-ac10-1046b4946f6a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employee Corporate Cards to Card Providers', CAST(N'2009-05-28T11:50:04.583' AS DateTime), N'78c92d47-2cf1-4970-93f7-5dc972683f88', N'9f3aa3ed-481d-499a-89b3-f1c8aefa61e5', N'0d340de1-dbab-4fa6-aec3-4c6a0f76e5f8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employee Pool Cars to Cars', CAST(N'2009-06-03T12:47:03.363' AS DateTime), N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'4d36cfab-1e35-44b8-9546-a5433e0517ec', N'9e9aa130-a67c-4afb-91fb-bbe996068420')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Item Roles to Rolesubcats', CAST(N'2009-06-09T16:47:58.297' AS DateTime), N'0123e0c5-5e68-4911-a062-9a6967d33beb', N'db7d42fd-e1fa-4a42-84b4-e8b95c751bda', N'1fc6809f-a7d7-4ffd-ad80-0d9ac8d4284b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Subcat_split to subcats', CAST(N'2009-06-11T14:06:56.380' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'9fe1aa3f-8e0e-4ca6-a31e-86e4c5484a4a', N'5de84270-6d62-4342-bc64-f7e9f490ad88')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'subcat_split to categories', CAST(N'2009-06-11T14:10:43.387' AS DateTime), N'75c247c2-457e-4b14-bbec-1391cd77fb9e', N'9fe1aa3f-8e0e-4ca6-a31e-86e4c5484a4a', N'ded97af3-cc65-4805-b611-96e0af9ba561')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'ESR Elements to Global ESR Elements', CAST(N'2009-06-24T15:55:58.310' AS DateTime), N'468e228e-3989-407d-9a5e-f942fc8a0f71', N'06f622dc-3785-446d-a5a3-10c9c0f8a1b1', N'ab70783e-8b55-4530-856f-9071845ce8d8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Import Templates to ESR Trusts', CAST(N'2009-06-26T15:04:26.410' AS DateTime), N'c7009e76-4093-41ea-ad86-823876a95b5c', N'9c995f3d-f01e-4d1e-b965-7066644b5103', N'9a158653-49c0-45fa-921e-092ffafe5edb')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Mileage Categories to global currencies', CAST(N'2009-06-28T13:42:26.590' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'5a83aeaf-86c8-48fb-aa2b-e7ab05a74a0b', N'cad575c8-d48b-4296-9e13-fc67c0a4eac3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Financial Export to Reports', CAST(N'2009-07-05T16:35:41.570' AS DateTime), N'7bc82041-8455-4e7b-9773-008e06adf7c1', N'64e82627-2fa9-4ebb-9fed-bdaae20e484d', N'c086685c-aeae-4aa7-9d49-c86f247c2365')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Financial Export to Employees', CAST(N'2009-07-05T22:57:56.120' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'64e82627-2fa9-4ebb-9fed-bdaae20e484d', N'fda87476-3fe3-446b-b751-5f517b294c87')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Export History to Employees', CAST(N'2009-07-06T00:03:49.377' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'17dffe0a-9ea6-439a-8763-1e3907334956', N'3f57ff52-9d27-42d2-bda6-1fa5f4fe84b8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Export History to Financial Exports', CAST(N'2009-07-06T11:09:47.587' AS DateTime), N'64e82627-2fa9-4ebb-9fed-bdaae20e484d', N'17dffe0a-9ea6-439a-8763-1e3907334956', N'86b550e0-4387-4c56-8ebf-a96fb7a956cb')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to esr_assignments', CAST(N'2009-07-08T10:10:01.443' AS DateTime), N'bf9aa39a-82d6-4960-bfef-c5943bc0542d', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'7ca01694-df17-4113-9375-9a04d87424f2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audiences to Teams', CAST(N'2009-07-09T10:58:11.133' AS DateTime), N'fa495951-4d06-49ad-9f85-d67f9eff4a27', N'506b66cc-d096-4f00-9adf-ba03579d84fb', N'01e0e268-6e5c-46aa-ad7f-4265934dc5b1')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audiences to Employees', CAST(N'2009-07-09T10:58:14.957' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'506b66cc-d096-4f00-9adf-ba03579d84fb', N'67ab0507-6c5d-4f39-84c2-14f8998441c3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audiences to Budget Holders', CAST(N'2009-07-09T10:58:19.127' AS DateTime), N'e740e6dc-ec3e-4a19-810b-eac6abb7782f', N'506b66cc-d096-4f00-9adf-ba03579d84fb', N'd0660be2-677f-4769-83b0-d9ecf56a866f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audiences to Audience Teams', CAST(N'2009-07-09T15:12:27.110' AS DateTime), N'fdfb79d6-81b0-46c3-9c9a-0d32f776132e', N'506b66cc-d096-4f00-9adf-ba03579d84fb', N'8f89c0c6-70ca-40e2-b46d-183481342d9e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audiences to Audience Budget Holders', CAST(N'2009-07-09T15:12:55.437' AS DateTime), N'b2a352fd-5f5a-4649-adb1-efc117fcb305', N'506b66cc-d096-4f00-9adf-ba03579d84fb', N'c7151e70-784c-46d0-812f-3114ca77b387')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audiences to Audience Employees', CAST(N'2009-07-09T15:13:21.870' AS DateTime), N'8d7d8a57-8822-4735-a26e-851daa0141e6', N'506b66cc-d096-4f00-9adf-ba03579d84fb', N'4318ca55-dc2c-4206-8e79-6e188101be4f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audience Teams to Teams', CAST(N'2009-07-09T15:24:35.753' AS DateTime), N'fa495951-4d06-49ad-9f85-d67f9eff4a27', N'fdfb79d6-81b0-46c3-9c9a-0d32f776132e', N'32eb1011-82d0-4925-a307-99d483d962e9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audience Budget Holders to Budget Holders', CAST(N'2009-07-09T15:25:09.820' AS DateTime), N'e740e6dc-ec3e-4a19-810b-eac6abb7782f', N'b2a352fd-5f5a-4649-adb1-efc117fcb305', N'242bc6a9-d021-4c76-adcd-dcd0766d80a1')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audience Employees to Employees', CAST(N'2009-07-09T15:26:11.197' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'8d7d8a57-8822-4735-a26e-851daa0141e6', N'55b9bc0b-3f76-4d34-ad09-f0332e13209f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Audience Budget Holders to Employees', CAST(N'2009-07-09T16:26:54.593' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'b2a352fd-5f5a-4649-adb1-efc117fcb305', N'5056ef76-140d-46e5-a46e-0820dae49088')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to userdefinedExpenses', CAST(N'2009-07-11T15:44:24.820' AS DateTime), N'65394331-792e-40b8-af8b-643505550783', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'596fb4e3-a33d-4ac7-8117-94d89ba88aca')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to userdefinedCars', CAST(N'2009-07-11T15:46:59.347' AS DateTime), N'7e9e6bee-f8ca-45d8-b914-1a9b105e47b2', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'b71a4b5e-fd4a-452f-9525-6cd89bf0ed0f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to userdefinedCars', CAST(N'2009-07-11T15:53:13.037' AS DateTime), N'7e9e6bee-f8ca-45d8-b914-1a9b105e47b2', N'618db425-f430-4660-9525-ebab444ed754', N'00a692cb-2190-43d1-9f2c-5a24d99c7d21')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Team Employees to Employees', CAST(N'2009-07-16T19:47:38.317' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'd99f21d9-64fa-4a4b-8db6-8f2a6df3b857', N'a612a371-4321-43c7-8f99-c26705e2ba23')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Teams to Team Employees', CAST(N'2009-07-16T19:56:12.000' AS DateTime), N'd99f21d9-64fa-4a4b-8db6-8f2a6df3b857', N'fa495951-4d06-49ad-9f85-d67f9eff4a27', N'24df0364-42b9-429e-84de-d784016a497d')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Cost Codes to Userdefined_CostCodes', CAST(N'2009-07-27T14:18:27.763' AS DateTime), N'e4cca1ba-a065-4116-860b-abaa1e7bb2ef', N'02009e21-aa1d-4e0d-908a-4e9d73ddfbdf', N'065a9f9f-d8f9-4316-a5ad-e030703b6ad1')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Departments to UserdefinedDepartments', CAST(N'2009-07-27T14:28:35.403' AS DateTime), N'155ae388-1b60-4fb2-a1bd-c46f543fa401', N'a0f31cb0-16bb-4ace-aaea-69a7189d9599', N'53886291-1d3e-43d1-b04e-d6a61b8ca129')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Project Codes to UserdefinedProjectCodes', CAST(N'2009-07-27T14:35:50.813' AS DateTime), N'ce235f78-82c6-4ba1-8845-034a015c5dca', N'e1ef483c-7870-42ce-be54-ecc5c1d5fb34', N'e88bc5a2-919d-4788-9df8-8ebe8a861bbb')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Pool Car Users to Employees', CAST(N'2009-08-03T16:33:24.903' AS DateTime), N'618db425-f430-4660-9525-ebab444ed754', N'4d36cfab-1e35-44b8-9546-a5433e0517ec', N'964dc947-bdc9-4861-9aa3-97420ecb382f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employee Roles to Item Roles', CAST(N'2009-08-12T10:37:23.573' AS DateTime), N'db7d42fd-e1fa-4a42-84b4-e8b95c751bda', N'8211e41f-710e-42ab-a2df-1574fd003b31', N'5bce1205-892e-44e8-be87-0cc1a17c075f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'userdefinedExpenses to savedexpenses', CAST(N'2009-08-20T10:09:20.753' AS DateTime), N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'65394331-792e-40b8-af8b-643505550783', N'72f350c1-693c-4981-9fe8-fece541756c9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to employee_costcodes', CAST(N'2009-09-01T10:18:52.640' AS DateTime), N'04acc980-73a7-4866-8162-7b244679c676', N'618db425-f430-4660-9525-ebab444ed754', N'25a59e97-0626-455c-923b-0faf5b5210d2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to mileage_categories', CAST(N'2009-09-04T19:09:20.483' AS DateTime), N'5a83aeaf-86c8-48fb-aa2b-e7ab05a74a0b', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'56e14981-06e5-44ae-9580-91c0b5dc700f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to mileage date ranges', CAST(N'2009-09-07T14:12:10.390' AS DateTime), N'd1ee2cbb-605d-4681-a46b-e626c03581f0', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3794758c-6c85-4d1c-93fe-97d670cc0e99')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to mileage thresholds', CAST(N'2009-09-07T14:12:56.007' AS DateTime), N'7c169a27-6352-4447-a8df-245d718230e9', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'43d648bb-39b1-4532-9ab7-d2b016c2a975')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to subcat_vat_rates', CAST(N'2009-09-09T07:03:25.470' AS DateTime), N'14b0d560-44c7-4811-a94d-da1e7ed9c325', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'2caf3f63-188e-4c97-b995-cebcb3a776ca')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to access roles', CAST(N'2009-11-26T06:31:58.647' AS DateTime), N'12ded231-b220-4acb-a51d-896c52ff8979', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'4de52375-1b8b-4332-b75d-c0eec6087a14')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'advances to global_currencies', CAST(N'2009-11-26T06:31:58.650' AS DateTime), N'4c948c64-f145-4bf4-be56-8014e9004f3c', N'3e07c14c-6ba1-473d-bd7f-230444d73fc9', N'0cec3728-0a12-4bc8-b368-9caacd45925b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to esr_assignmnets', CAST(N'2009-09-14T10:50:12.983' AS DateTime), N'bf9aa39a-82d6-4960-bfef-c5943bc0542d', N'618db425-f430-4660-9525-ebab444ed754', N'7d447ed0-9e19-4b12-b30e-95a64d1f594a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to userdefined costcodes', CAST(N'2009-09-14T13:45:29.177' AS DateTime), N'e4cca1ba-a065-4116-860b-abaa1e7bb2ef', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f3eafff0-5a04-462f-b802-eed62417c933')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to mileage_categories', CAST(N'2009-09-17T07:58:22.370' AS DateTime), N'5a83aeaf-86c8-48fb-aa2b-e7ab05a74a0b', N'618db425-f430-4660-9525-ebab444ed754', N'2f934a1d-ef7b-4dfc-a9e0-0f06e06d2d09')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to card providers', CAST(N'2009-11-26T06:31:58.650' AS DateTime), N'78c92d47-2cf1-4970-93f7-5dc972683f88', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'727d62c3-cabc-4d2e-9fd6-b41301859844')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'savedexpenses to userdefined projectcodes', CAST(N'2009-10-05T11:18:15.053' AS DateTime), N'ce235f78-82c6-4ba1-8845-034a015c5dca', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'3c180102-40db-4ca8-bb79-08df0d39a6b7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'claims to claimhistory', CAST(N'2009-11-26T06:31:58.650' AS DateTime), N'c8dbf7c8-6ed7-4872-be06-5b3af3b02c5a', N'0efa50b5-da7b-49c7-a9aa-1017d5f741d0', N'faac3554-562d-4b95-8c0c-82e581e65846')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedexpenses to countrysubcats', CAST(N'2009-11-26T06:31:58.650' AS DateTime), N'eb45d0c0-0c45-462b-87b1-5d5c1b4f06ef', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'bfecca9f-ba89-49fd-9472-6d3b1d9707c6')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to Suppliers', CAST(N'2010-01-07T11:23:45.097' AS DateTime), N'299ff396-6947-4d46-a4df-1983cd311a77', N'bcd8272e-3d91-42f1-b6d2-fb79f27176db', N'80b5dbcd-ea84-40cc-acd0-340b92e9f507')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'flagAssociatedRoles to Item Roles', CAST(N'2011-04-26T20:22:01.660' AS DateTime), N'db7d42fd-e1fa-4a42-84b4-e8b95c751bda', N'fea2c660-25e6-487f-b65c-0b2a04bc55dc', N'a5975b2f-07c3-47e1-9113-3094fde289c6')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'flagAssociatedExpenseItems', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'afb7d2b2-04a1-4951-978d-c7236529f1dd', N'fb6d329e-c89b-41ea-b879-bb68ae49e269')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'employees to esrTrusts', CAST(N'2010-03-30T11:42:03.267' AS DateTime), N'c7009e76-4093-41ea-ad86-823876a95b5c', N'618db425-f430-4660-9525-ebab444ed754', N'56ceb679-b7e4-42f0-8002-8b7dca7128b4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'flagsaccociatedfields to fields', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'5b32610e-35db-492a-b6d1-5f392ca4c040', N'75cf2927-f03b-4939-8e3b-0a9ecf22cf59', N'76b60a10-d571-4b75-905c-d6fd9fe009a2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedexpenses to esrTrusts', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'c7009e76-4093-41ea-ad86-823876a95b5c', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'337ce60f-dea6-46e8-9056-37d4c7818fbb')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'custom_fields to tables', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'30195a8c-0eb3-4190-84c9-7c8423f4fe64', N'4ba6567d-b23e-48bd-846f-0f8a9d473fee', N'd3566169-2e62-4621-b4e1-e45cf82042ac')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'mimeTypes to globalMimeTypes', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'd0709f39-1b5b-4917-998a-8c0ba8eafe7c', N'aef1b759-2a32-4b0d-86e3-e17a0dfa4bf9', N'e2bc638e-6d40-4945-b3c6-fe4534dc55fc')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to User Defined Contract Details', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'a5508ef4-d0ee-48b1-b9b4-19473d133f98', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'37161cc7-4f25-43a9-8618-02ce6561771f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to User Defined Contract Product Details', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'04f351a6-f7ff-4b39-a782-1e8317567e57', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'9c40ea49-4e4a-4d53-a066-01fd5e9d80a8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to User Defined Contract Product Details', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'04f351a6-f7ff-4b39-a782-1e8317567e57', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'670576ba-7b59-4a4b-a730-4cca7f8abb3a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to User Defined Product Details', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'9353859b-cf95-4a58-863c-5a134e16c5f5', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'9c820e27-eaf0-4a34-8139-485967e56534')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to User Defined Supplier Details', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'1371141b-67d7-4d46-b9b6-8e37dee3c1c8', N'299ff396-6947-4d46-a4df-1983cd311a77', N'daeb1ad9-ef57-4b8c-9a05-482604bb5bfc')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to User Defined Supplier Contacts', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'd731594c-fed5-49de-98c4-650297aa7219', N'bcd8272e-3d91-42f1-b6d2-fb79f27176db', N'a5940338-b9c6-42d2-a712-a04859fd0a7a')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Tasks to Task Types', CAST(N'2011-04-26T20:22:01.787' AS DateTime), N'bd9b3bc1-54b6-4c93-87bc-16920f11f9c9', N'308188ca-e5ee-4769-a25a-7d19cfbb8423', N'99e1baa0-198e-46fe-8029-eae86fad01a8')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'userdefinedGroupings to tables', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'30195a8c-0eb3-4190-84c9-7c8423f4fe64', N'd0c6f76d-a79e-484f-af29-4252cb239490', N'd239b449-8bc4-417f-ad1e-fb9b9d151b33')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Products to Contract Details userdefined', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'a5508ef4-d0ee-48b1-b9b4-19473d133f98', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'ea3c8ece-a6ca-44ab-94ec-f72b8581bbb5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Suppliers to Contract Details', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'299ff396-6947-4d46-a4df-1983cd311a77', N'56566c7a-c3e1-4bab-8915-dd792160f96b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Suppliers to Contract Details User Defined', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'a5508ef4-d0ee-48b1-b9b4-19473d133f98', N'299ff396-6947-4d46-a4df-1983cd311a77', N'902f2524-e5f9-462f-b269-74aad4ff5684')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Supplier Notes View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'89150c5d-d60a-4581-996f-5a816ea40b21', N'299ff396-6947-4d46-a4df-1983cd311a77', N'2f92b4ff-ce70-44c8-bf2d-f2568fcf4ea5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Supplier Notes', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'66d82bd0-68a8-4657-90c5-a5110c1ac976', N'299ff396-6947-4d46-a4df-1983cd311a77', N'e802b450-328f-46dc-a41a-85d0baf20bc2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Product Notes View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'527c558c-42f3-4cdb-9c28-442685ad2e66', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'ee34ed2a-7b95-4f66-87de-4006d26a2591')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Contract Notes View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'5d69fa65-d521-45c0-8dbb-221b70c0c79e', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'0b7081b7-11f2-41aa-b082-9bf030bd6d32')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Invoice Notes View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'4d57269c-1054-46bb-9055-7a75ef080c91', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'96333863-9d61-4897-97b5-ba12939fc701')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Product Notes', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'e8a273ca-dd71-4b50-8d6b-ba5eb6c83af1', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'610baa3b-1d4f-42b7-9c28-ed1048aed9ac')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Product Notes View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'527c558c-42f3-4cdb-9c28-442685ad2e66', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'b0a68533-2110-466b-bff1-b9eba0075832')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to Contact Notes View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'082b486c-89fd-4a37-b211-b28fa5bb21b0', N'bcd8272e-3d91-42f1-b6d2-fb79f27176db', N'1ce3b289-e218-4ef5-a003-36af8196b905')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Contacts to Contact Notes', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'6fc17779-5a48-4eca-bdd1-c390a356350e', N'bcd8272e-3d91-42f1-b6d2-fb79f27176db', N'fc11d0e6-576b-4894-be9b-e47cdfad2a12')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Contract Details', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'30a65283-6d82-44a0-9480-c6eb91170e06')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Userdefined to tables', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'30195a8c-0eb3-4190-84c9-7c8423f4fe64', N'f07e080b-f2ae-452b-90b3-7d2530baae17', N'74e6660a-ae1a-4296-baeb-99f7873b9a72')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Contract Notes', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'7d3465ec-5101-4bf8-9c09-cd9568bb1224', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'3c6bad51-6dcc-4f6e-8aaa-b8e225bd93be')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Contract Notes', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'7d3465ec-5101-4bf8-9c09-cd9568bb1224', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'93c7ac64-ae39-4cb0-86e1-cededc57fac7')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Contract History', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'8d9bea63-fe0a-4dba-85b7-6a13f3761039', N'299ff396-6947-4d46-a4df-1983cd311a77', N'fe59a8d9-c6d1-4215-b79a-4f8a76731d89')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Contract Savings', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'bc23258a-a807-490a-b26c-be7514f4f965', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'651564bf-a696-4b90-b152-a726054a9648')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Contract History', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'8d9bea63-fe0a-4dba-85b7-6a13f3761039', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'c2dc7e99-1d6b-4fe8-9b1e-e5c582f3d7d2')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Userdefined to userdefinedInformation View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'eacd0e78-7fe6-431d-bd77-08c3b4cb18fb', N'f07e080b-f2ae-452b-90b3-7d2530baae17', N'd7850daf-ec3f-4936-83ac-4fa7a61be015')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Products to Supplier Details', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'299ff396-6947-4d46-a4df-1983cd311a77', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'ecd63158-a3d4-4e4e-bcaa-7f7dda94b7ea')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Supplier Contact Notes View', CAST(N'2011-04-26T20:22:01.790' AS DateTime), N'082b486c-89fd-4a37-b211-b28fa5bb21b0', N'299ff396-6947-4d46-a4df-1983cd311a77', N'c1c3bc7a-0020-4da8-9c99-ea124bf93d18')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Supplier Contact Notes', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'6fc17779-5a48-4eca-bdd1-c390a356350e', N'299ff396-6947-4d46-a4df-1983cd311a77', N'7922b598-0447-4081-8c3e-7c182edd08fd')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Forecast Details to Contract Status', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'8ceaa3fa-4b2c-4846-b988-22cc7e643d94', N'9c73b5da-ec86-438f-8513-524b6ab08c57', N'a4af5dd6-36e7-4634-be60-366291a9ba90')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Forecast Details to Contract Category', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'20133759-fdb8-40d5-bd52-82450124168a', N'9c73b5da-ec86-438f-8513-524b6ab08c57', N'bc927c60-bfbf-4082-bcf6-32a651153868')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Forecast Details to Supplier Details', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'299ff396-6947-4d46-a4df-1983cd311a77', N'9c73b5da-ec86-438f-8513-524b6ab08c57', N'affc93ab-8d18-48d3-816c-6e662422c5b5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Contract Details UserDefined', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'a5508ef4-d0ee-48b1-b9b4-19473d133f98', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'921e1acb-0402-46b2-b563-1d2218637b16')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Supplier Userdefined', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'1371141b-67d7-4d46-b9b6-8e37dee3c1c8', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'b490c79a-486e-436f-8451-7a0411fa82c4')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Contract Status', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'8ceaa3fa-4b2c-4846-b988-22cc7e643d94', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'a719f63e-b027-48f6-85be-9e3f0ec18b14')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Contract Category', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'20133759-fdb8-40d5-bd52-82450124168a', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'135e7ac0-a90d-451d-953f-8d55a1f0c254')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Invoice Details to Contract Type', CAST(N'2011-04-26T20:22:01.793' AS DateTime), N'53418d93-5c7b-4b14-b222-1f6bcbe59840', N'74477541-68d3-42eb-8b4b-eaa30a545125', N'600ed34d-4123-4fbf-9e25-e4950bf3f936')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'contract details to supplier contacts', CAST(N'2011-04-26T20:22:01.797' AS DateTime), N'bcd8272e-3d91-42f1-b6d2-fb79f27176db', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'a670bee3-a9bc-4dde-9609-c8e230371b0f')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Licences to Userdefined Product Licences', CAST(N'2011-04-26T20:22:01.797' AS DateTime), N'9b38866a-6786-4064-b77c-10d3948e0994', N'32d1e316-d501-47b2-9bb2-aa70646e3231', N'a5678a01-d373-4674-94c0-65a96060fa08')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Product Licence Userdefined', CAST(N'2011-04-26T20:22:01.797' AS DateTime), N'9b38866a-6786-4064-b77c-10d3948e0994', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'a5a95b14-8eaf-40bb-97e3-7436c77b3b0e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Userdefined Product Licence Fields', CAST(N'2011-04-26T20:22:01.797' AS DateTime), N'9b38866a-6786-4064-b77c-10d3948e0994', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'30da3884-d7c3-4e1a-b903-9fe589545302')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Details to Userdefined Product Licence Fields', CAST(N'2011-04-26T20:22:01.797' AS DateTime), N'9b38866a-6786-4064-b77c-10d3948e0994', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'd25216ad-938f-4837-bd17-e032959a8c27')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedexpenses to Employee MileageTotals', CAST(N'2011-06-07T12:46:29.800' AS DateTime), N'6323a27f-c737-4799-844a-680a8c663701', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'348e2354-27a4-42f4-b31c-d69686c772f5')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'SavedExpenses to Savedexpenses_Subcat_Countries', CAST(N'2011-07-20T12:44:59.493' AS DateTime), N'bee8684b-6eae-47f1-97a9-166e9e790b95', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'fb63e2cb-ee9b-444c-a24c-525c0b72c52b')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Codes Contract categories', CAST(N'2011-08-06T06:20:53.107' AS DateTime), N'20133759-fdb8-40d5-bd52-82450124168a', N'299ff396-6947-4d46-a4df-1983cd311a77', N'f5d4b486-72b9-4544-b77f-f00c62d044a3')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employees to AccountsSubAccounts', CAST(N'2011-08-06T06:20:53.107' AS DateTime), N'be306290-d258-4295-b3f2-5b990396be20', N'618db425-f430-4660-9525-ebab444ed754', N'dae41276-e31a-4b48-9c4e-77c1042d1530')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Subcats to UserdefinedSubcats', CAST(N'2011-11-22T08:40:52.050' AS DateTime), N'82f57980-92c9-4a4b-b76d-2e8485c0bb41', N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'8d90963f-b93a-45ac-bc84-bdb6e56b93dd')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedsexpenses to Card Statements', CAST(N'2012-06-18T15:27:23.363' AS DateTime), N'05c465b6-45fb-4bd8-8c48-086b370b834e', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'36a0e266-75fa-4702-8b8b-7b10a98890ce')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'mobileExpenseItems to subcats', CAST(N'2012-03-24T07:02:19.720' AS DateTime), N'401b44d7-d6d8-497b-8720-7ffcc07d635d', N'5e4cba58-d747-45b1-b2df-8f7c218fed18', N'949a5caa-f999-483a-9d01-fb5dd494462e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'mobileExpenseItems to reasons', CAST(N'2012-03-24T07:02:19.813' AS DateTime), N'83077e08-fe7d-4c1a-a306-be4327c349c1', N'5e4cba58-d747-45b1-b2df-8f7c218fed18', N'c25aaed3-6b10-4c51-bbf8-30e9fd92a97c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Employees to employeeaccessroles', CAST(N'2011-10-21T17:42:54.880' AS DateTime), N'f26c8337-98b8-45a3-adca-9549287a3610', N'618db425-f430-4660-9525-ebab444ed754', N'56cfa36f-74ab-4652-babc-03f6dd9f6f07')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedsexpenses to Card Card Transactions RBS Purchase', CAST(N'2012-06-20T08:01:15.767' AS DateTime), N'3615128f-3c02-4909-bd2d-32833fba15d0', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'5cfbe289-555e-40ba-92e1-b596948e3498')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedsexpenses to Card Card Transactions RBS Credit', CAST(N'2012-06-21T08:09:50.007' AS DateTime), N'd65be15e-229e-4c05-8c9f-53a3107b14f4', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'e5e6bd20-7fe0-4376-a567-575c34cbf53e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Savedsexpenses to Card Card Transactions ', CAST(N'2012-06-27T20:10:09.967' AS DateTime), N'3c273833-fb08-4870-8184-df8197d82221', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'f2cf7536-206c-43f0-b2eb-f942c3d61aa9')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Supplier Details to Userdefined Supplier Contacts', CAST(N'2012-06-30T06:25:56.633' AS DateTime), N'd731594c-fed5-49de-98c4-650297aa7219', N'299ff396-6947-4d46-a4df-1983cd311a77', N'dece78e1-ed8a-4bb3-81c2-5b9d1255f652')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract details to Userdefined Product details', CAST(N'2012-06-30T06:25:56.640' AS DateTime), N'9353859b-cf95-4a58-863c-5a134e16c5f5', N'998e51fa-2c23-467e-b90f-75c44d1838bc', N'c469feee-eda3-4f95-9375-7869e0e5b28c')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Contract Product Details to Userdefined Product Details', CAST(N'2012-06-30T06:25:56.640' AS DateTime), N'9353859b-cf95-4a58-863c-5a134e16c5f5', N'35c52038-f99b-4feb-85c2-5d1755c91ca8', N'bc715054-3cf9-4e44-8779-be7125f45d87')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Product Details to Userdefined Contract Product Details', CAST(N'2012-06-30T06:25:56.643' AS DateTime), N'04f351a6-f7ff-4b39-a782-1e8317567e57', N'676df92b-386a-4e39-be7d-a54ab7d6d168', N'b3a5ddc1-45b2-4650-b4ad-2148efa6f0af')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Document Merge Associations Link to Document Templates', NULL, N'e826900f-9ed5-49f7-aae1-31ee5cb92945', N'5531e4dc-1018-49d5-b26b-7c5aeb5e74f3', N'f3e84605-2311-4840-b03a-3f891996669e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Document Merge Project Link to Document Templates', NULL, N'1d3ef4de-3f3f-4e23-bc29-779f502cca9a', N'5531e4dc-1018-49d5-b26b-7c5aeb5e74f3', N'a2769cbd-24b1-437c-b42c-a8a789b654b1')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Currencies to Static Exchange Rates', CAST(N'2012-11-07T20:25:37.930' AS DateTime), N'21cdcdfc-5af0-448c-b90e-f84af490f523', N'850422ea-ad71-4cef-b6af-227933bf8065', N'4f924ddc-c6f8-46b4-a4e5-47c2269abe95')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Mileage Categories to Mileage Date Ranges', CAST(N'2013-01-29T19:32:38.097' AS DateTime), N'd1ee2cbb-605d-4681-a46b-e626c03581f0', N'5a83aeaf-86c8-48fb-aa2b-e7ab05a74a0b', N'b21cb70d-7dd5-4d4d-adc3-dd8223dbb707')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'From Mileage Cat to Mileage Threshold via Date range', CAST(N'2013-02-13T20:38:04.597' AS DateTime), N'7c169a27-6352-4447-a8df-245d718230e9', N'5a83aeaf-86c8-48fb-aa2b-e7ab05a74a0b', N'7b668e7b-d021-4e63-a185-9159b6b8726e')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Car to CarAssignmentNumberAllocations', CAST(N'2013-04-27T14:32:05.260' AS DateTime), N'59117613-50aa-4483-8ca1-4146cde0fb00', N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'439012b0-0071-43c8-a70d-a1acb59f9698')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Cars to ESR Assignments', CAST(N'2013-04-27T13:32:06.390' AS DateTime), N'bf9aa39a-82d6-4960-bfef-c5943bc0542d', N'a184192f-74b6-42f7-8fdb-6dcf04723cef', N'79872a79-3c17-4ca9-b9a0-4c68cec9b2ce')
GO
INSERT [dbo].[jointables_base] ([description], [amendedon], [tableid], [basetableid], [jointableid]) VALUES (N'Saved Expenses to Esr Assignment Costings', CAST(N'2013-04-27T14:32:07.127' AS DateTime), N'3efb3d39-0583-42f2-8557-580319f2082a', N'd70d9e5f-37e2-4025-9492-3bcf6aa746a8', N'a3af1529-2c7a-490e-9b1f-3c184402ff21')
GO

