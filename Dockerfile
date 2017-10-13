FROM awright18/aspnetcore-build-mono:latest AS builder

COPY Server/Nikeza.Wordpress/Nikeza.Wordpress.fsproj Server/Nikeza.Wordpress/Nikeza.Wordpress.fsproj
COPY Server/Nikeza.YouTube/Nikeza.YouTube.fsproj Server/Nikeza.YouTube/Nikeza.YouTube.fsproj
COPY Server/Nikeza.Server/Nikeza.Server.fsproj Server/Nikeza.Server/Nikeza.Server.fsproj
RUN dotnet restore Server/Nikeza.Server/Nikeza.Server.fsproj

COPY Client/app/*.json Client/app/
RUN npm install -g yarn \
    && cd Client/app \
    && npm install -g elm \
    && npm install \
    && elm-package install -y

COPY . .

RUN cd Client/app \
    && elm-make Home.elm --output=../../Server/Nikeza.Server/home.html \
    && cd ../../ \
    && dotnet publish Server/Nikeza.Server/Nikeza.Server.fsproj -f netcoreapp1.1 -o /app/ -c Release \
    && mkdir -p app/wwwroot/ \
    && cp -R Client/app/Assets/ app/wwwroot/

FROM microsoft/aspnetcore
WORKDIR app/
COPY --from=builder /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Nikeza.Server.dll"]
