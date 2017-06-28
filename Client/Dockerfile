FROM ubuntu:16.04

RUN mkdir -p /opt/app/src
VOLUME /opt/app

RUN apt-get update
RUN apt-get install curl --yes
RUN curl -sL https://deb.nodesource.com/setup_6.x | bash -
RUN apt-get install -y nodejs

RUN npm i -g yarn
RUN apt-get install netbase --yes

WORKDIR /opt/app

CMD yarn install && yarn build && yarn live

# CMD yarn build && yarn server