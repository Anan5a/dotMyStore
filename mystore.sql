USE [MyStore];
CREATE TABLE [Users] (
	[Id] bigint IDENTITY(1,1) NOT NULL UNIQUE,
	[Name] nvarchar(50) NOT NULL,
	[Email] nvarchar(100) NOT NULL UNIQUE,
	[Password] nvarchar(256) NOT NULL,
	[Role] bigint NOT NULL,
	[CreatedAt] datetime2(7) NOT NULL,
	[ModifiedAt] datetime2(7) DEFAULT NULL,
	PRIMARY KEY ([Id])
);

CREATE TABLE [Roles] (
	[id] bigint IDENTITY(1,1) NOT NULL UNIQUE,
	[RoleName] nvarchar(10) NOT NULL UNIQUE CHECK ([RoleName] IN ('Admin', 'User')),
	PRIMARY KEY ([id])
);

CREATE TABLE [ProductImages] (
	[Id] bigint IDENTITY(1,1) NOT NULL UNIQUE,
	[ImageUrl] nvarchar(max) NOT NULL,
	[ProductId] bigint NOT NULL,
	PRIMARY KEY ([Id])
);

CREATE TABLE [Products] (
	[Id] bigint IDENTITY(1,1) NOT NULL UNIQUE,
	[Title] nvarchar(max) NOT NULL,
	[Description] nvarchar(max),
	[SKU] nvarchar(16) NOT NULL,
	[Price] decimal(18,0) NOT NULL,
	[Weight] decimal(18,0) NOT NULL,
	PRIMARY KEY ([Id])
);

ALTER TABLE [Users] ADD CONSTRAINT [Users_fk4] FOREIGN KEY ([Role]) REFERENCES [Roles]([id]);

ALTER TABLE [ProductImages] ADD CONSTRAINT [ProductImages_fk2] FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]);


--- Insert role data, so we would not need a new api for only 2 entries

INSERT INTO [Roles]([RoleName]) VALUES ('Admin'), ('User');