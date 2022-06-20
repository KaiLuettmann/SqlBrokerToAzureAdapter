SET XACT_ABORT ON

USE [__DatabaseName__]
IF NOT EXISTS (SELECT 1 FROM sys.triggers WHERE object_id = OBJECT_ID(N'[__SchemaName__].[__TableName__Deleted]'))
BEGIN
   EXECUTE ('
        CREATE TRIGGER [__SchemaName__].[__TableName__Deleted] ON [__SchemaName__].[__TableName__] AFTER Delete AS
        BEGIN
         DECLARE @h UNIQUEIDENTIFIER
         DECLARE @msg NVARCHAR(MAX);
         DECLARE @count INT;

         BEGIN DIALOG CONVERSATION @h FROM SERVICE [__TableName__ChangesSenderService] TO SERVICE ''__TableName__ChangesReceiverService'' ON CONTRACT [__TableName__.Changes] WITH ENCRYPTION = OFF;

         SET @count = (SELECT count(1) FROM Deleted);
         SET @msg = (SELECT * FROM Deleted FOR JSON AUTO);

         IF @count > 0
            SEND ON CONVERSATION @h MESSAGE TYPE [__DeleteBrokerMessageTypeName__]( @msg );
        END


        ALTER TABLE [__SchemaName__].[__TableName__] ENABLE TRIGGER [__TableName__Deleted]
    ')
END