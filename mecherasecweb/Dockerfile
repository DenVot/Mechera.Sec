FROM alpine:3.14

EXPOSE 3000/tcp

COPY . /app
RUN apk add nodejs npm
WORKDIR /app
RUN npm install && \
    npm install -g serve && \
    npm run build

CMD serve -s build