FROM alpine:3.11

RUN apk update && apk upgrade

RUN apk add --no-cache --update clang cmake git bash make alpine-sdk

# libraries

RUN mkdir -p /opt
ENV CXX=clang++
ENV CC=clang-9

# - nlohmann/json
RUN cd /opt && git clone --depth 1 --branch v3.3.0 https://github.com/nlohmann/json.git
# RUN cd /opt/json && cmake -G Ninja . && cmake --build .

RUN clang --version && ls /usr/bin