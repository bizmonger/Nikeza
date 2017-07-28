FROM microsoft/dotnet:1.1.2-runtime-jessie

RUN mkdir /opt/app

WORKDIR /opt/app

COPY dist /opt/app

ENTRYPOINT ["dotnet", "Nikeza.Server.dll"]