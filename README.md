# EShoppingZone Backend Microservices

A scalable microservices-based backend for the **EShoppingZone E-Commerce Platform**, built with **.NET 8, ASP.NET Core Web API, Entity Framework Core, SQL Server, JWT Authentication, and Docker**.

This backend powers customer shopping, merchant product management, order processing, wallet payments, authentication, and administrative operations.

---

## System Use Case Diagram

The following diagram outlines the overarching use cases across the EShoppingZone platform, mapping system actors (Customer, Merchant, Admin) to their respective capabilities.

```mermaid
graph TD
    Customer[Customer]
    Merchant[Merchant]
    Admin[Admin]
    System[System Worker]

    Customer --> Profile[Manage Profile & Addresses]
    Customer --> Shop[Browse & Search Products]
    Customer --> Cart[Manage Shopping Cart]
    Customer --> Order[Place & Track Orders]
    Customer --> Wallet[Add Funds & Pay with Wallet]

    Merchant --> ManageProducts[Manage Product Catalog]
    Merchant --> ViewOrders[View Customer Orders]

    Admin --> Users[Manage Users & Roles]
    Admin --> SystemMonitor[Monitor System Health]

    System --> Process[Process Payments]
```

---

## Microservices Architecture & Connectivity

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

Each service follows a consistent layered structure:

```
ServiceName/
├── Controllers/      # HTTP endpoints
├── Services/         # Business logic
├── Repositories/     # Data access layer
├── Entities/         # EF Core models
├── DTOs/             # Request / response models
├── Data/             # DbContext
├── Middlewares/      # Global exception handling
└── Migrations/       # EF Core migrations
```

---

## Microservice Internal Workflows (File-to-File Flow)

The following diagrams illustrate the internal execution flow of files within each microservice when an API request is received. The architecture follows a strict Controller -> Service -> Repository -> Database pattern to ensure separation of concerns.

### 1. Profile Service Flow
```mermaid
graph TD
    Router[HTTP Request] --> C[Controllers<br/>ProfileController.cs]
    C --> S[Services<br/>ProfileService.cs]
    S --> R[Repositories<br/>ProfileRepository.cs]
    R --> DB_CTX[Data<br/>AppDbContext.cs]
    DB_CTX --> DB[(SQL Server<br/>Profile DB)]
```

### 2. Product Service Flow
```mermaid
graph TD
    Router[HTTP Request] --> C[Controllers<br/>ProductController.cs]
    C --> S[Services<br/>ProductService.cs]
    S --> R[Repositories<br/>ProductRepository.cs]
    R --> DB_CTX[Data<br/>ProductDbContext.cs]
    DB_CTX --> DB[(SQL Server<br/>Product DB)]
```

### 3. Cart Service Flow
```mermaid
graph TD
    Router[HTTP Request] --> C[Controllers<br/>CartController.cs]
    C --> S[Services<br/>CartService.cs]
    S --> R[Repositories<br/>CartRepository.cs]
    R --> DB_CTX[Data<br/>CartDbContext.cs]
    DB_CTX --> DB[(SQL Server<br/>Cart DB)]
```

### 4. Order Service Flow
```mermaid
graph TD
    Router[HTTP Request] --> C[Controllers<br/>OrderController.cs]
    C --> S[Services<br/>OrderService.cs]
    S --> R[Repositories<br/>OrderRepository.cs]
    R --> DB_CTX[Data<br/>OrderDbContext.cs]
    DB_CTX --> DB[(SQL Server<br/>Order DB)]
```

### 5. Wallet Service Flow
```mermaid
graph TD
    Router[HTTP Request] --> C[Controllers<br/>WalletController.cs]
    C --> S[Services<br/>WalletService.cs]
    S --> R[Repositories<br/>WalletRepository.cs]
    R --> DB_CTX[Data<br/>WalletDbContext.cs]
    DB_CTX --> DB[(SQL Server<br/>Wallet DB)]
```

---

## Data Architecture (ER Diagram)

This section provides a logical Entity-Relationship (ER) diagram for the EShoppingZone system.

