# Praxis API — Best Stories Service

This project exposes an API to retrieve the best stories from Hacker News, applying caching, deduplication, and resilience patterns.

---

# Overview

The API fetches the best stories from the Hacker News API, processes them, and returns the top `N` stories ordered by score.

It is designed with:

* Vertical Slice Architecture
* Clean separation of concerns
* Resilience (retry, circuit breaker)
* Redis for server-side caching
* ETag for client-side caching
* Request deduplication (in-flight cache)
* OpenAPI + Scalar UI

---

# Tech Stack

* .NET 10 (Minimal API)
* Redis (distributed cache)
* Docker + Docker Compose
* OpenAPI + Scalar UI
* HttpClientFactory + Resilience pipeline

---

# How to Run

## Run with Docker

### 1. Prerequisites

* Docker installed

---

### 2. Run the application

```bash
docker-compose up --build
```

---

### 3. Access the services

| Service         | URL                          |
| --------------- | ---------------------------- |
| API             | http://localhost:5000        |
| Scalar (API UI) | http://localhost:5000/scalar |
| Redis UI        | http://localhost:8081        |

---

# API Endpoint

## GET /stories/{n}

Returns the top `n` best stories.

### Example:

```bash
GET /stories/10
```

---

# Key Design Decisions

## 1. Deduplication (In-Flight Cache)

Prevents multiple simultaneous requests from triggering duplicate external API calls.

---

## 2. Redis Caching

* Reduces repeated calls to Hacker News
* TTL-based caching (5 minutes)

---

## 3. Resilience (Built-in .NET)

Using:

```csharp
.AddStandardResilienceHandler();
```

Provides:

* Retry
* Circuit breaker
* Timeout
* Jitter

---

## 4. Fetch Limiting (`Take(100)`)

Limits number of external calls to:

* Prevent overload
* Improve performance
* Ensure stable response times

---

## 5. Vertical Slice Architecture

Each feature contains:

* Endpoint
* Service
* Models

Avoids unnecessary layering complexity.

---

# Assumptions

* Hacker News API is publicly available and stable
* Redis is available (or gracefully handled if not)
* Data consistency is eventual (due to caching)
* Only top stories are required (not real-time updates)

---

# Improvements if the project grows (Given More Time)

## 1. Observability

* Add OpenTelemetry (tracing + metrics)
* Structured logging (Serilog)

---

## 2. Rate Limiting Strategy

* Per-user limits
* Adaptive throttling

---

## 3. Configuration Improvements

* Move constants (like 100) to config
* Environment-based tuning

---

## 4. Health Checks

* Redis health check
* External API health check

---

## 5. Testing

* Add unit tests for validating caching and deduplication logic and others

---

## 6. Security

* Add authentication (JWT)
* Add API key support

---
