# XbimWebDemo
Simple demonstration of xBim IFC and Geometry [].NET libraries](http://github/XbimTeam/) running under a web server, using a standard ASP.NET MVC5 template.

This is a sample application - so it's not meant to be production quality - just enough to demonstrate how to IFC files are parsed, 
their geometry converted and how a basic WebGL viewer can be invoked. The application also demonstrates how to federate geometry,
and has some diagnostics on the internal engine.


##Pre-requisites
In addition to the vanilla MVC project packages, this demo application uses the latest Xbim Nuget packages for 
*Xbim.Essentials* and *Xbim.Geometry*. 

The WebGL package (*XbimWebUI*) is pre-release and only available from Myget under the Xbim-develp feed. This means 
you must add `https://www.myget.org/F/xbim-develop/api/v2` to your Nuget sources before building the app. 

The Geometry engine is dependent on the [VC12 C++ runtime](http://www.microsoft.com/en-us/download/details.aspx?id=40784). This is installed with Visual Studio 2013, but will need
 deploying manually if installing to a non-developer machine. 

##Running

Assuming the pre-requisites are met you can just build and run the app.

A couple of hard-coded links will open some simple models. If you copy a couple of IFC files to *App_Data* folder 
and you should be able to see how to federate larger models like:

![Screenshot](http://andyward.github.io/images/Clinic_federated.PNG)

##Licence

This demo is made available under the MIT license.

The xbim toolkit is licenced under the CDDL Open Source licence. 

