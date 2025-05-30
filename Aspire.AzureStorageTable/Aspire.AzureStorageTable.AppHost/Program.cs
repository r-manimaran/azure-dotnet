using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var azStorage = builder.AddAzureStorage("azstorage");

if(builder.Environment.IsDevelopment())
{
    azStorage.RunAsEmulator();
}

var strTables = azStorage.AddTables("strTables");

builder.AddProject<Projects.EmployeesApi>("employeesapi")
    .WithExternalHttpEndpoints() // This will add the external HTTP endpoints for the Employees API
    .WithReference(strTables)
    .WaitFor(strTables);

builder.Build().Run();
