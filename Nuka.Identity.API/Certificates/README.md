**How to create .pfx file:**
```shell
$ openssl req -newkey rsa:2048 -nodes -keyout idsrv4dev.key -x509 -days 3650 -out idsrv4dev.cer
$ openssl pkcs12 -export -in idsrv4dev.cer -inkey idsrv4dev.key -out idsrv4dev.pfx
```