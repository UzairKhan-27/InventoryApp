# InventoryApp

---

## 📌 Design Decisions

- **Layered Architecture**  
  The solution follows a clear layered structure for scalability and maintainability:
  - **Controllers**: Thin, focused on HTTP handling and role-based access, delegating logic to services.
  - **Services**: Encapsulate business logic following the "thick service, thin controller" principle.
  - **Interfaces**: Ensure loose coupling, testability, and flexibility (e.g., swapping caching strategies).
  - **DTOs**: Explicitly define input/output shapes to prevent over-posting and maintain separation from domain models.
  - **Data Layer**: Uses **Entity Framework Core** with a **Code-First** approach for schema management and clean database interactions.

- **Dependency Injection (DI)**  
  ASP.NET Core's DI container enables decoupling of services, DbContext, caching, and logging components, ensuring modularity, testability, and flexibility.

- **Caching**  
  **IMemoryCache** is used to cache frequently accessed data (e.g., product and supplier lists), reducing database load and enhancing response times.

- **Audit Logging**  
  All state-changing operations (Add, Update, Delete) trigger audit log entries, supporting traceability and compliance with metadata.

- **Soft Deletion**  
  Entities are soft-deleted with an `IsDeleted` flag, preserving data integrity, historical traceability, and foreign key references.

- **Inventory via StockMovement Table**  
  Stock quantity is managed through the **StockMovement** table, ensuring:
  - Immutable audit trail for inventory changes
  - Real-time stock level accuracy
  - Decoupling product data from inventory flow, enhancing data integrity and business resilience.

- **Role-Based Authorization**  
  Role-based access control using `[Authorize(Roles = "...")]`:
  - **CentralAdmin**: Full CRUD access
  - **StoreAdmin**: Scoped access, limited to their own store’s data.

- **Rate Limiting**  
  Separate rate-limiting policies for:
  - `ReadPolicy`: Frequent GETs
  - `WritePolicy`: Control over POST/PUT/DELETE actions.

- **Scalability & Best Practices**  
  - Decoupled business logic.
  - Interfaces for easy testing, replacement, and scalability.
  - Thin controllers, ensuring maintainability.
  - EF Core code-first approach for database consistency with domain models.


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
  ![Swagger UI Screenshot](./docs/api_design_2.PNG)
  ![Swagger UI Screenshot](./docs/api_design_3.PNG)

---
