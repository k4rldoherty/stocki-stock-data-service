# Stocki API

## About

- My original discord bot api, just seperating it out into microservices type architechture before adding features so i can reuse the api in other projects etc.

## How to get it working

- Pull the git repo
- Get an API key from finnhub and alpha vantage. (Free)
- Generate a Https cert
- Use `docker build -t stockdataservice .`

## Command to run to start the application

docker run -p 8080:8080 \
-p 8081:8081 \
-v ~/.https-cert:/https \
-e HTTPSCERTPASSWORD=<YOUR CERT PASSWORD> -e FHAPI=<YOUR FINNHUB API> \
-e ALPHAAPI=<YOUR ALPHA API> stockdataservice

## Commands to get a https cert

### Get Cert and key

openssl req -x509 \
-newkey rsa:4096 \
-keyout https-key.pem \
-out https-cert.pem \
-days 730

### Ensure program has access to read certs

chmod 644 /path/to/certs/\*

### Turn into useable file (this password is the password for above)

openssl pks12 -export \
n-out https-cert.pfx \
n-inkey https-key.pem \
n-in https-cert.pem
