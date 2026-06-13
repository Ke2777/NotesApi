# NotesApi

ASP.NET Core 8 приложение для заметок с PostgreSQL. Проект можно запускать на Ubuntu сервере через Docker Compose: Compose поднимает и приложение, и базу данных.

## Что будет на сервере

- `notes-api` - контейнер с ASP.NET Core приложением.
- `postgres` - контейнер с PostgreSQL.
- `postgres-data` - Docker volume с данными базы.

PostgreSQL отдельно устанавливать на сервер не нужно. База создается контейнером `postgres`, а данные сохраняются в volume и не удаляются при обновлении приложения.

## Первичная установка на Ubuntu

Установи Docker и Docker Compose plugin:

```bash
sudo apt update
sudo apt install -y ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

Склонируй проект на сервер и перейди в папку проекта:

```bash
git clone <URL_РЕПОЗИТОРИЯ>
cd NotesApi
```

Создай файл с настройками окружения:

```bash
cp .env.example .env
nano .env
```

Пример `.env`:

```env
POSTGRES_DB=notes_db
POSTGRES_USER=notes_user
POSTGRES_PASSWORD=change-this-long-random-password
APP_PORT=8080
```

Поменяй `POSTGRES_PASSWORD` на длинный случайный пароль. Остальные значения можно оставить как есть.

Запусти приложение:

```bash
docker compose up -d --build
```

Проверить логи:

```bash
docker compose logs -f notes-api
```

По умолчанию приложение будет доступно по адресу:

```text
http://SERVER_IP:8080
```

Если нужно использовать другой порт, измени `APP_PORT` в `.env`.

## Где вводятся данные от БД

Данные от БД вводятся в `.env`:

```env
POSTGRES_DB=notes_db
POSTGRES_USER=notes_user
POSTGRES_PASSWORD=your-password
```

`docker-compose.yml` передает их и контейнеру PostgreSQL, и приложению:

```env
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=...;Username=...;Password=...
```

Менять строку подключения в `appsettings.json` для Docker-деплоя не нужно.

## Обновление проекта

Перейди в папку проекта на сервере:

```bash
cd NotesApi
```

Забери новую версию кода:

```bash
git pull
```

Пересобери и перезапусти контейнер приложения:

```bash
docker compose up -d --build
```

Проверь логи:

```bash
docker compose logs -f notes-api
```

База данных при этом не удаляется, потому что хранится в volume `postgres-data`.

## Миграции базы данных

В `docker-compose.yml` включено:

```yaml
ApplyMigrations: "true"
```

При старте приложение само применяет EF Core миграции к PostgreSQL. Отдельно запускать `dotnet ef database update` на сервере не нужно.

## Остановка и запуск

Остановить контейнеры:

```bash
docker compose down
```

Запустить снова:

```bash
docker compose up -d
```

Посмотреть состояние:

```bash
docker compose ps
```

## Бэкап базы данных

Создать бэкап:

```bash
docker compose exec -T postgres sh -c 'pg_dump -U "$POSTGRES_USER" "$POSTGRES_DB"' > notes_backup.sql
```

Восстановить бэкап:

```bash
docker compose exec -T postgres sh -c 'psql -U "$POSTGRES_USER" "$POSTGRES_DB"' < notes_backup.sql
```

## HTTPS и домен

Для домена и HTTPS поставь перед приложением Nginx или Caddy и проксируй запросы на:

```text
http://127.0.0.1:8080
```

В этом проекте HTTPS-редирект в production выключен, потому что TLS лучше завершать на reverse proxy.

## Полезные команды

```bash
docker compose ps
docker compose logs -f notes-api
docker compose logs -f postgres
docker compose restart notes-api
docker compose up -d --build
```
