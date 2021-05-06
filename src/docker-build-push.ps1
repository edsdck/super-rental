docker build -t edsdck/rental-system-rentals:1.0 .\Rentals
docker push edsdck/rental-system-rentals:1.0

docker build -t edsdck/rental-system-reservations:1.0 .\Reservations
docker push edsdck/rental-system-reservations:1.0

docker build -t edsdck/rental-system-gateway:1.0 .\Gateway
docker push edsdck/rental-system-gateway:1.0

docker build -t edsdck/rental-system-identity:1.0 .\IdentityServer
docker push edsdck/rental-system-identity:1.0

docker build -t edsdck/rental-system-mvc:1.0 .\WebApplication
docker push edsdck/rental-system-mvc:1.0