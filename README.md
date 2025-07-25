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

- ASP.NET Core 9.0, 8.0  
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
   ```
   git clone https://github.com/NamasivayamRaja/digital-learning-platform.git  
   cd digital-learning-platform  
   ```

2. Open the solution in Visual Studio 2022  
   - Build the solution using Visual Studio  
   - Or build from terminal:  
     ```
     dotnet build  
     ```

3. Start the infrastructure  
   - Open the project folder in command line:  
     ```
     docker-compose up -d  
     docker-compose up --build -d
     ```
   - Open the Container window from Visual Studio:  
     View > Other Windows > Containers (CTRL + K, CTRL + O)  
   - Select the running container and attach to the process  
     - If prompted, choose: "Disable Just My Code and Continue"  

## Local Development Certificate Setup

This project uses mkcert to generate locally trusted HTTPS certificates for secure microservice communication during development.

### Dev Certificate Structure

Certificates are stored in:

```
[PATH]\DigitalLearningPlatform\DevCertificates\digital-learning-userservice
```

Contents:
- cert.pem � Public certificate file  
- key.pem � Private key file  
- rootCA.pem � mkcert root certificate copied from system trust store  

### How to Generate and Configure

1. Install mkcert and setup system CA:

   ```powershell
   mkcert -install
   ```

2. Generate certificate for your service:

   ```powershell
   mkdir DevCertificates\digital-learning-userservice

   mkcert -cert-file DevCertificates\digital-learning-userservice\cert.pem `
          -key-file DevCertificates\digital-learning-userservice\key.pem `
          digital-learning-userservice localhost
   ```

3. Copy root CA:

   ```powershell
   Copy-Item "$env:USERPROFILE\.local\share\mkcert\rootCA.pem" `
             "D:\Learn\Microservices\DigitalLearningPlatform\DevCertificates\digital-learning-userservice\rootCA.pem"
   ```

4. Docker integration (Dockerfile):

   ```dockerfile
   COPY DevCertificates/digital-learning-userservice/rootCA.pem /usr/local/share/ca-certificates/rootCA.crt
   RUN update-ca-certificates
   ```

5. Container TLS validation:

   ```powershell
   docker run --rm -it `
     --network learning-network `
     -v "D:\Learn\Microservices\DigitalLearningPlatform\DevCertificates\digital-learning-userservice\rootCA.pem:/ca.pem" `
     curlimages/curl curl --cacert /ca.pem -v https://digital-learning-userservice:8081
   ```

6. Git ignore:

   ```
   DevCertificates/
   ```

## Development Workflow

- main branch contains stable code  
- development branch is for ongoing work  
- Feature branches should be created for new features  
- Pull requests require code review before merging  

## Project Structure

- src/ � Source code  
- Services/ � Microservice implementations  
- Gateway/ � API Gateway  
- BuildingBlocks/ � Shared libraries and utilities  
- tests/ � Test projects  
- docker/ � Docker configuration  
- docs/ � Documentation  

## License

This project is licensed under the [MIT License](LICENSE)