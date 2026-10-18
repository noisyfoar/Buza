# Выкладка на VPS REG.RU по IP с HTTPS

Для конфигурации используется Ubuntu 24.04 LTS, Docker Compose и Caddy. Домен не обязателен: Caddy получает краткосрочный публичный сертификат Let’s Encrypt для публичного IP и автоматически обновляет его. Сертификаты IP у Let’s Encrypt рассчитаны примерно на шесть дней, поэтому автоматическое продление и постоянный контейнер Caddy обязательны.

## 1. Подготовьте сервер и код

В панели REG.RU создайте облачный сервер с Ubuntu 24.04 LTS и добавьте SSH-ключ. Подключитесь по SSH к IPv4, указанному в карточке сервера. Официальная инструкция REG.RU: [подключение к облачному серверу](https://help.reg.ru/support/videospravka/servery-vpss/podklyucheniye-k-serveru-po-ssh-sftp-i-vnc).

Установите Git, `rsync` и Docker Engine с Compose plugin по [официальной инструкции Docker для Ubuntu](https://docs.docker.com/engine/install/ubuntu/). Откройте входящие порты **22/tcp**, **80/tcp**, **443/tcp** и **443/udp** в сетевом экране REG.RU и в UFW, если он включён. Порты 80 и 443 нужны для проверки домена/IP и HTTPS; приложение само по себе наружу порт не публикует.

Текущий рабочий каталог содержит изменения, ещё не отправленные в GitHub. Чтобы сервер получил именно эту версию, сначала проверьте `git status`, затем зафиксируйте нужные изменения и отправьте их в ветку репозитория. Не добавляйте файл `tunnel.pid` в коммит. После этого на VPS:

```bash
git clone https://github.com/noisyfoar/Buza.git
cd Buza
```

Если репозиторий закрытый, настройте доступ по SSH-ключу или используйте другой приватный способ клонирования; не помещайте GitHub-токен в URL команды.

## 2. Задайте IP и секреты

Создайте файл окружения из шаблона и закройте к нему доступ другим пользователям:

```bash
cp .env.vps.example .env.vps
chmod 600 .env.vps
```

Сгенерируйте два случайных секрета:

```bash
openssl rand -base64 48
openssl rand -base64 24
```

В `.env.vps` замените `SITE_ADDRESS` на **публичный IPv4 VPS**, `ACME_EMAIL` на свой адрес почты, первый случайный секрет укажите как `JWT_SECRET`, второй — как `ADMIN_INITIAL_PASSWORD`. Для `SITE_ADDRESS` пишется только IP, без `http://`, `https://` и порта.

## 3. Запустите сайт

```bash
docker compose --env-file .env.vps -f docker-compose.vps.yml up -d --build
docker compose --env-file .env.vps -f docker-compose.vps.yml ps
docker compose --env-file .env.vps -f docker-compose.vps.yml logs -f caddy app
```

После получения сертификата откройте `https://<публичный-IP>/`. Вход в админку: `https://<публичный-IP>/admin.html`, логин `admin`, пароль — значение `ADMIN_INITIAL_PASSWORD`. При первом запуске сервер требует пароль не короче 16 символов. Если база уже содержит администратора, начальный пароль из `.env.vps` не сбрасывает его.

Сайт и административный API доступны только за HTTPS-прокси Caddy. База SQLite и загруженные изображения хранятся в Docker volume `buza_vps_data`; не запускайте `docker compose down -v`, если хотите сохранить их.

## 4. Когда появится домен

Направьте DNS-запись A домена на IPv4 VPS, откройте `.env.vps`, замените `SITE_ADDRESS` на доменное имя и пересоздайте контейнеры:

```bash
docker compose --env-file .env.vps -f docker-compose.vps.yml up -d
```

Caddy получит сертификат для домена и будет перенаправлять HTTP на HTTPS.

## Обновление кода

После отправки новых изменений в GitHub выполните на VPS:

```bash
git pull
docker compose --env-file .env.vps -f docker-compose.vps.yml up -d --build
```

Миграции базы применяются приложением при старте. Сделайте резервную копию перед обновлением, если база содержит важные публикации или настройки.

## Автоматический деплой из GitHub

Файл `.github/workflows/deploy-vps.yml` запускается при каждом `git push` в ветку `master` и по кнопке **Run workflow** в GitHub Actions. Он синхронизирует checkout в `/opt/buza`, сохраняет серверный `.env.vps`, затем пересобирает и запускает Docker Compose. Каталог `/opt/buza` должен быть выделен только под этот проект: синхронизация удаляет из него файлы, которых больше нет в репозитории. SQLite и загруженные фотографии остаются в Docker volume.

### Один раз подготовьте пользователя деплоя на сервере

Создайте отдельный ключ для GitHub Actions на Windows в PowerShell. На запрос пароля ключа нажмите Enter два раза, чтобы оставить пароль пустым: GitHub Actions не сможет интерактивно его запросить.

```powershell
ssh-keygen -t ed25519 -C "buza-github-actions" -f "$env:USERPROFILE\.ssh\buza_github_actions"
Get-Content "$env:USERPROFILE\.ssh\buza_github_actions.pub"
```

Скопируйте выведенную строку публичного ключа. В уже открытом SSH-сеансе `root` на VPS выполните:

```bash
adduser --disabled-password --gecos "" buza-deploy
usermod -aG docker buza-deploy
install -d -m 700 -o buza-deploy -g buza-deploy /home/buza-deploy/.ssh
nano /home/buza-deploy/.ssh/authorized_keys
```

Вставьте в `nano` скопированную строку публичного ключа, сохраните `Ctrl+O`, Enter и выйдите `Ctrl+X`. Затем выполните:

```bash
chown buza-deploy:buza-deploy /home/buza-deploy/.ssh/authorized_keys
chmod 600 /home/buza-deploy/.ssh/authorized_keys
install -d -m 755 -o buza-deploy -g buza-deploy /opt/buza
```

Создайте папку проекта и `.env.vps` на сервере (значения секретов сгенерируйте по шагу 2 выше):

```bash
install -d -m 755 -o buza-deploy -g buza-deploy /opt/buza
nano /opt/buza/.env.vps
```

Вставьте в файл `SITE_ADDRESS`, `ACME_EMAIL`, `JWT_SECRET` и `ADMIN_INITIAL_PASSWORD`, сохраните и выйдите. Убедитесь, что файл принадлежит пользователю деплоя и доступен только ему:

```bash
chown buza-deploy:buza-deploy /opt/buza/.env.vps
chmod 600 /opt/buza/.env.vps
```

Установите `rsync`, если это ещё не сделано (`apt install -y rsync`). Выйдите из root-сеанса и проверьте новый ключ с Windows:

```powershell
ssh -i "$env:USERPROFILE\.ssh\buza_github_actions" buza-deploy@89.111.152.194
```

### Добавьте значения в GitHub

В репозитории откройте **Settings → Secrets and variables → Actions**.

В разделе **Variables** добавьте:

- `VPS_HOST` = `89.111.152.194`
- `VPS_USER` = `buza-deploy`

В разделе **Secrets** добавьте:

- `VPS_SSH_KEY` = всё содержимое файла `buza_github_actions` (без `.pub`), включая строки `BEGIN OPENSSH PRIVATE KEY` и `END OPENSSH PRIVATE KEY`;
- `VPS_KNOWN_HOSTS` = результат команды `ssh-keyscan -H 89.111.152.194`, запущенной в PowerShell.

Приватный ключ добавляйте только в GitHub Secret. Не помещайте его в репозиторий и не отправляйте в чат. После первого сохранения файлов в репозитории откройте **Actions** и проверьте запуск `Deploy Buza to VPS`. Следующие push в `master` будут запускать выкладку автоматически.
