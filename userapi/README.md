# UserApi (.NET 8) - Clean Architecture + CQRS + JWT + SQL Server (Docker)

## Rodando com Docker
1. Ajuste se necessário `src/Api/appsettings.json`.
2. `docker compose up --build`
3. A API estará em http://localhost:8001/swagger

## Endpoints
- POST /users  -> criar usuário (body { name, email, password })
- GET /users   -> lista usuários
- POST /auth/login -> { email, password } retorna { token }

## Testes
`dotnet test`
