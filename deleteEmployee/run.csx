#r "Newtonsoft.Json"
#r "Microsoft.Azure.DocumentDB.Core"
#r "Microsoft.Azure.WebJobs.Extensions.CosmosDB"



using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;

public static async Task<IActionResult> Run(
  HttpRequest req, 
  [CosmosDB(ConnectionStringSetting = "cmp-cosmosdb-crud_DOCUMENTDB")]  string Id ,string employeeId )
{
  DocumentClient client = new DocumentClient(new Uri("https://cmp-cosmosdb-crud.documents.azure.com:443/"),"sil1vWU7SOImoaPAc1yooJ2tKRCTmXWnPBayldN07QZOJNfvHw2hXvOwrllitYq8zEPRD2UclBjMKQFXtiNOvw=="); 
  var option = new FeedOptions { EnableCrossPartitionQuery = true };
  var collectionUri = UriFactory.CreateDocumentCollectionUri("cmptestdb", "newEmployeeContainer");
  
  var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == Id)
        .AsEnumerable().FirstOrDefault();
  
  if (document == null)
  {
    return new NotFoundResult();
  }

  await client.DeleteDocumentAsync(document.SelfLink ,new RequestOptions {PartitionKey = new Microsoft.Azure.Documents.PartitionKey(employeeId)});
  return new OkResult();
}