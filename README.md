# Ping-Tower / API

> Part of the **Ping-Tower** uptime monitoring system.
> 📖 [Full project description and architecture →](https://github.com/Ping-Tower)

API gateway of the system. Accepts requests from the web client, manages server monitoring configuration, streams real-time status updates via WebSocket, and routes notification events to delivery workers.

## Stack

- **C# / ASP.NET Core 10** — Clean Architecture (Domain → Application → Infrastructure → Presentation)
- **MediatR** — CQRS, pipeline behaviors (validation, transaction, logging)
- **Entity Framework Core + PostgreSQL** — transactional data: users, servers, ping settings, tokens
- **SignalR** — WebSocket hub for real-time status push to the frontend
- **RabbitMQ** — publishes server lifecycle events; consumes status change events
- **ClickHouse** — read-only queries for ping history, uptime stats, and chart data
- **Redis** — per-user notification cooldown store

## Role in the system

```
Frontend (Vue/React)
    │  REST + WebSocket (SignalR)
    ▼
  [ API ]  ──publishes──▶  serverEventsExchange  ──▶  Ping Service
    │                                             ──▶  State Elevator
    │
    ├── consumes ◀── statusEventsExchange ◀── State Elevator
    │       └── updates PostgreSQL, pushes SignalR, sends notifications
    │
    ├── publishes ──▶ emailQueue    ──▶  Email Sender
    └── publishes ──▶ telegramQueue ──▶  Telegram Bot
```

## Endpoints

**Auth** `POST /api/auth/{register, login, logout, refresh, verify-email, forgot-password, reset-password, resend-verification-code}`

**Servers**
| Method | Path | Description |
|---|---|---|
| `GET` | `/api/servers` | List servers (with optional `?search=`) |
| `POST` | `/api/servers` | Create server |
| `GET` | `/api/servers/{id}` | Get server by id |
| `PUT` | `/api/servers/{id}` | Update server |
| `DELETE` | `/api/servers/{id}` | Delete server |
| `GET` | `/api/servers/{id}/settings` | Get ping settings |
| `PATCH` | `/api/servers/{id}/settings` | Update ping settings |
| `GET` | `/api/servers/{id}/state` | Get current status from Redis |
| `GET` | `/api/servers/{id}/pings` | Ping history from ClickHouse |
| `GET` | `/api/servers/{id}/overview` | Aggregated chart + uptime stats |

**Users**
| Method | Path | Description |
|---|---|---|
| `GET` | `/api/users/me` | Current user profile |
| `GET` | `/api/users/notification-settings` | Notification preferences |
| `PATCH` | `/api/users/notification-settings` | Update preferences |

**Telegram** `GET/POST/DELETE /api/telegram-accounts`

**WebSocket** `ws://.../hubs/monitoring` — SignalR hub, emits `ServerStatusChanged` events

## Running

```bash
cp .env.example .env
# fill in connection strings, JWT secret, SMTP credentials, RabbitMQ URL

docker compose up
```

The service runs on port **8080** and exposes a Swagger UI at `/swagger`.

## Tests

```bash
# unit tests
dotnet test tests/Api.UnitTests

# integration tests (requires running infra)
dotnet test tests/Api.IntegrationTests
```