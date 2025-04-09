FROM denoland/deno:1.44.4

RUN apt-get update && \
   apt-get install curl -y

WORKDIR /app

VOLUME /app/dat
VOLUME /app/data

COPY . .

RUN deno cache main.ts

CMD [ "deno", "run", "-A", "main.ts" ]

