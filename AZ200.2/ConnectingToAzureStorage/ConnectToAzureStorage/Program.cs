using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Option 1: Stable, used in this project
//Install-Package WindowsAzure.Storage
using Microsoft.WindowsAzure.Storage;
//Use to Access Table Storage
using Microsoft.WindowsAzure.Storage.Table;

//Option 2: (Recommended, new approach still in preview) will have conflict with option 1
//1. Install-Package Microsoft.azure.storage.common 
using Microsoft.Azure;
//2. Install-Package Microsoft.azure.Cosmosdb.table
// to access table storage



namespace ConnectToAzureStorage
{
    class Program
    {
        public static void ConnectToStorage()
        {
            //Get the connectionstring
            //1. Click the storageAccount > Access Keys > Copy the ConnectionString
            var account = CloudStorageAccount.Parse("constr");

            Console.WriteLine("Successfully Connected to Storage Account!");
            Console.WriteLine(account.BlobEndpoint.ToString());
            Console.WriteLine(account.TableEndpoint.ToString());
            Console.WriteLine(account.FileEndpoint.ToString());
            Console.WriteLine(account.QueueEndpoint.ToString());
        }

        public static void UsingStorageAccountTable()
        {
            //Design and implement Storage Tables
            var account = CloudStorageAccount.Parse("constr");
            CloudTableClient tableClient = account.CreateCloudTableClient();

            //Creating a table
            var table = tableClient.GetTableReference("Customers");
            table.CreateIfNotExists();


            //Adding an Entity to the Table
            // Create a new customer entity.
            CustomerEntity customer1 = new CustomerEntity("Harp", "Walter");
            customer1.Email = "Walter@contoso.com";
            customer1.PhoneNumber = "425-555-0101";
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);
            // Execute the insert operation.
            table.Execute(insertOperation);
        }


        public static void InsertBatchOfEntities()
        {
            Console.WriteLine("Inserting Batch Data!");
            var account = CloudStorageAccount.Parse("constr");
            CloudTableClient tableClient = account.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");


            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();
            // Create a customer entity and add it to the table.
            CustomerEntity customer1 = new CustomerEntity("Smith", "Jeff");
            customer1.Email = "Jeff@contoso.com";
            customer1.PhoneNumber = "425-555-0104";

            // Create another customer entity and add it to the table.
            CustomerEntity customer2 = new CustomerEntity("Smith", "Ben");
            customer2.Email = "Ben@contoso.com";
            customer2.PhoneNumber = "425-555-0102";

            // Add both customer entities to the batch insert operation.
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);

            // Execute the batch operation.
            table.ExecuteBatch(batchOperation);
        }

        public static void RetrievingEntities()
        {
            Console.WriteLine("Retrieving Batch Data!");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("constr");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"));

            // Print the fields for each customer.
            foreach (CustomerEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }
        }
        public static void RetrievingARangeofEntities()
        {
            Console.WriteLine("Retrieving A Range of Data!");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("constr");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");

            // Create the table query.
            TableQuery<CustomerEntity> rangeQuery = new TableQuery<CustomerEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Smith"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, "E")));

            // Loop through the results, displaying information about the entity.
            foreach (CustomerEntity entity in table.ExecuteQuery(rangeQuery))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
            }
        }

        public static void RetrievingASingleEntity()
        {
            Console.WriteLine("Retrieving A Single Data!");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("constr");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                Console.WriteLine(((CustomerEntity)retrievedResult.Result).PhoneNumber);
            }
            else
            {
                Console.WriteLine("The phone number could not be retrieved.");
            }
        }

        public static void ReplaceAnEntity()
        {
            Console.WriteLine("Replace an Entity");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("constr");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            CustomerEntity updateEntity = (CustomerEntity)retrievedResult.Result;

            if (updateEntity != null)
            {
                // Change the phone number.
                updateEntity.PhoneNumber = "425-555-0105";

                // Create the Replace TableOperation.
                TableOperation updateOperation = TableOperation.Replace(updateEntity);

                // Execute the operation.
                table.Execute(updateOperation);

                Console.WriteLine("Entity updated.");
            }
            else
            {
                Console.WriteLine("Entity could not be retrieved.");
            }
        }

        public static void InsertOrReplaceEntity()
        {
            Console.WriteLine("Insert or Replace an Entity");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("constr");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");

            // Create a customer entity.
            CustomerEntity customer3 = new CustomerEntity("Jones", "Fred");
            customer3.Email = "Fred@contoso.com";
            customer3.PhoneNumber = "425-555-0106";

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer3);

            // Execute the operation.
            table.Execute(insertOperation);

            // Create another customer entity with the same partition key and row key.
            // We've already created a 'Fred Jones' entity and saved it to the
            // 'people' table, but here we're specifying a different value for the
            // PhoneNumber property.
            CustomerEntity customer4 = new CustomerEntity("Jones", "Fred");
            customer4.Email = "Fred@contoso.com";
            customer4.PhoneNumber = "425-555-0107";

            // Create the InsertOrReplace TableOperation.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(customer4);

            // Execute the operation. Because a 'Fred Jones' entity already exists in the
            // 'people' table, its property values will be overwritten by those in this
            // CustomerEntity. If 'Fred Jones' didn't already exist, the entity would be
            // added to the table.
            table.Execute(insertOrReplaceOperation);

        }
        public static void DeletingAnEntity()
        {
            Console.WriteLine("Deleting an Entity");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("constr");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");

            // Create a retrieve operation that expects a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity.
            CustomerEntity deleteEntity = (CustomerEntity)retrievedResult.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                table.Execute(deleteOperation);

                Console.WriteLine("Entity deleted.");
            }
            else
            {
                Console.WriteLine("Could not retrieve the entity.");
            }

        }

        public static void DeletingATable() {
            Console.WriteLine("Deleting a Table");

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("constr");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "Customers" table.
            CloudTable table = tableClient.GetTableReference("Customers");

            // Delete the table it if exists.
            table.DeleteIfExists();
        }
        static void Main(string[] args)
        {
            ConnectToStorage();
            Console.ReadLine();
            UsingStorageAccountTable();
            Console.ReadLine();
            InsertBatchOfEntities();
            Console.ReadLine();
            RetrievingEntities();
            Console.ReadLine();
            RetrievingARangeofEntities();
            Console.ReadLine();
            RetrievingASingleEntity();
            Console.ReadLine();
            ReplaceAnEntity();
            Console.ReadLine();
            InsertOrReplaceEntity();
            Console.ReadLine();
            DeletingAnEntity();
            Console.ReadLine();
            DeletingATable();
            Console.ReadLine();
        }

       

    }
    //Table Entities
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            //The PartitionKey is a property that will exist on every 
            //single object that is best used to group similar objects together.
            //The partition key forms the first part of an entity's primary key.
            this.PartitionKey = lastName;
            //The row key is a unique identifier for an entity within a 
            //given partition.Together the PartitionKey and RowKey 
            //uniquely identify every entity within a table.
            //You must include the RowKey property in every insert, update, and delete operation. 
            this.RowKey = firstName;
        }
        public CustomerEntity() { }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
