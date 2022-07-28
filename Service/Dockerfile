FROM ubuntu:18.10
RUN apt-get update
RUN apt-get install -y python3 python3-dev python3-pip
RUN pip3 install uwsgi
COPY ./ /app
WORKDIR /app
RUN pip3 install -r requirements.txt
CMD uwsgi --socket 0.0.0.0:80 --protocol=http -w application:app