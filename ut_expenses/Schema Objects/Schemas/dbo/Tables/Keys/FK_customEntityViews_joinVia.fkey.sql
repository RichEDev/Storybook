alter table dbo.customEntityViews
add constraint [FK_customEntityViews_joinVia] foreign key (SortColumnJoinViaID) references joinVia (joinViaID) on delete set null