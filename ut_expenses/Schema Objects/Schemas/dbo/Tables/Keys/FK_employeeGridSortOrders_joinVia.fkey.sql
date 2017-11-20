alter table dbo.employeeGridSortOrders
add constraint [FK_employeeGridSortOrders_joinVia] foreign key (sortJoinViaID) references joinVia (joinViaID) on delete cascade