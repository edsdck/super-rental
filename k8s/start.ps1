kubectl apply `
    -f ./rental-system-namespace.yaml `
    -f ./rentals-api.yaml `
    -f ./reservations-api.yaml `
    -f ./identity-api.yaml `
    -f ./gateway.yaml `
    -f ./ingress.yaml