SET XACT_ABORT ON

USE [__DatabaseName__]
IF NOT EXISTS (SELECT 1 FROM sys.service_queues WHERE object_id = OBJECT_ID(N'[__SchemaName__].[__ReceiverQueueName__]'))
BEGIN
    CREATE QUEUE [__SchemaName__].[__ReceiverQueueName__] WITH STATUS = ON , RETENTION = OFF , POISON_MESSAGE_HANDLING (STATUS = ON)  ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 FROM sys.service_queues WHERE object_id = OBJECT_ID(N'[__SchemaName__].[__SenderQueueName__]'))
BEGIN
    CREATE QUEUE [__SchemaName__].[__SenderQueueName__] WITH STATUS = ON , RETENTION = OFF , POISON_MESSAGE_HANDLING (STATUS = ON)  ON [PRIMARY]
END