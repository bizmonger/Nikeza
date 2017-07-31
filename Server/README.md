APIs is a collection of project mean to integrate with 3rd party services

# video-cli 
A simple cli for getting uploads from a YouTube channel
* To get a API Key follow the introductions [here](https://developers.google.com/youtube/v3/getting-started)

# video-youtube
A Simple DotNet Core library for getting YouTube uploads from a user or channel. 

# Docker 

There are a few helper scripts to make using Docker a bit simpler. For simplicity with Docker and to avoid relative paths all the scripts are just in the Server root. 

## Docker base Images
We are using the official jessie [dotnet core image](https://hub.docker.com/r/microsoft/dotnet/).

## Dev 

The dev images use the SDK variant of the official Microsoft. To use the dev image you need to build the Docker container
```
./docker/build-docker-dev
```

Once that's done you can run the helper script to boot the image, compile and start the server.
```
./docker/run-docker-dev
```

## Production 

## Build A Release

At some point, we will distribute a packages docker image using Docker hub. For now, you can compile your own. To make this simpler, you can use the helper scripts

```
./docker/build-docker-dev
./docker/run-docker-build
```

Finally, we can run the release image
```
./docker/run-docker-prod
```

You can also execute the build artifact by running
```
dotnet ./dist/Nikeza.Server.dll
```

## Helper scripts
* `build-app` - runs dotnet restore & build on each project.
* `build-app-release` - restores, builds, and publishes a release build framework dependent variant to the dist folder.
* `build-docker-dev` - builds a docker image named `nikeza-server-dev` used for development and building releases.
* `build-docker-prod` - builds a docker image named `nikeza-server-prod` used for running a framework dependent variant of the release. 
* `run-app-dev` - restores, builds and run the debug app.
* `run-docker-build` - start the `nikeza-server-dev` image and build a framework dependent release artifact and put the result into the dist folder.
* `run-docker-build` -  start the `nikeza-server-dev` and run the debug variant.
* `run-docker-prod` -  start the `nikeza-server-prod` and run the built app from dist with the dotnet core runtime.

# Why use 0.0.0.0 ?

Cause Docker. Using localhost creates a port binding conflict. So, the workaround is to use 0.0.0.0. See source for workaround https://github.com/aspnet/KestrelHttpServer/issues/639#issuecomment-220420114
