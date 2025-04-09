FROM denoland/deno:2.3.1

RUN apt-get update && \
   apt-get install curl -y

WORKDIR /app

VOLUME /app/dat
VOLUME /app/data

COPY . .

RUN deno install

CMD [ "deno", "task", "run" ]

