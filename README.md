# How to start

1. add nginx ingress to kubernetes cluster 

[instructions here](https://kubernetes.github.io/ingress-nginx/deploy/#provider-specific-steps)

2. start up all deployments and services by executing file `\k8s\start.ps1`

# Migrations

```
dotnet ef migrations add [NAME] -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
```

```
dotnet ef migrations add [NAME] -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
```

```
dotnet ef migrations add [NAME] -c ApplicationDbContext -o Data/Migrations/Identity
```

## Connect to host's SQL Server from Kubernetes cluster (minikube)

Local SQL Express setup instructions:
`(Enable TCP/IP connections) -> (Enable SQL Server authentication) -> (Create a database and a database user)`

https://www.papercut.com/support/resources/manuals/ng-mf/common/topics/ext-db-specific-ms-sql-express.html

Create an user to login to.

```sql
CREATE DATABASE rental_system_identity;

USE rental_system_identity;

CREATE LOGIN your_user WITH PASSWORD = '123456';
CREATE USER your_user FOR LOGIN your_user;
EXEC sp_addrolemember 'db_owner', 'your_user'