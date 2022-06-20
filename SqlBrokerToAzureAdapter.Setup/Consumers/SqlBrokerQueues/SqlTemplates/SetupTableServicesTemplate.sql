SET XACT_ABORT ON

USE [__DatabaseName__]
IF NOT EXISTS (SELECT 1 FROM sys.service_message_types WHERE name = '__UpdateBrokerMessageTypeName__')
BEGIN
    CREATE MESSAGE TYPE [__UpdateBrokerMessageTypeName__] VALIDATION = NONE
END
IF NOT EXISTS (SELECT 1 FROM sys.service_message_types WHERE name = '__DeleteBrokerMessageTypeName__')
BEGIN
    CREATE MESSAGE TYPE [__DeleteBrokerMessageTypeName__] VALIDATION = NONE
END
IF NOT EXISTS (SELECT 1 FROM sys.service_message_types WHERE name = '__InsertBrokerMessageTypeName__')
BEGIN
    CREATE MESSAGE TYPE [__InsertBrokerMessageTypeName__] VALIDATION = NONE
END

IF NOT EXISTS (SELECT 1 FROM sys.service_contracts WHERE name = '__TableName__.Changes')
BEGIN
    CREATE CONTRACT [__TableName__.Changes] ([__DeleteBrokerMessageTypeName__] SENT BY INITIATOR,
    [__InsertBrokerMessageTypeName__] SENT BY INITIATOR,
    [__UpdateBrokerMessageTypeName__] SENT BY INITIATOR)
END

IF NOT EXISTS (SELECT 1 FROM sys.services WHERE name = '__TableName__ChangesReceiverService')
BEGIN
    CREATE SERVICE [__TableName__ChangesReceiverService]  ON QUEUE [__SchemaName__].[__ReceiverQueueName__] ([__TableName__.Changes])
END
IF NOT EXISTS (SELECT 1 FROM sys.services WHERE name = '__TableName__ChangesSenderService')
BEGIN
    CREATE SERVICE [__TableName__ChangesSenderService]  ON QUEUE [__SchemaName__].[__SenderQueueName__] ([__TableName__.Changes])
END