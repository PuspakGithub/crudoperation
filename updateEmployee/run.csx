#r "Newtonsoft.Json"
#r "Microsoft.Azure.DocumentDB.Core"
#r "Microsoft.Azure.WebJobs.Extensions.CosmosDB"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;



public class Employee
{  
  public string employeeId { get; set; }
  public string name { get; set; }
  public string dept { get; set; }
  public string mobileno { get; set; }
}

public static async Task<IActionResult> Run(
  HttpRequest req, 
  [CosmosDB(ConnectionStringSetting = "cmp-cosmosdb-crud_DOCUMENTDB")]   ILogger log, string Id )
{
 // DocumentClient client = new DocumentClient(new Uri("https://<your-account-name>.documents.azure.com:443/"), "<your-account-key>");
  DocumentClient client = new DocumentClient(new Uri("https://cmp-cosmosdb-crud.documents.azure.com:443/"),"sil1vWU7SOImoaPAc1yooJ2tKRCTmXWnPBayldN07QZOJNfvHw2hXvOwrllitYq8zEPRD2UclBjMKQFXtiNOvw==");
 //DocumentClient client = new DocumentClient(new Uri("https://cmp-cosmos-test-db.documents.azure.com:443/"), "LaaoWmd7fZTIBybgdEFrxxlzxlYef4TiM3KK3VnmhvIFdssg6OaHUYPnxCE4mLh5yXO8Znu1nskeq8gCTh17EQ==");
  //log.LogInformation("Creating a Id: ",Id);
  //log.LogInformation("Creating a employeeId: ",employeeId);
  string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
  var updated = JsonConvert.DeserializeObject<Employee>(requestBody);
  
  var option = new FeedOptions { EnableCrossPartitionQuery = true };
  var collectionUri = UriFactory.CreateDocumentCollectionUri("cmptestdb", "newEmployeeContainer");
  
  var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == Id)
        .AsEnumerable().FirstOrDefault();
  
  if (document == null)
  {
    return new NotFoundResult();
  }

  document.SetPropertyValue("name", updated.name); 
  document.SetPropertyValue("dept", updated.dept);
  document.SetPropertyValue("mobileno", updated.mobileno);
  
  await client.ReplaceDocumentAsync(document);
  
  //return new OkResult();
  return (ActionResult)new OkObjectResult(document);
}