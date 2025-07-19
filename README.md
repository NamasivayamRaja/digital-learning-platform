# Digital Learning Platform

A microservices-based learning platform built with .NET Core.

## Architecture

This project is built using a microservices architecture with the following components:

- **User Service**: User management, authentication, and profiles
- **Content Service**: Course and learning material management
- **Assessment Service**: Quizzes, tests, and grading
- **Leaderboard Service**: Progress tracking and gamification
- **AI Tutor Service**: AI-powered learning assistance
- **Learning Path Service**: Custom learning journeys
- **API Gateway**: Unified entry point using Ocelot

## Technology Stack

- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- Docker & Docker Compose
- RabbitMQ (for messaging)
- Redis (for caching)
- Seq (for logging)

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Docker Desktop
- Visual Studio 2022 or compatible IDE

### Setup Instructions

1. Clone the repository:
- git clone https://github.com/NamasivayamRaja/digital-learning-platform.git 
- cd digital-learning-platform
2. Open the solution in Visual Studio 2022
	1. Build the solution using VS
	2. Build the solution using Terminal:  
		- dotnet build
3. Start the infrastructure:
    - Visual Studio - Select docker-compose as startup project and Press F5 / play button
	- command line docker-compose up -d

### Development Workflow

- Main branch contains stable code
- Development branch is for ongoing work
- Feature branches should be created for new features
- Pull requests require code review before merging

## Project Structure

- `src/` - Source code
- `Services/` - Microservice implementations
- `Gateway/` - API Gateway
- `BuildingBlocks/` - Shared libraries and utilities
- `tests/` - Test projects
- `docker/` - Docker configuration
- `docs/` - Documentation

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.