CREATE DATABASE [ServiceBrokerTest];
GO

SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
begin Transaction
GO

USE [ServiceBrokerTest]
GO

CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL
    CONSTRAINT PK_User PRIMARY KEY (ID)) ON [PRIMARY]
GO

Commit;
