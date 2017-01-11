
[![build status](https://api.travis-ci.org/cyrillef/models.autodesk.io.png)](https://travis-ci.org/cyrillef/models.autodesk.io)
[![.Net](https://img.shields.io/badge/.Net-4.5-blue.svg)](https://msdn.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://www.nuget.org/)
![Platforms](https://img.shields.io/badge/platform-windows%20%7C%20osx%20%7C%20linux-lightgray.svg)
[![License](http://img.shields.io/:license-mit-blue.svg)](http://opensource.org/licenses/MIT)

*Forge API*:
[![oAuth2](https://img.shields.io/badge/oAuth2-v1-green.svg)](http://developer-autodesk.github.io/)
[![Data-Management](https://img.shields.io/badge/Data%20Management-v1-green.svg)](http://developer-autodesk.github.io/)
[![OSS](https://img.shields.io/badge/OSS-v2-green.svg)](http://developer-autodesk.github.io/)
[![Model-Derivative](https://img.shields.io/badge/Model%20Derivative-v2-green.svg)](http://developer-autodesk.github.io/)
[![Viewer](https://img.shields.io/badge/Forge%20Viewer-v2.12-green.svg)](http://developer-autodesk.github.io/)

# forge.commandline-csharp


<b>Note:</b> For using this sample, you need a valid oAuth credential.
Visit this [page](https://developer.autodesk.com) for instructions to get on-board.


Demonstrates the Autodesk Forge API authorisation and translation process using a C#/.Net console application.

* both 2 legged and 3 legged
* support both callback and promises


## Description

This sample exercises the .Net framework as a command line utility to demonstrate the Forge OAuth application
authorisation process and the Model Derivative API mentioned in the Quick Start guide.

In order to make use of this sample, you need to register your consumer key, of course:
* https://developer.autodesk.com > My Apps

This provides the credentials to supply while calling the Forge WEB service API endpoints.


## Dependencies

Visual Studio 2015, .Net Framework 4.5+


## Setup/Usage Instructions

  1. Download (fork, or clone) this project.
  2. Request your consumer key/secret key from [https://developer.autodesk.com](https://developer.autodesk.com).
  3. Set 2 environment variables FORGE_CLIENT_ID / FORGE_CLIENT_SECRET, or edit the Program.cs
     file and replace the placeholders by the consumer key/secret keys.
  4. *Note* for the 3 legged command: while registering your keys, make sure that the callback you define for your
     callback (or redirect_uri) is set in an environment variable named FORGE_CALLBACK.
     Default is : http://localhost:3006/oauth
  5. Load the project in Visual Studio 2015, and build the solution
  

A typical workflow is (replace -cb by -promise if you want to use promises vs callbacks):

    # Do authentication.
    forge.commandline-csharp.exe 2legged

    # Create a bucket. Bucket name must be lower case and valid characters.
    forge.commandline-csharp.exe bucketCreate my_bucket_name

    # Upload a model.
    forge.commandline-csharp.exe upload samples/Au.obj

    # Register the model to get it translated.
    forge.commandline-csharp.exe translate Au.obj

    # Wait until the translation completes.
    # Translation is complete when it reaches 'success - 100%'
    forge.commandline-csharp.exe translateProgress Au.obj

    # Retrieve preview image.
    forge.commandline-csharp.exe thumbnail Au.obj Au.png

    # Create an HTML page with your URN and a read-only access token.
    forge.commandline-csharp.exe html Au.obj Au.html

Note your access token and bucket name are saved in the data folder to be used as default by the scripts, but you can
edit them as you wish.

Bucket information (JSON replies) returned by the system are stored in the data folder as well.


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). 
Please see the [LICENSE](LICENSE) file for full details.


## Written by

Cyrille Fauvel <br />
Forge Partner Development <br />
http://developer.autodesk.com/ <br />
http://around-the-corner.typepad.com <br />
