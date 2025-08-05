# 📦 Scan2Pay API

Cette API permet de gérer les fonctionnalités d'une application de paiement par scan. Elle est construite avec **ASP.NET Core**, **Entity Framework Core**, et utilise **PostgreSQL** comme base de données via Docker.

---

## 🚀 Fonctionnalités

- API REST en ASP.NET Core
- Base de données PostgreSQL
- Migrations avec Entity Framework Core
- QR Code support
- Docker Compose pour l’environnement de développement

---

## ⚙️ Prérequis

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- [Git](https://git-scm.com/)
- (Optionnel) [pgAdmin](https://www.pgadmin.org/) ou [TablePlus](https://tableplus.com/) pour visualiser la base de données

---

## 🐳 Lancer la base de données avec Docker

1. Cloner le projet :

```bash
git clone https://github.com/ton-compte/scan2pay_api.git
cd scan2pay_api


2 - Lancer la base de données et pgAdmin :

docker-compose up -d


pgAdmin est disponible sur http://localhost:5050


Installer les dépendances
Depuis le dossier du projet (où se trouve le .csproj) :

dotnet restore



Appliquer les migrations EF Core
Si c’est la première fois qu’on utilise le projet :

dotnet tool install --global dotnet-ef
dotnet ef database update


Lancer l'API

dotnet watch run --launch-profile https


Ajouter une nouvelle migration (si tu modifies le modèle):

dotnet ef migrations add NomDeLaMigration
dotnet ef database update


























