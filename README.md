# EShoppingZone Backend Microservices

A scalable microservices-based backend for the **EShoppingZone E-Commerce Platform**, built with **.NET 8, ASP.NET Core Web API, Entity Framework Core, SQL Server, JWT Authentication, and Docker**.

This backend powers customer shopping, merchant product management, order processing, wallet payments, authentication, and administrative operations.

---

# Architecture Overview

The backend follows a **microservices architecture**, where each service owns its business domain, database context, and API endpoints.

## Microservices

- **Profile Service** → User authentication, profiles, roles, addresses
- **Product Service** → Product catalog management
- **Cart Service** → Shopping cart operations
- **Order Service** → Checkout, orders, status tracking
- **Wallet Service** → Digital wallet and transactions
- **API Gateway** → Unified entry point for all backend services

---

# Tech Stack

| Layer | Technology |
|------|------------|
| Framework | .NET 8 |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Authentication | JWT + GitHub OAuth |
| API Gateway | YARP Reverse Proxy |
| Inter-Service Communication | IHttpClientFactory |
| Logging | Serilog |
| Containerization | Docker |
| Orchestration | Docker Compose / Kubernetes |
| CI/CD | GitHub Actions |

---

# High-Level Architecture

```mermaid
graph TD
    Client[React Frontend]

    subgraph "EShoppingZone Microservices (.NET 8)"
        ApiGateway[API Gateway<br/>YARP Reverse Proxy]
        Profile[Profile API]
        Product[Product API]
        Cart[Cart API]
        Order[Order API]
        Wallet[Wallet API]
    end

    subgraph "Databases (SQL Server)"
        DB_Profile[(Profile DB)]
        DB_Product[(Product DB)]
        DB_Cart[(Cart DB)]
        DB_Order[(Order DB)]
        DB_Wallet[(Wallet DB)]
    end

    Client --> ApiGateway
    
    ApiGateway --> Profile
    ApiGateway --> Product
    ApiGateway --> Cart
    ApiGateway --> Order
    ApiGateway --> Wallet

    Profile --> DB_Profile
    Product --> DB_Product
    Cart --> DB_Cart
    Order --> DB_Order
    Wallet --> DB_Wallet
```
