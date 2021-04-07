# How to start

1. add nginx ingress to minikube

    `https://kubernetes.io/docs/tasks/access-application-cluster/ingress-minikube/`

2. start up all deployments and services by executing file `\k8s\start.ps1`

use `minikube ip` to get the ip of the cluster.

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