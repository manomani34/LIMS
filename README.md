# LIMS

Laboratory Information Management System

## Architecture

- Modular Monolith
- Clean Architecture
- Domain-Driven Design
- CQRS-oriented Application layer
- API-first; no standalone UI
- Shared database/schema
- Laboratory is a domain Aggregate Root, not a Tenant

## Projects

- LIMS.Api
- LIMS.Application
- LIMS.Domain
- LIMS.Infrastructure
- LIMS.Contracts

## Initial domain modules

Requests, Samples, Experiments, Protocols, Instruments, Results,
Workflows, Approvals, Files, Laboratories.

## Development status

Initial solution scaffolding.
Persistence provider, authentication provider and external storage
will be selected and configured in subsequent architecture decisions.