```mermaid
erDiagram
    %% Entities
    USER_PROFILE {
        string UserId PK
        string FullName
        string Email
        string PasswordHash
        string Role
        string Phone
        boolean IsActive
        datetime CreatedAt
    }

    ADDRESS {
        int AddressId PK
        string UserId FK
        string Street
        string City
        string State
        string ZipCode
        string Country
    }

    PRODUCT {
        int ProductId PK
        string Name
        string Description
        double Price
        int StockQuantity
        string Category
        string ImageUrl
        boolean IsActive
    }

    CART {
        int CartId PK
        string UserId FK
        double TotalAmount
        datetime UpdatedAt
    }

    CART_ITEM {
        int CartItemId PK
        int CartId FK
        int ProductId FK
        int Quantity
        double UnitPrice
    }

    ORDERS {
        int OrderId PK
        string UserId FK
        double TotalAmount
        string Status
        datetime OrderDate
        int ShippingAddressId FK
    }

    PRODUCT_SNAPSHOT {
        int SnapshotId PK
        int OrderId FK
        int ProductId FK
        string ProductName
        int Quantity
        double PriceAtPurchase
    }

    EWALLET {
        int WalletId PK
        string UserId FK
        double Balance
        datetime LastUpdated
    }

    STATEMENT {
        int StatementId PK
        int WalletId FK
        double Amount
        string TransactionType
        string Remarks
        datetime TransactionDate
    }

    %% Logical Relationships
    USER_PROFILE ||--o{ ADDRESS : "has"
    USER_PROFILE ||--o| CART : "owns"
    USER_PROFILE ||--o{ ORDERS : "places"
    USER_PROFILE ||--o| EWALLET : "owns"
    
    CART ||--o{ CART_ITEM : "contains"
    PRODUCT ||--o{ CART_ITEM : "added as"
    
    ORDERS ||--o{ PRODUCT_SNAPSHOT : "includes"
    PRODUCT ||--o{ PRODUCT_SNAPSHOT : "purchased as"
    ADDRESS ||--o{ ORDERS : "shipped to"
    
    EWALLET ||--o{ STATEMENT : "records"
```

---

## Low-Level Design (LLD) - Checkout Workflow

The following sequence diagram illustrates the low-level interactions between the microservices when a customer places an order.

```mermaid
sequenceDiagram
    actor Customer
    participant Gateway as API Gateway
    participant Order as Order Service
    participant Cart as Cart Service
    participant Wallet as Wallet Service
    participant DB as SQL Server

    Customer->>Gateway: POST /api/orders (Checkout)
    Gateway->>Order: Forward Request
    Order->>Cart: GET /api/cart/{userId} (Fetch Cart Items)
    Cart-->>Order: Return Cart Items & Total
    Order->>Wallet: POST /api/wallet/deduct (Deduct Amount)
    Wallet->>DB: Update Wallet Balance
    DB-->>Wallet: Success
    Wallet-->>Order: Payment Successful
    Order->>DB: Save Order & Order Items
    DB-->>Order: Order Created
    Order->>Cart: DELETE /api/cart/{userId} (Clear Cart)
    Cart-->>Order: Cart Cleared
    Order-->>Gateway: 200 OK (Order Confirmed)
    Gateway-->>Customer: Order Details Response
```

---

## UML Class Diagram (Domain Models)

The following UML class diagram represents the core structural relationships and behaviors within the backend domains.

```mermaid
classDiagram
    class UserProfile {
        +String UserId
        +String FullName
        +String Email
        +String Role
        +Register()
        +Login()
        +UpdateProfile()
    }

    class Product {
        +int ProductId
        +String Name
        +double Price
        +int StockQuantity
        +UpdateStock()
        +ApplyDiscount()
    }

    class Cart {
        +int CartId
        +String UserId
        +double TotalAmount
        +AddItem(Product, int)
        +RemoveItem(int)
        +ClearCart()
    }

    class Order {
        +int OrderId
        +String UserId
        +double TotalAmount
        +String Status
        +ProcessPayment()
        +UpdateStatus()
    }

    class Wallet {
        +int WalletId
        +String UserId
        +double Balance
        +AddFunds(double)
        +DeductFunds(double)
    }

    UserProfile "1" --> "1" Cart : owns
    UserProfile "1" --> "*" Order : places
    UserProfile "1" --> "1" Wallet : owns
    Cart "1" o-- "*" Product : contains
    Order "1" o-- "*" Product : includes
```

---

## Services

| Service | Responsibility |
|---|---|
| **API Gateway** | Single entry point, routes all requests via YARP Reverse Proxy |
| **Profile Service** | User authentication, JWT token issuance, profile & address management |
| **Product Service** | Product catalog CRUD, inventory tracking |
| **Cart Service** | Shopping cart operations, adding/removing items |
| **Order Service** | Checkout process, order lifecycle and status tracking |
| **Wallet Service** | Digital wallet management, fund additions, payment statements |

---

## Tech Stack

- **Runtime**: .NET 8 / ASP.NET Core Web API
- **ORM**: Entity Framework Core 8
- **Database**: SQL Server
- **API Gateway**: YARP Reverse Proxy
- **Authentication**: JWT + GitHub OAuth
- **Inter-Service Communication**: IHttpClientFactory
- **Logging**: Serilog
- **Containerization**: Docker
- **Orchestration**: Docker Compose / Kubernetes
- **CI/CD**: GitHub Actions

---

## Testing

Each service has a corresponding test project using xUnit and Moq (e.g., `ProfileService.Tests`, `ProductService.Tests`).

Run all tests from the solution root:

```bash
dotnet test
```

---

## Project Structure

```
EShoppingZone-Backend/
├── backend/
│   ├── ApiGateway/
│   ├── CartService/
│   ├── CartService.Tests/
│   ├── OrderService/
│   ├── OrderService.Tests/
│   ├── ProductService/
│   ├── ProductService.Tests/
│   ├── ProfileService/
│   ├── ProfileService.Tests/
│   ├── WalletService/
│   └── WalletService.Tests/
└── EShoppingZone-Backend.sln
```
