# Introduction

## Create an Azure-Topic
Create an Azure-Topic with the Name `sqlBroker-to-azureAdapter-example-v1` in your Azure subscription.

## Add your Azure ConnectionString
Add your Azure ConnectionString to the file SqlBrokerToAzureAdapter.Execution.Console/appsettings.Development.json to the section "Execution.AzureTopicProducer.ConnectionString".
Alternatively, you can set it as an environment variable:
```shell
export Execution__AzureTopicProducer__ConnectionString=<YourConnectionString>
```


## Build example sql server docker container

```shell
docker build -t sql-broker-to-azure-adapter-example -f ./sql/mssql-2019.Dockerfile ./sql/
```

## Start example sql server docker container

```shell
docker run --name sql-broker-to-azure-adapter-example -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Password@12345' -p 1433:1433 sql-broker-to-azure-adapter-example
```

## Setup database

```shell
env ASPNETCORE_ENVIRONMENT=Development dotnet run --project SqlBrokerToAzureAdapter.Execution.Console install
```

## Execute the Adapter

```shell
env ASPNETCORE_ENVIRONMENT=Development dotnet run --project SqlBrokerToAzureAdapter.Execution.Console run
```

## Connect to Database and see how it works

### Insert a user

```sql
INSERT INTO [dbo].[User](Lastname, Firstname, Phone, Email, NickName)
VALUES('Mustermann', 'Max','0123456', 'max@mustermann.de', 'Maxi')
```

As you can see in the console of the adapter the database changes are detected and the following log is written:
```shell
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerQueueRepository[0]
      Message received: ConversationHandle:'781e433e-f36b-1410-8108-008c04d07b9b', MessageEnqueueTime: '20.06.2022 10:34:57', MessageTypeName: 'User.Inserted'
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Start handling of broker message with id '781e433e-f36b-1410-8108-008c04d07b9b'.
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Insert detected.
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.MessageBodyDeserializer[0]
      Deserializing message body [{"Id":1,"Lastname":"Mustermann","Firstname":"Max","Phone":"0123456","EMail":"max@mustermann.de","NickName":"Maxi"}].
info: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
      Insert values transformed to Azure-Event
info: SqlBrokerToAzureAdapter.Producers.AzureTopics.AzureTopicProducer[0]
      Azure-Events published to Azure on topic 'sqlBroker-to-azureAdapter-example-v1'
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Handling of broker message with id '781e433e-f36b-1410-8108-008c04d07b9b' finished.
```

### Update the name information of the user

```sql
UPDATE [dbo].[User]
SET Firstname = 'Maximilian'
WHERE Firstname = 'Max' AND Lastname = 'Mustermann'
```

As you can see in the console of the adapter the database changes are detected and the following log is written:
```shell
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerQueueRepository[0]
      Message received: ConversationHandle:'981e433e-f36b-1410-8108-008c04d07b9b', MessageEnqueueTime: '20.06.2022 11:04:25', MessageTypeName: 'User.Updated'
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Start handling of broker message with id '981e433e-f36b-1410-8108-008c04d07b9b'.
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
Update detected.
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.MessageBodyDeserializer[0]
Deserializing message body [{"OldValue":{"Id":1,"Lastname":"Mustermann","Firstname":"Max","Phone":"0123456","EMail":"max@mustermann.de","NickName":"Maxi"},"NewValue":{"Id":2,"Lastname":"Mustermann","Firstname":"Maximilian","Phone":"0123456","EMail":"max@mustermann.de","NickName":"Maxi"}},].
dbug: SqlBrokerToAzureAdapter.Transformations.ObjectComparer[0]
The following members are changed: Firstname
dbug: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
EditTransformation of type 'UserNameChangedTransformation' is responsible for detected changes.
info: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
Edited values transformed to Azure-Event
info: SqlBrokerToAzureAdapter.Producers.AzureTopics.AzureTopicProducer[0]
Azure-Events published to Azure on topic 'sqlBroker-to-azureAdapter-example-v1'
dbug: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
EditTransformation of type 'UserContactInfoChangedTransformation' is not responsible for detected changes.
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
Handling of broker message with id '981e433e-f36b-1410-8108-008c04d07b9b' finished.
```

### Update the contact information of the user

```sql
UPDATE [dbo].[User]
SET Phone = 01234567
WHERE Lastname = 'Mustermann'
```

As you can see in the console of the adapter the database changes are detected and the following log is written:
```shell
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerQueueRepository[0]
      Message received: ConversationHandle:'a504433e-f36b-1410-870d-00405a72a7fc', MessageEnqueueTime: '20.06.2022 11:05:01', MessageTypeName: 'User.Updated'
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Start handling of broker message with id 'a504433e-f36b-1410-870d-00405a72a7fc'.
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Update detected.
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.MessageBodyDeserializer[0]
      Deserializing message body [{"OldValue":{"Id":1,"Lastname":"Mustermann","Firstname":"Maximilian","Phone":"0123456","EMail":"max@mustermann.de","NickName":"Maxi"},"NewValue":{"Id":1,"Lastname":"Mustermann","Firstname":"Maximilian","Phone":"01234567","EMail":"max@mustermann.de","NickName":"Maxi"}},].
dbug: SqlBrokerToAzureAdapter.Transformations.ObjectComparer[0]
      The following members are changed: Phone
dbug: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
      EditTransformation of type 'UserNameChangedTransformation' is not responsible for detected changes.
dbug: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
      EditTransformation of type 'UserContactInfoChangedTransformation' is responsible for detected changes.
info: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
      Edited values transformed to Azure-Event
info: SqlBrokerToAzureAdapter.Producers.AzureTopics.AzureTopicProducer[0]
      Azure-Events published to Azure on topic 'sqlBroker-to-azureAdapter-example-v1'
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Handling of broker message with id 'a504433e-f36b-1410-870d-00405a72a7fc' finished.
```

### Delete the user

```sql
DELETE [dbo].[User]
where Lastname = 'Mustermann'
```

As you can see in the console of the adapter the database changes are detected and the following log is written:
```shell
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerQueueRepository[0]
      Message received: ConversationHandle:'9c1e433e-f36b-1410-8108-008c04d07b9b', MessageEnqueueTime: '20.06.2022 10:41:09', MessageTypeName: 'User.Deleted'
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Start handling of broker message with id '9c1e433e-f36b-1410-8108-008c04d07b9b'.
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Delete detected.
dbug: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.MessageBodyDeserializer[0]
      Deserializing message body [{"Id":1,"Lastname":"Mustermann","Firstname":"Maximilian","Phone":"0123456","EMail":"max@mustermann.de","NickName":"Maxi"}].
info: SqlBrokerToAzureAdapter.Adapter.SqlBrokerToAzureAdapter[0]
      Deleted values transformed to Azure-Event
info: SqlBrokerToAzureAdapter.Producers.AzureTopics.AzureTopicProducer[0]
      Azure-Events published to Azure on topic 'sqlBroker-to-azureAdapter-example-v1'
info: SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.SqlBrokerMessageHandler[0]
      Handling of broker message with id '9c1e433e-f36b-1410-8108-008c04d07b9b' finished.
```