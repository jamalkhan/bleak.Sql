create table testxyz.employee(
  Id bigint identity(1,1) not null,
  firstname nvarchar(500) not null,
  lastname nvarchar(500) not null,
primary key(id)
)
distkey(id)
compound sortkey(firstname, lastname);
