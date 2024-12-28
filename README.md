dotnet new console
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet tool install --global dotnet-ef
dotnet build
dotnet ef dbcontext scaffold "server=hcwilli.at;port=3306;user=d0424dc5;password=3QHu9nnDesLrDbKF44vN;database=d0424dc5" Pomelo.EntityFrameworkCore.MySql -c MyCmdbContext -o Context