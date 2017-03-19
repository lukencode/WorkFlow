CREATE TABLE [Order]  (
	Id nvarchar(100) NOT NULL PRIMARY KEY, 
	[Started] datetime null, 
	Updated datetime null, 
	Completed datetime null, 
	[Status] nvarchar(100) not null, 
	[JSON] nvarchar(max) not null
) 