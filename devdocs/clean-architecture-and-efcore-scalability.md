# Clean Architecture & EF Core Scalability Notes (MiLyst)

## Context
This document captures key questions, discussion points, and recommendations from a design conversation focused on:
- Clean Architecture for .NET
- EF Core performance/scalability considerations
- A growth path from single-install deployments to a multi-tenant SaaS model on PostgreSQL

---

## Key Questions

### 1) What is Clean Architecture for .NET?

#### Summary
Clean Architecture structures a solution so that business rules are independent of frameworks and infrastructure.

#### Core principle: Dependency Rule
Dependencies only point inward:
- Outer layers can depend on inner layers.
- Inner layers do not depend on outer layers.

#### Typical layers/projects in a .NET solution
- **Domain**
  - Entities, value objects, domain services (pure business rules)
  - No dependencies on ASP.NET Core, EF Core, etc.
- **Application**
  - Use cases (commands/queries), orchestration, validation
  - Defines interfaces needed for infrastructure (e.g., repositories, email, clock)
  - Depends on Domain
- **Infrastructure**
  - EF Core DbContext, migrations, repository implementations
  - External integrations (email provider, file storage, external APIs)
  - Depends on Application (implements interfaces) and Domain
- **Presentation (API/UI)**
  - ASP.NET Core endpoints/controllers, auth, middleware
  - Maps HTTP requests to Application use cases
  - Depends on Application

#### Practical outcome
- Business logic remains testable without the database or web server.
- Infrastructure (Postgres, EF Core, external services) can evolve without rewriting core use cases.

---

### 2) Scalability and performance considerations of EF Core

#### Executive summary
EF Core can scale well for large, high-traffic apps when the system is designed with:
- Correct indexing
- Bounded/paginated queries
- Projection-based reads
- Clear tenant boundaries

For most systems, the first bottlenecks are usually:
- Database schema/indexes
- Query shapes (pagination, filtering, avoiding over-fetching)
- Contention/locking patterns

#### Common EF Core pitfalls at scale
- **N+1 queries** (often caused by lazy loading or navigation access patterns)
- **Over-fetching** entire object graphs for list screens
- **Tracking overhead** on read-heavy endpoints
- **Unbounded queries** (missing pagination)
- **Suboptimal LINQ translation** for certain patterns

#### EF Core practices that keep performance high
- Use **`AsNoTracking()`** for read-only queries
- Prefer **projections** (`Select(...)`) to DTOs for list screens
- Enforce **pagination** for any list endpoint
- Avoid **lazy loading** (explicit includes or projections)
- Consider **compiled queries** for the hottest repeated queries once profiling data exists
- Instrument and inspect SQL (logs/tracing) and tune based on real metrics

---

## Workload notes discussed

### Business use case
- Thousands of products
- A product can initiate tasks that move through ~5–10 stages/lists (e.g., todo, cutting, pending, painting, packaging, website, delivery, done)
- ~30–40 tasks per month

### GiveCamp use case
- ~20 projects
- ~100+ tasks per project running simultaneously over one weekend annually
- Keep multiple years of data

### Growth outlook
- Potentially hundreds of similar use cases in year 1
- Initially deployed on separate servers (reduces near-term scaling pressure)
- Long-term goal: avoid architectural dead ends and preserve a SaaS path

---

## Recommendations for PostgreSQL + multi-tenant SaaS readiness

### Start with a shared database / shared schema multi-tenant model
You indicated you are fine with starting with a multi-tenant single DB model, and delaying “hard isolation” until later.

Key recommendation: make **tenant boundaries explicit and hard to violate**.

#### Tenant modeling
- Add a **`TenantId`** to every tenant-owned table (Tasks, Projects, Products, etc.)
- Make `TenantId` part of key composite indexes (see below)

#### Tenant enforcement options
- **EF Core global query filters** to automatically apply `TenantId = currentTenant`
- Strongly consider **PostgreSQL Row Level Security (RLS)** as a defense-in-depth mechanism to prevent accidental cross-tenant access

> Note: RLS and connection pooling require careful handling of per-request/session settings.

### Keep the task workflow model index-friendly
The “tasks move through lists” requirement maps well to a simple state model:
- Store “list/stage/status” as a column (enum/int or small lookup FK)
- Avoid join-heavy representations early unless you truly need highly dynamic/custom boards

### Indexing guidance for common task list queries
Your likely hotspot is “tasks for tenant + project/product + stage, sorted, paginated”.

Examples of helpful composite indexes (shape them to your actual filters/sorts):
- `(TenantId, ProjectId, Stage, SortKey)`
- `(TenantId, Stage, UpdatedAt)`
- `(TenantId, AssigneeId, Stage, DueDate)` (if assignment is common)

### Sharding path (later)
You can shard without a rewrite by introducing a **Tenant -> Shard mapping**:
- Resolve tenant
- Resolve shard connection string
- Create DbContext using that connection

This supports “scale far on single DB, shard when needed, move to multi-db if required” with minimal disruption.

### ID strategy question (to decide early)
Choose identifiers that won’t fight sharding later:
- **UUIDs**: simplest for sharding and global uniqueness (larger indexes)
- **bigint**: smaller/faster indexes, but sharding may require composite keys or allocation strategies

---

## Practical next steps (if/when implementing)
- Decide and apply a consistent `TenantId` strategy across the schema.
- Establish conventions for:
  - always paginating list endpoints
  - using `AsNoTracking()` + DTO projections for reads
  - logging/tracing slow queries
- Add a small set of “known hot queries” and ensure indexes match the WHERE + ORDER BY.

