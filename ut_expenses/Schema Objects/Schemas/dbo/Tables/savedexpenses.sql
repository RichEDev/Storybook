CREATE TABLE [dbo].[savedexpenses](
	[expenseid] [int] IDENTITY(756969,1) NOT NULL,
	[bmiles] [decimal](18, 2) NULL,
	[pmiles] [decimal](18, 2) NULL,
	[reason] [nvarchar](2500) NULL,
	[receipt] [bit] NOT NULL,
	[net] [money] NOT NULL,
	[vat] [money] NOT NULL,
	[total] [money] NOT NULL,
	[subcatid] [int] NOT NULL,
	[date] [datetime] NOT NULL,
	[staff] [tinyint] NULL,
	[others] [tinyint] NULL,
	[companyid] [int] NULL,
	[returned] [bit] NULL,
	[home] [bit] NULL,
	[refnum] [nvarchar](50) NOT NULL,
	[claimid] [int] NOT NULL,
	[plitres] [int] NULL,
	[blitres] [int] NULL,
	[allowanceamount] [money] NULL,
	[currencyid] [int] NULL,
	[attendees] [nvarchar](1000) NULL,
	[tip] [money] NOT NULL,
	[countryid] [int] NULL,
	[foreignvat] [money] NOT NULL,
	[convertedtotal] [money] NOT NULL,
	[exchangerate] [float] NOT NULL,
	[tempallow] [bit] NOT NULL,
	[reasonid] [int] NULL,
	[normalreceipt] [bit] NOT NULL,
	[receiptattached] [bit] NOT NULL,
	[allowancestartdate] [datetime] NULL,
	[allowanceenddate] [datetime] NULL,
	[carid] [int] NULL,
	[allowancededuct] [money] NULL,
	[allowanceid] [int] NULL,
	[nonights] [tinyint] NULL,
	[quantity] [float] NULL,
	[directors] [tinyint] NULL,
	[amountpayable] [money] NOT NULL,
	[hotelid] [int] NULL,
	[primaryitem] [bit] NOT NULL,
	[norooms] [tinyint] NOT NULL,
	[vatnumber] [nvarchar](50) NULL,
	[personalguests] [tinyint] NULL,
	[remoteworkers] [tinyint] NULL,
	[accountcode] [nvarchar](50) NULL,
	[pencepermile] [money] NULL,
	[basecurrency] [int] NULL,
	[globalexchangerate] [float] NULL,
	[globalbasecurrency] [int] NULL,
	[globaltotal] [money] NULL,
	[itemtype] [tinyint] NOT NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[mileageid] [int] NULL,
	[transactionid] [int] NULL,
	[journey_unit] [tinyint] NULL,
	[esrAssignID] [int] NULL,
	[AssignmentNumber] [nvarchar](50) NULL,
	[CacheExpiry] [datetime] NULL,
	[hometooffice_deduction_method] [tinyint] NULL,
	[addedAsMobileItem] [bit] NOT NULL,
	[addedByDeviceTypeId] [int] NULL,
	[itemCheckerId] [int] NULL,
 CONSTRAINT [PK_savedexpenses] PRIMARY KEY NONCLUSTERED 
(
	[expenseid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_allowances] FOREIGN KEY([allowanceid])
REFERENCES [dbo].[allowances] ([allowanceid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_allowances]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_card_transactions] FOREIGN KEY([transactionid])
REFERENCES [dbo].[card_transactions] ([transactionid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_card_transactions]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_cars] FOREIGN KEY([carid])
REFERENCES [dbo].[cars] ([carid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_cars]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_claims_base] FOREIGN KEY([claimid])
REFERENCES [dbo].[claims_base] ([claimid])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_claims_base]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_companies] FOREIGN KEY([companyid])
REFERENCES [dbo].[companies] ([companyid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_companies]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_countries] FOREIGN KEY([countryid])
REFERENCES [dbo].[countries] ([countryid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_countries]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_currencies] FOREIGN KEY([currencyid])
REFERENCES [dbo].[currencies] ([currencyid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_currencies]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_employees] FOREIGN KEY([itemCheckerId])
REFERENCES [dbo].[employees] ([employeeid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_employees]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_esr_assignments] FOREIGN KEY([esrAssignID])
REFERENCES [dbo].[esr_assignments] ([esrAssignID])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_esr_assignments]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_mileage_categories] FOREIGN KEY([mileageid])
REFERENCES [dbo].[mileage_categories] ([mileageid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_mileage_categories]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_reasons] FOREIGN KEY([reasonid])
REFERENCES [dbo].[reasons] ([reasonid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_reasons]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_savedexpenses] FOREIGN KEY([expenseid])
REFERENCES [dbo].[savedexpenses] ([expenseid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_savedexpenses]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_savedexpenses1] FOREIGN KEY([expenseid])
REFERENCES [dbo].[savedexpenses] ([expenseid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_savedexpenses1]
GO

ALTER TABLE [dbo].[savedexpenses]  WITH CHECK ADD  CONSTRAINT [FK_savedexpenses_subcats] FOREIGN KEY([subcatid])
REFERENCES [dbo].[subcats] ([subcatid])
GO

ALTER TABLE [dbo].[savedexpenses] CHECK CONSTRAINT [FK_savedexpenses_subcats]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_bmiles]  DEFAULT ((0)) FOR [bmiles]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_pmiles]  DEFAULT ((0)) FOR [pmiles]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_receipt]  DEFAULT ((0)) FOR [receipt]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_net]  DEFAULT ((0)) FOR [net]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_vat]  DEFAULT ((0)) FOR [vat]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_total]  DEFAULT ((0)) FOR [total]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_staff]  DEFAULT ((0)) FOR [staff]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_others]  DEFAULT ((0)) FOR [others]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_returned]  DEFAULT ((0)) FOR [returned]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_home]  DEFAULT ((0)) FOR [home]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_plitres]  DEFAULT ((0)) FOR [plitres]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_blitres]  DEFAULT ((0)) FOR [blitres]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_allowanceamount]  DEFAULT ((0)) FOR [allowanceamount]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF__savedexpenses__tip__74444068]  DEFAULT ((0)) FOR [tip]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF__savedexpenses__forei__0A338187]  DEFAULT ((0)) FOR [foreignvat]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF__savedexpenses__conve__11D4A34F]  DEFAULT ((0)) FOR [convertedtotal]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF__savedexpenses__excha__12C8C788]  DEFAULT ((0)) FOR [exchangerate]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF__savedexpenses__tempa__0880433F]  DEFAULT ((0)) FOR [tempallow]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_normalreceipt]  DEFAULT ((0)) FOR [normalreceipt]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_receiptattached]  DEFAULT ((0)) FOR [receiptattached]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_amountpayable]  DEFAULT ((0)) FOR [amountpayable]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_primaryitem]  DEFAULT ((1)) FOR [primaryitem]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_norooms]  DEFAULT ((0)) FOR [norooms]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_previous_itemtype]  DEFAULT ((1)) FOR [itemtype]
GO

ALTER TABLE [dbo].[savedexpenses] ADD  CONSTRAINT [DF_savedexpenses_addedAsMobileItem]  DEFAULT ((0)) FOR [addedAsMobileItem]
GO