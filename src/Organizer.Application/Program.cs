using Microsoft.Extensions.DependencyInjection;
using Organizer.Application.DependencyInjection;
using Organizer.Application.Services;

Console.WriteLine("Starting Photo Organizer");

var services = new ServiceCollection();
services.AddApplication();

var serviceProvider = services.BuildServiceProvider();
var workflowService = serviceProvider.GetRequiredService<IWorkflowService>();
await workflowService.RunAsync();

Console.WriteLine("Finished Photo Organizer");