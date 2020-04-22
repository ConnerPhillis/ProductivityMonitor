
## Introduction

The project required us to build an application to measure "productivity" by monitoring keyboard and mouse activity of users. This required a substantial amount of work, and at least for my own personal implementation required making calls directly to the Windows Kernel, writing a polling service for key presses, building a database to store usage data, writing a web API, and writing a front end to consume, format, and display the data provided by the web API. Throughout this document I will detail the approach I took to developing each of the implementations, present the frontend, and finally analyze a four hour productivity session.


## Calling the Windows Kernel

When I was first doing research about starting this project using Windows Hooks, a platform to capture system events and redirect them to your own application, was the first avenue that was suggested to me by a coworker. After initially struggling to get the hook to report its progress to my application, I managed to get it working, however I found that the hook introduced an incredibly large strain on my system, making it almost unusable. After spending time trying to optimize my solution I eventually decided that polling would have to be my approach. Using information about the Windows kernel from pinvoke.net I was able to find several calls in User32.dll that worked for my needs. The specific system calls were:
 - `GetForegroundWindow` 
 - `GetWindowThreadProcessId`
 - `GetAsyncKeyState`
 - `GetCursorPos`

I spent several hours writing services that polled these API methods, and when I was able to run all of them at the same time I found that their performance was significantly better than the code that used Windows Hooks. This mean that I was ready to move on to the database.

## Building Out the Database

I chose to use SQLite as the database provider for my application, reason being it is a free open source database that runs mostly in the memory of the application using it. Since my needs for scaling were fairly small, and there was almost no need to distribute the database it was a clear choice.

I also chose to use Microsoft's Entity Framework Core Object Relational Mapper to interact with the database. Entity Framework is a quick easy solution for deploying databases, and the set of APIs it offers in ASP.NET Core makes its usage natural.

Originally in an attempt to save time I created a database with three tables, one to store all of the information about whatever process was currently running, one to store information about the mouse movements, and another to store information about keyboard input. These tables were initially independent of one another for reasons that I will explain more of later. This design initially worked very well, making it so that I could record all inputs separate of one another and aggregate the information as I needed. 

Eventually, however, I discovered that the lack of connectivity between the entities was causing a serious performance penalty, as some API methods were taking ~7 seconds to query the database, and only increasing as I logged more data. Unfortunately, because of how SQLite manages its table structure when it came time to build relationships between the entities I had to delete the existing database and lose all the data I had previously recorded. Once I had rebuilt the database, however, the performance problem seemed to evaporate, and so far no method call has had a duration longer than 200 ms.

## Writing the Polling Services

The polling services were written using ASP.NET Core 3's new worker service template. This framework allows ASP.NET to run a background service behind a web api with access to the same dependency injection container that the Web API controllers have, giving quick and easy access to the database or cache. Originally, I built out the services so that the application monitoring service would take a record every second, and the mouse and keyboard services would check on location and key status every 10 milliseconds. Each one recording to their own personal database table. 

While this initially worked very well, it became clear to me that there needed ot be some coupling between the services, as their disconnected nature made querying the data that they recorded very difficult and time consuming. Part of the database rewrite was exposing a bit of internal state from the application service that allowed keyboard and mouse events to form relationships with the application records, drastically simplifying the query logic.

I also discovered that the mouse input service was logging an absurd amount of data. Because when the mouse is moved it is usually moved in wide archs and for long durations I discovered that it had managed to record 72 thousand independent events during its first twelve minutes of operation. This was causing an explosion in the size of the database and also contributing to the degraded performance that I was experiencing. After cutting the polling time from 10 ms to 100 ms and deleting a lot of data that was unnecessary I found the database to once again be growing at a respectable rate, and left the polling services to focus on the Web API.

## Constructing the Web API

Because most of my experience as a backend developer is with REST APIs I chose to write all of my data accesses as REST endpoints. Initially because of the decoupled nature of the database I wrote three endpoints, one for fetching application, keyboard, and mouse data. When I started to test these methods with Postman, however, I found that the amount of data that they were returning was too large to expect a web browser to handle without having issues. 

This mean that my initial set of three API endpoints, while still usable, had to be deleted in favor of four more resource intensive API endpoints. After writing these endpoints I found that the back end experienced significantly more CPU load while operating, however the data was far easier for the frontend to stomach, and building out the functions that would go generate the graphs was far easier.

Note: The documentation for the API methods can be found at the /documentation endpoint from the content root of the frontend (e.g. https://localhost:5001/documentation)

## Building the Front End

Initially I thought that the front end could be written in its entirety with ASP.NET Core's Razor pages, however, I found that integrating that framework with the charting library that I chose to use would have been too difficult, so I instead chose to serve mostly static content, and do the majority of the data adjustments with JavaScript. 

The charting library that I chose was chart.js, and open source charting library that generates clean looking charts. Admittedly,the format that they request that their data be provided in is extremely awkward to work with, but lacking other free open source alternatives with similar levels of documentation I was forced to compromise.

The majority of the issues that I experienced developing the frontend had to do with the data formatting issues that chart.js was causing, barring that, developing the frontend was far easier than I was expecting it to be. 