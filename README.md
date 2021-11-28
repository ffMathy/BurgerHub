# BurgerHub
![Build status](https://github.com/ffMathy/BurgerHub/actions/workflows/dotnet.yml/badge.svg)
BurgerHub is a sample project intended to demonstrate various skills in various areas.

## Prerequisites
- Docker installed.
- DOTNET SDK 6.0.

## Starting dependencies
Run `scripts/start-dependencies.bat` to start dependencies. For now, the only dependency is Mongo.

## Seeding with random data
Start the `BurgerHub.Api.Seeder` Console Application project.

## Testing
The implementation can be tested via PostMan. Inside the `postman` directory, I have exported a collection with all the APIs. Note that the hardcoded values in the requests (such as various IDs) etc, might not match the IDs of your seeded data.
