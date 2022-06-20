SET XACT_ABORT ON

USE [__DatabaseName__]
IF NOT EXISTS (SELECT 1 FROM sys.triggers WHERE object_id = OBJECT_ID(N'[__SchemaName__].[__TableName__Inserted]'))
BEGIN
   EXECUTE ('
   CREATE TRIGGER [__SchemaName__].[__TableName__Inserted] ON [__SchemaName__].[__TableName__] AFTER INSERT AS
    BEGIN
     DECLARE @h UNIQUEIDENTIFIER
     DECLARE @msg NVARCHAR(MAX);

     BEGIN DIALOG CONVERSATION @h FROM SERVICE [__TableName__ChangesSenderService] TO SERVICE ''__TableName__ChangesReceiverService'' ON CONTRACT [__TableName__.Changes] WITH ENCRYPTION = OFF;

     SET @msg = (SELECT * FROM INSERTED FOR JSON AUTO);

     SEND ON CONVERSATION @h MESSAGE TYPE [__InsertBrokerMessageTypeName__]( @msg );
    END

    ALTER TABLE [__SchemaName__].[__TableName__] ENABLE TRIGGER [__TableName__Inserted]
    ')
END
