var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LeftJoinAndRightJoinDemo>("leftjoinandrightjoindemo");

builder.Build().Run();
