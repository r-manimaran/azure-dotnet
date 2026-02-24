# Using AzureKeyVault Emulator to store Keys and Secret in Dev environment.

![alt text](image-3.png)

1. Get the Token from the KeyVault Emulator
```bash
https://localhost:57647/token
```
![alt text](image-1.png)

2. Store the Secret and Key using the Curl Command.
```bash

curl -X PUT "https://localhost:57647/secrets/mysecret?api-version=7.2" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJsb2NhbHVzZXIiLCJqdGkiOiIzYjdkOWQ0MS0zZjM2LTRlNGYtYWUxYi03OGI4ODNiMjNhOGQiLCJleHAiOjE3NzE5Nzc4NDQsImlzcyI6ImxvY2FsYXp1cmVrZXl2YXVsdC5sb2NhbGhvc3QuY29tIiwiYXVkIjoibG9jYWxhenVyZWtleXZhdWx0LmxvY2FsaG9zdC5jb20ifQ.wG0qLwL0PUB7cKLql_JGDZP7HgHO2II5sjV2SVcZr28" \
     -d '{"value":"mysecretvalue"}' \
     --insecure

curl -X POST "https://localhost:57647/keys/mykey/create?api-version=7.2" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJsb2NhbHVzZXIiLCJqdGkiOiIzYjdkOWQ0MS0zZjM2LTRlNGYtYWUxYi03OGI4ODNiMjNhOGQiLCJleHAiOjE3NzE5Nzc4NDQsImlzcyI6ImxvY2FsYXp1cmVrZXl2YXVsdC5sb2NhbGhvc3QuY29tIiwiYXVkIjoibG9jYWxhenVyZWtleXZhdWx0LmxvY2FsaG9zdC5jb20ifQ.wG0qLwL0PUB7cKLql_JGDZP7HgHO2II5sjV2SVcZr28" \
     -d '{"kty":"RSA"}' \
     --insecure

```

![alt text](image-2.png)


3. Retrieve the Secret and Key from the Endpoint.

https://localhost:7263/secret/mysecret

![alt text](image.png)


