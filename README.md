# Pereprodai

Backend маркетплейса объявлений (мини-Avito). Modular Monolith на .NET 8. (на 10й ещё не адаптировали библиотеки, но через пару месяцев можно обновить)

Проект поизучать DDD, CQRS, Elasticsearch и Redis. Есть что потыкать, поиграться.

## Запуск

**Необходимы:** Docker, .NET 8 SDK

```bash
# 1. Подымаем (а лучше сначала чистим образы, чтобы wsl не сожрала весь диск)
docker compose up -d

# 2. Запускаем прокт (миграции применяются при старте)
dotnet run --project src/Pereprodai.Api

# 3. Ничего не понимаем и выходим. Спрашиваем claude, ну что круто? Он тратит все токены ииии.. ии.. просит закинуть ему ещё денюшку :) (ну в общем всё круто, классно, и вообще поскорей бы меня взять на работку)
```

API - `http://localhost:5000`. Документация - на Scalar `/scalar`.

## Стек

- .NET 8, ASP.NET Core
- PostgreSQL + EF Core
- Elasticsearch (поиск, индексация и трата 1гб оперативки, ведь это круто, у нас же большой проект)
- Redis (кеширование поиска, счётчики просмотров)
- MediatR (CQRS + domain events)
- FluentValidation
- Docker Compose

## Любимые тестики

```bash
# Все тесты
dotnet test

# Только юнит-тесты
dotnet test tests/Pereprodai.Catalog.UnitTests

# Интеграционные тесты (нужен Docker для Testcontainers)
dotnet test tests/Pereprodai.IntegrationTests
```

## Архитектура

### Модули

Проект построен как Modular Monolith - три модуля:

```
Catalog       - объявления о продажи "кота-терминатора", с разгоном до сотки, или просто "dafadasdsad", жизненный цикл, CRUD
Moderation    - очередь модерации, approve/reject
Search        - read CQRS, индексация в Elasticsearch, поиск с кешем в Redis
```

Модули не ссылаются друг на друга. Общение через domain events (MediatR notifications).

### CQRS

- **Commands** (запись) - идут в PostgreSQL через EF Core.
- **Queries** (чтение) - поиск объявлений в Elasticsearch, остальное в PSQL

```
Command -> Handler -> PostgreSQL -> Domain Event -> Search module -> Elasticsearch
Query -> Handler -> (Elasticsearch / PostgreSQL) -> Response
```

### Структура

```
src/
  Pereprodai.Api/              - Web API, контроллеры, middleware, hosted services
  Pereprodai.Shared/           - базовые абстракции, и общий функционал
  Modules/
    Pereprodai.Catalog/        - внутри, как и в остальных Domain, Application и Infrastructure по папочкам
    Pereprodai.Moderation/
    Pereprodai.Search/
tests/
  Pereprodai.Catalog.UnitTests/
  Pereprodai.Moderation.UnitTests/
  Pereprodai.IntegrationTests/
```

### Слои внутри модуля

- **Domain** - агрегаты, value objects, бизнес-правила. Ничего не знает про БД и всякую всячину снаружи
- **Application** - команды, запросы, хэндлеры, валидаторы, DTO. Связывает domain с инфраструктурой через интерфейсы
- **Infrastructure** - реализации репозиториев, EF Core, Redis, Elasticsearch

## API

Авторизация - пока заглушка через заголовок `X-User-Id`.

### Listings (`/api/listings`)

- Создание, редактирование, архивация объявлений
- Отправка на модерацию
- Просмотр объявления
- Список своих объявлений с фильтрами

### Moderation (`/api/moderation`)

- Очередь модерации
- Одобрение / отклонение (с причиной). И с отклонением уже опубликованного объявления (а то вдруг передумали)

### Search (`/api/search`)

- Полнотекстовый поиск по объявлениям
- Фильтры, сортировка, пагинация

## Паттерны, прикольчики и что-то ещё :)

### Domain Events

Агрегаты собирают всякие события, которые вылупляются после `SaveChanges`. Связывает модули без прямых зависимостей.

К примеру: `Ad.Publish()` -> `AdPublishedEvent` -> Search module индексирует в Elasticsearch.

### Write-Behind (счётчик просмотров)

Просмотры пишутся в Redis (`INCR`), а не в PostgreSQL. Dirty-tracking через Redis отслеживает изменённые ID. `BackgroundService` сервис `ViewCountFlushService` раз в 5 мин обновляет накопленные счётчики в PostgreSQL

### Redis-кеш поиска

Результаты поиска кешируются в Redis на 5 минут. Кеш инвалидируется при публикации, обновлении или архивации объявления