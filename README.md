# InventoryApp

---

## 📌 Design Decisions

- **Layered Architecture**  
  The solution is structured into clearly separated layers for better organization and scalability:
  - **Controllers**: Thin controllers focus purely on HTTP handling and role-based access, delegating all logic to services.
  - **Services**: All business logic is encapsulated in service classes, following the "thick service, thin controller" principle.
  - **Interfaces**: Each service implements a corresponding interface (e.g., `IProductsService`, `ISuppliersService`) to support:
    - Loose coupling
    - Easy unit testing & mocking
    - Plug-and-play flexibility (e.g., swap cache/db strategies)
  - **DTOs (Data Transfer Objects)**: Explicitly define expected input/output shapes, avoiding over-posting and coupling to domain models.
  - **Data Layer**: Uses **Entity Framework Core** with a **Code-First** approach for flexible schema management and clean database interaction.

- **Dependency Injection (DI)**  
  ASP.NET Core's built-in DI container is leveraged to inject services, DbContext, caching, and audit logging components. This ensures:
  - Decoupled components
  - Greater modularity
  - Improved testability and flexibility

- **Caching**  
  Frequently accessed data like supplier and product lists are cached using `IMemoryCache`, reducing unnecessary database reads and improving response times.

- **Audit Logging**  
  Every write operation (Add, Update, Delete) triggers an audit log entry with metadata — capturing who made what change and when, supporting traceability and compliance.

- **Soft Deletion**  
  Entities are "deleted" via an `IsDeleted` flag instead of physical removal, allowing:
  - Historical traceability
  - Data recovery
  - Better integrity in audit logs and foreign key references

- **Inventory via StockMovement Table**  
  Stock quantity is **not stored directly on the Product**. Instead, all inventory levels are computed from the **StockMovement** table, which records every stock-in, stock-out, and transfer event. This design:
  - Provides an **immutable audit trail** for all inventory changes
  - Ensures **accurate, real-time stock levels** through aggregation
  - Supports advanced features like stock reconciliation, reporting, and traceability
  - Decouples product metadata from inventory flow for better **data integrity** and **business resilience**

- **Role-Based Authorization**  
  Secured using `[Authorize(Roles = "...")]`:
  - **CentralAdmin**: Full CRUD access
  - **StoreAdmin**: Scoped access — can perform read and write operations limited to their **own store's** data.  

- **Rate Limiting**  
  Rate-limiting middleware is applied with separate policies:
  - `ReadPolicy`: Allows frequent GETs
  - `WritePolicy`: Tighter control over POST/PUT/DELETE  

- **Scalability & Best Practices**  
  - Business logic is fully decoupled from the web framework.
  - Interfaces provide abstraction and enable easy replacement, testing, and scaling.
  - Thin controllers improve maintainability.
  - DTOs support backward-compatible evolution.
  - EF Core code-first keeps the database model in sync with the domain layer.

## ✅ Assumptions

- Each **Product** must be associated with a valid **Supplier** (`SupplierId` is required when adding a product).
- Only **CentralAdmin** users are allowed to perform global write operations (Add, Update, Delete across all stores).
- **StoreAdmins** can perform operations within the scope of their **own stores**, but do not have access to global or cross-store data.
- Deleted entities are retained in the database via **soft deletion** (`IsDeleted = true`) to maintain data integrity and audit history.
- All API consumers are expected to authenticate using a **JWT-based** token system with embedded role claims.
- **Audit logging** is mandatory for all state-changing operations to support traceability.

---

## 🖥️ API Design

- **Swagger UI**  
  The API includes **Swagger UI** for auto-generated, interactive documentation. It simplifies exploration and testing of endpoints, improving the developer experience and reducing onboarding time.
  ![Swagger UI Screenshot](./docs/api_design_1.PNG)
