# 🎓 الإنجليزية مع ديما | English with Dima

منصة تعليم اللغة الإنجليزية للطلاب المصريين — اختبارات تفاعلية وألعاب تعليمية مصممة لمنهج وزارة التربية والتعليم.

An interactive English learning platform for Egyptian students — featuring smart tests, educational games, and personalized learning.

---

## ✨ Features

- 📝 **Smart Tests** — 11 question types (MCQ, True/False, Fill-in, Matching, etc.)
- 🎮 **Educational Games** — Matching, Wheel of Knowledge, Drag & Drop, Flip Cards
- 📊 **Admin Dashboard** — Manage questions, tests, and games
- 🌍 **Full Arabic RTL** — Complete right-to-left Arabic interface
- 🔐 **Authentication** — Student login, teacher login, guest access
- 📱 **Responsive Design** — Works on desktop, tablet, and mobile

---

## 🏗️ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | Angular 19, TypeScript, SCSS |
| **Backend** | ASP.NET Core 8, C# |
| **Database** | SQL Server + Entity Framework Core |
| **Auth** | JWT + ASP.NET Identity |
| **i18n** | ngx-translate (Arabic/English) |
| **Real-time** | SignalR |

---

## 📁 Project Structure

```
englishwithdima/
├── client/                  # Angular frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/        # Services, guards, interceptors
│   │   │   ├── pages/       # Home, Tests, Games, Admin, Auth
│   │   │   └── shared/      # Layout (Header, Footer)
│   │   └── assets/i18n/     # ar.json, en.json translations
│   └── package.json
├── src/                     # .NET backend
│   ├── EnglishPlatform.API/          # Controllers, Program.cs
│   ├── EnglishPlatform.Application/  # Services, DTOs, Mappings
│   ├── EnglishPlatform.Domain/       # Entities, Enums
│   ├── EnglishPlatform.Infrastructure/ # DbContext, Repositories
│   └── EnglishPlatform.Shared/       # Result wrapper
└── EnglishPlatform.sln
```

---

## 🚀 Getting Started

### Prerequisites
- [Node.js](https://nodejs.org/) (v18+)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full)

### Frontend
```bash
cd client
npm install
ng serve
```

### Backend
```bash
cd src/EnglishPlatform.API
dotnet restore
dotnet run
```

---

## 👨‍🏫 Admin Access

The admin dashboard is accessible at `/admin` (requires Teacher role login).

- **Dashboard** — Overview stats and quick actions
- **Questions** — Create/edit questions with multiple types
- **Tests** — Create, publish, and manage tests
- **Games** — Create matching, drag & drop, and flip card games

---

## 📄 License

This project is private. All rights reserved © 2024 English with Dima.
