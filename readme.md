## Application Details

The Productivity Monitor runs on the Windows Operating System and tracks application, keyboard, and mouse data locally. It operates using several of Asp.Net Core 3's Worker Services, Entity Framework running on top of SQLite, a Asp.Net web API, and Razor Pages.

Note: the application can be made cross platform if someone finds Kernel methods in Unix that provide the same information as their Windows equivalent.

## Building the Application

To build the application you must first have the [.Net Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) installed. This comes with a runtime, so no need for the separate one.

Once you have downloaded the application, navigate to the .\src\ProductivityMonitor.Service\ProductivityMonitor.Service directory, and run the command `dotnet run`. This will start the Kestrel Web server, initialize the database, and start running the application on http://localhost:5000 and https://localhost:5001 (http will redirect to https).

The service will immediately start collecting data on start, and you can see the live results by refreshing the page.

## Determining Productive Time

The application determines if you are being productive during a certain time by registering mouse and keyboard activity.

