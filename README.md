# ShowTime
Is an application for scraping the [Tv Maze Api](https://www.tvmaze.com/api)

## Functionality
 - Retrieves all shows from the [Show Api](https://www.tvmaze.com/api#show-search)
 - When [rate limiting](https://www.tvmaze.com/api#rate-limiting) is hit, a retry will be done

# How to run

## Prerequisites
SQL Server default instance installed

## Commands
Navigate to Showtimed.Api folder
Run: dotnet tool restore
Run: dotnet ef database update

