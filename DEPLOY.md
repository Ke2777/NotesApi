# Deploy NotesApi on Ubuntu

## First deploy

Install Docker and the Compose plugin on the server:

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

Copy the project to the server, then create the environment file:

```bash
cp .env.example .env
nano .env
```

Set a strong `POSTGRES_PASSWORD`, then start the app:

```bash
docker compose up -d --build
docker compose logs -f notes-api
```

The app will listen on `http://SERVER_IP:8080` unless `APP_PORT` is changed.

## Updates

After changing code or pulling a new version:

```bash
git pull
docker compose up -d --build
docker compose logs -f notes-api
```

PostgreSQL data is stored in the named Docker volume `postgres-data`, so app rebuilds do not delete the database.

## Database backups

Create a backup:

```bash
docker compose exec -T postgres sh -c 'pg_dump -U "$POSTGRES_USER" "$POSTGRES_DB"' > notes_backup.sql
```

Restore a backup:

```bash
docker compose exec -T postgres sh -c 'psql -U "$POSTGRES_USER" "$POSTGRES_DB"' < notes_backup.sql
```

## Reverse proxy

For a domain and HTTPS, put Nginx or Caddy in front of the app and proxy to `127.0.0.1:8080`.
