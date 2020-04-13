# AWS SSM Parameter Store demo

Example of using AWS Parameter Store as backend for multi tenant application config.

## Usage

1. Create parameters to Parameter store

```
/my-app/tenant1/dbuser    tenant1dbuser
/my-app/tenant1/dbpass    tenant1dbpass
/my-app/tenant2/dbuser    tenant2dbuser
/my-app/tenant2/dbpass    tenant2dbpass
```
2. Configure used AWS account as you like

```
export AWS_ACCESS_KEY_ID=yourawsaccesskeyid
export AWS_SECRET_ACCESS_KEY=yourawssecretaccesskey
export AWS_REGION=eu-west-1
```

3. Retrieve configuration

```
dotnet run [tenant]
```

See files under `data` directory to be filled with the key-value pairs retrieved from the Parameter store. Those JSON files are used as a cache so the next configuration query is hit from the cache.

```json
{"dbpass":"tenant1dbpass","dbuser":"tenant1dbuser"}
```