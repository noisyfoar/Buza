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

## Выкладка на VPS REG.RU

Пошаговая инструкция для Ubuntu 24.04, Docker Compose и HTTPS по публичному IP находится в [docs/deploy-vps-reg-ru.md](docs/deploy-vps-reg-ru.md).

## Администратор

- Логин: `admin`
- Локальная разработка: пароль по умолчанию `admin123`.
- VPS: задайте `ADMIN_INITIAL_PASSWORD` в `.env.vps`; значение требуется только для первичного создания администратора.
- Панель: `/admin.html`

## Основные API

- `GET /api/products`
- `GET /api/products/{id}`
- `GET /api/events`
- `GET /api/settings`
- `POST /api/auth/login`
- `GET|POST|PUT|DELETE /api/admin/products`
- `GET|POST|PUT|DELETE /api/admin/events`
- `POST /api/admin/events/{id}/images` — добавить несколько фотографий к публикации
- `DELETE /api/admin/events/{id}/images/{imageId}` — удалить фотографию
- `PUT /api/admin/settings`

Изображения загружаются в `wwwroot/uploads`, а в базе хранятся только пути к файлам.

## Тестирование

Интеграционные тесты контроллеров, JWT, SQLite, товаров и настроек:

```powershell
dotnet test .\Buza.Tests\Buza.Tests.csproj
```

E2E-тесты переходов и админского управления товарами:

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
