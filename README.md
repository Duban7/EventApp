## 1. Клонирование репозитория

```bash
git clone <URL_репозитория>
cd <папка_проекта>
```

## 2. Настройте строку подключения

``` appsettings.json
......
 "DbConnectionString": "Server=DUBAN\\SQLEXPRESS;Database=EventAppDb;Trusted_Connection=True;TrustServerCertificate=True;",
......
```

## 3. Применение миграции

```bash
dotnet ef database update
```

## 4. Запуск проекта

```bash
dotnet restore
dotnet build
dotnet run
```
