FROM ubuntu:16.04

RUN mkdir -p /opt/app/src
VOLUME /opt/app

RUN apt-get update
RUN apt-get install curl --yes
RUN curl -sL https://deb.nodesource.com/setup_6.x | bash -
RUN apt-get install -y nodejs

# Elm needs this and the base ubuntu images does not have it
RUN apt-get install netbase --yes

RUN npm i -g yarn@0.24.6

WORKDIR /opt/app


CMD yarn install && yarn build && yarn server

# CMD yarn build && yarn server