SET XACT_ABORT ON

USE [__DatabaseName__]
IF NOT EXISTS (SELECT 1 FROM sys.triggers WHERE object_id = OBJECT_ID(N'[__SchemaName__].[__TableName__Updated]'))
BEGIN
   EXECUTE ('
    CREATE TRIGGER [__SchemaName__].[__TableName__Updated]
    ON [__SchemaName__].[__TableName__]
    AFTER UPDATE
    AS
    BEGIN
        DECLARE @h UNIQUEIDENTIFIER;
        DECLARE @msg NVARCHAR(MAX);

        BEGIN DIALOG CONVERSATION @h
        FROM SERVICE [__TableName__ChangesSenderService]
        TO SERVICE ''__TableName__ChangesReceiverService''
        ON CONTRACT [__TableName__.Changes]
        WITH ENCRYPTION = OFF;
        SET @msg =
        (
            SELECT CONCAT(''['',STRING_AGG(CONVERT(NVARCHAR(max),CONCAT(''{"OldValue":'',OldValue, '',"NewValue":'', NewValue,''},'')), CHAR(13)),'']'') AS names
            FROM (SELECT
            (
                SELECT *
                FROM DELETED AS d
                WHERE d.__PrimaryKeyColumnName__ = source.__PrimaryKeyColumnName__
                FOR JSON AUTO, without_array_wrapper
                ) AS OldValue,
            (
                SELECT *
                FROM INSERTED AS i
                WHERE i.__PrimaryKeyColumnName__ = source.__PrimaryKeyColumnName__
                FOR JSON AUTO, without_array_wrapper
                ) AS NewValue
            FROM INSERTED AS source
            ) AS t
        );

        SEND ON CONVERSATION @h
        MESSAGE TYPE [__UpdateBrokerMessageTypeName__]
        (@msg);
    END;

    ALTER TABLE [__SchemaName__].[__TableName__] ENABLE TRIGGER [__TableName__Updated]
    ')
END