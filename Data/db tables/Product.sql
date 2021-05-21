CREATE TABLE Product (
	[ID] bigint IDENTITY(1,1),
	[BrandID] int NULL,
	[Code] bigint NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,    	
	[ExternalName] NVARCHAR(50) NOT NULL,    	
	[Description] NVARCHAR(MAX) NULL,
	[ImageID] varchar(50) NULL,
	[Price] money NULL,
);

ALTER TABLE Product ADD CONSTRAINT PK_Product PRIMARY KEY(ID)

---------------------------------------------------------------------------

CREATE TABLE Category
(
	[ID] int identity(1,1),
	[Name] varchar(50) NOT NULL,

	[DesktopSpotlight] bit NULL,
	[DesktopSpotlightImageID] varchar(50) NULL,
	[MobileSpotlight] bit NULL,
	[MobileSpotlightImageID] varchar(50) NULL
)
ALTER TABLE Category Add Constraint PK_Category Primary Key(ID)

---------------------------------------------------------------------------

CREATE TABLE CategoryDetail
(
	[ID] int identity(1,1),
	[CategoryID] int NOT NULL,
	[Name] varchar(50) NOT NULL,
	[TitleIconID] varchar(50) NULL,
)

ALTER TABLE CategoryDetail Add Constraint PK_CategoryDetail Primary Key(ID)
ALTER TABLE CategoryDetail Add Constraint FK_CategoryDetail Foreign Key(CategoryID) References Category(ID)

--------------------------------------------------------------------------

CREATE TABLE Brand
(
	[ID] int identity(1,1),
	[Name] varchar(50) NOT NULL,

	[DesktopSpotlight] bit NULL,
	[DesktopSpotlightImageID] varchar(50) NULL,
	[MobileSpotlight] bit NULL,
	[MobileSpotlightImageID] varchar(50) NULL
)
ALTER TABLE Brand Add Constraint PK_Brand Primary Key(ID)
ALTER TABLE Product Add Constraint FK_ProductBrand Foreign Key(BrandID) References Brand(ID)

-------------------------------------------------------------------------

CREATE TABLE ProductCategoryDetail
(
	ProductID bigint NOT NULL,
	CategoryDetailID int NOT NULL
)

ALTER TABLE ProductCategoryDetail Add Constraint PK_ProductCategoryDetail Primary Key(ProductID, CategoryDetailID)
ALTER TABLE ProductCategoryDetail Add Constraint FK_ProductCategoryDetail_Product Foreign Key(ProductID) References Product(ID)
ALTER TABLE ProductCategoryDetail Add Constraint FK_ProductCategoryDetail_Category Foreign Key(CategoryDetailID) References CategoryDetail(ID)

--DROP TABLE ProductCategoryDetail
--DROP TABLE CategoryDetail
--DROP TABLE Category
--DROP TABLE Product
