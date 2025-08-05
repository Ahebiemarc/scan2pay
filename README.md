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



<img width="479" height="1025" alt="Capture d'écran 2025-05-07 133943" src="https://github.com/user-attachments/assets/cb483f2f-2350-4bf5-b364-23011bc06b73" />
<img width="503" height="1026" alt="Capture d'écran 2025-05-07 133957" src="https://github.com/user-attachments/assets/ce86ec49-a7b7-4b3a-b2b3-d3163a14f333" />
<img width="497" height="1022" alt="Capture d'écran 2025-05-07 134031" src="https://github.com/user-attachments/assets/40d50548-823c-45cb-ac76-a3885fa6c6ef" />
<img width="494" height="1028" alt="Capture d'écran 2025-05-07 134011" src="https://github.com/user-attachments/assets/912831ce-5e68-4d6d-a048-181d7d7b49e0" />
<img width="497" height="1021" alt="Capture d'écran 2025-05-07 134058" src="https://github.com/user-attachments/assets/1d6e1f11-0d49-482a-8f5f-b82c60a1eebd" />
<img width="495" height="1022" alt="Capture d'écran 2025-05-07 134116" src="https://github.com/user-attachments/assets/4cc452e0-6e36-4403-b9a3-d0c12e0ff3cf" />
<img width="503" height="1024" alt="Capture d'écran 2025-05-07 134133" src="https://github.com/user-attachments/assets/7f302cff-bcf2-43fd-a022-18a9ed51733d" />
<img width="498" height="1023" alt="Capture d'écran 2025-05-07 134246" src="https://github.com/user-attachments/assets/aa18fd5a-abfe-4f4c-9423-5ab39ed5dde2" />
<img width="499" height="1017" alt="Capture d'écran 2025-05-07 134402" src="https://github.com/user-attachments/assets/e94ced47-3ce0-4331-8e63-18312f51b442" />
<img width="490" height="1017" alt="Capture d'écran 2025-05-07 134423" src="https://github.com/user-attachments/assets/a795de1f-c591-4443-90b5-2b8d710fde1e" />
<img width="501" height="1018" alt="Capture d'écran 2025-05-07 134529" src="https://github.com/user-attachments/assets/543f860b-457b-463a-a53b-56cf38908359" />
<img width="508" height="1022" alt="Capture d'écran 2025-05-07 134619" src="https://github.com/user-attachments/assets/2cb21df2-2f60-4fcf-b412-82ec2cc5c92b" />
![WhatsApp Image 2025-08-05 à 02 33 51_6eab602b](https://github.com/user-attachments/assets/080281b6-32ce-457c-b67d-9a36ff470a87)
![WhatsApp Image 2025-08-05 à 02 33 52_a470d0a4](https://github.com/user-attachments/assets/55ceceb8-acd5-41b6-8c60-6cbdbd63e5f6)
![WhatsApp Image 2025-08-05 à 02 33 53_e0669cff](https://github.com/user-attachments/assets/1d144317-7d1b-464c-96f4-ddff47f08b88)
![WhatsApp Image 2025-08-05 à 02 33 51_80cebf4d](https://github.com/user-attachments/assets/cdaa29b8-2b56-4176-ad14-c2a5fc94d88b)
![WhatsApp Image 2025-08-05 à 02 33 52_bb9b4350](https://github.com/user-attachments/assets/f8484b25-d5ac-4fa3-ad99-ce009b0e4669)
![WhatsApp Image 2025-08-05 à 02 33 52_08f8b558](https://github.com/user-attachments/assets/fe7216e5-636e-4d96-bcc7-4655774385b5)
![WhatsApp Image 2025-08-05 à 02 42 20_7b0b73a4](https://github.com/user-attachments/assets/f0acc655-7569-44c7-868d-583fd4d3b7f7)

























