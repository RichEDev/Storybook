create view [dbo].[savedexpenses_subcat_countries]
As
select expenseid,subcats_countries.subcatid,subcats_countries.countryid,subcats_countries.accountcode from savedexpenses  inner join subcats_countries on subcats_countries.subcatid=savedexpenses.subcatid 
									and subcats_countries.countryid=savedexpenses.countryid
