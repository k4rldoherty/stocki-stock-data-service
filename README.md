# Stocki Stock Data API

## Overview

The Stocki API Bot is designed to query stock information and related news, providing real-time data for various applications. It forms a part of a microservices architecture, making it reusable across multiple projects. Contributors can extend its capabilities by integrating additional features or optimizing its performance.

## Prerequisites

To contribute to or use the Stocki API Bot, you will need API keys from the following services:

- Finnhub
- Alpha Vantage
These keys will allow the bot to access stock data and news.

## Setting Up the Environment

1. Obtain API Keys

- Sign up for the following services to get your API keys:
-- Finnhub: Obtain your API key from Finnhub.
-- Alpha Vantage: Obtain your API key from Alpha Vantage.

2. Set API Keys as Environment Variables

- The API keys need to be set as environment variables for the application to access them.

## For Local Development

Use the following commands to export the API keys as environment variables:

```bash
export FHAPI=<Your_Finnhub_API_Key>
export ALPHAAPI=<Your_AlphaVantage_API_Key>
```

### For Docker

If you are running the application inside a Docker container, you need to pass these environment variables when building and running the container.

Build the Docker container:

``` bash
docker build -t stocki-api .
Run the Docker container with environment variables:
```

``` bash
docker run -e FHAPI=<Your_Finnhub_API_Key> -e ALPHAAPI=<Your_AlphaVantage_API_Key> -p 8080:8080 stocki-api
Replace <Your_Finnhub_API_Key> and <Your_AlphaVantage_API_Key> with the actual keys you obtained.
```

## Development Guide

### Cloning the Repository

To get started, clone the repository:

``` bash
git clone https://github.com/k4rldoherty/stocki-stock-data-service.git
cd stocki-stock-data-service
```
### Dependencies

The project is primarily written in C# and uses a Dockerfile for containerization. Ensure you have the following installed:

- .NET Core SDK
- Docker

## Running the Application Locally

Restore dependencies:
```bash
dotnet restore
Build and run the application:
```
```bash
dotnet run
```

The application will start and be accessible at http://localhost:8080.

## Contribution Guidelines

We welcome contributions to improve the Stocki API Bot. To contribute:

Fork the repository and create a new branch for your feature or bug fix.
Ensure your code adheres to the repository's coding standards.
Write tests for any new functionality.
Submit a pull request with a detailed description of your changes.
Feature Ideas for Contribution

Add support for additional stock APIs.
Implement caching mechanisms for frequently queried data.
Enhance error handling and logging.
Build a frontend interface for easier interaction with the API.
Contact

If you have any questions or need help, feel free to open an issue in the repository.
