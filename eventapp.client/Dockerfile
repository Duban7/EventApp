FROM node:20 as dev

WORKDIR /app
COPY ["eventapp.client/package.json", "."]
COPY ["eventapp.client/package-lock.json", "."]

RUN npm install -g @angular/cli
RUN npm ci

COPY ["eventapp.client","."] 

CMD ["ng", "serve", "--host", "0.0.0.0"]
