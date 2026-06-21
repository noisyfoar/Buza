# Напиток здоровья и молодости — буза

Веб-сайт стартап-проекта на ASP.NET Core Web API, SQLite и чистом HTML/CSS/JavaScript.

## Запуск

1. Установите .NET 10 SDK или .NET 10 Runtime.
2. В папке проекта выполните:

```powershell
dotnet restore
dotnet run
```

3. Откройте адрес из консоли. По умолчанию в `Properties/launchSettings.json` указан `http://localhost:5179`.

При первом запуске приложение применит EF Core миграции, создаст файл `database.db`, начальные настройки, демо-товары и администратора.

## Администратор

- Логин: `admin`
- Пароль: `admin123`
- Панель: `/admin.html`

## Основные API

- `GET /api/products`
- `GET /api/products/{id}`
- `GET /api/settings`
- `POST /api/feedback`
- `POST /api/auth/login`
- `GET|POST|PUT|DELETE /api/admin/products`
- `GET|PUT /api/admin/feedback`
- `PUT /api/admin/settings`

Изображения загружаются в `wwwroot/uploads`, а в базе хранятся только пути к файлам.

## Тестирование

Интеграционные тесты контроллеров, JWT, SQLite, заявок, товаров и настроек:

```powershell
dotnet test .\Buza.Tests\Buza.Tests.csproj
```

E2E-тесты переходов, кнопок, формы заявки и админского создания товара:

```powershell
npm install
npx playwright install chromium
npm run test:e2e
```

Полный прогон:

```powershell
npm run test:all
```

Playwright запускает сайт с отдельной временной SQLite-базой, поэтому E2E-тесты не изменяют рабочий `database.db`.
