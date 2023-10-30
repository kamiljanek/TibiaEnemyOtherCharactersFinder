# _Tibia Enemy Other Characters Finder_

<table>
    <tr>
        <td>
            Tibia Enemy Other Characters Finder is a simple ASP.NET Api that helps players to detect other characters of their enemy.
        </td>
    </tr>
</table>

### Web Api
You can check out https://tibia.bieda.it/

---
## Table of Contents
* [General Info](#general-information)
* [Technologies Used](#technologies-used)
* [Features](#features)
* [Screenshots](#screenshots)
* [Setup](#setup)
* [Usage](#usage)
* [Docker Usage](#docker-usage)
* [Project Status](#project-status)
* [Room for Improvement](#room-for-improvement)
* [Contact](#contact)
* [License](#license)

---
## General Information
#### Dear Tibia Players ! 
- Have you ever been killed or cheated by some noob character and you want revenge on his main character ? 

- Now it's possible! Application gives opportunity to find all characters of that player. Just type your suspect character name and it gives you list of most possible other characters.

#### Important!
- You have to remember that application does not have 100% sure of other character match, it is only sugestion based on propability.
The more player plays, the more likely result will be close to true.


---
## Technologies Used
- ASP.NET Core 7.0
- C# 11.0
- EF Core 7.0
- Postgres - version 14.2
- RabbitMq
- TestContainers - documentation [_here_](https://github.com/testcontainers/testcontainers-dotnet)
- BenchmarkDotNet
- Dapper
- Moq
- Serilog
- Seq - documentation [_here_](https://docs.datalust.co/docs) , license [_here_](https://datalust.co/pricing)
- Swagger - documentation [_here_](https://swagger.io/docs/)
- Xunit
- MediatR
- FluentAssertions
- Autofac
- Docker
- Polly

---
## Features
**List the ready features here:**
- All data analyser mechanism with optimization data storage, also work if character changed name or was traded
- GetCharacterController - return character info + other characters
- GetWorldsController - return list of currently scanning worlds


---
## Screenshots

<h4 align="center">Swagger Api view</h>

![](img/main_window.png)

<h4 align="center">Character Controller</h>

![](img/character_endpoint.png) 

<h4 align="center">Example result</h>

![](img/result.png)


---
## Setup

### Project require 
1. SDK version 7.0.x or higher ( instruction for Windows/Linux/macOS [_here_](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) )
2. ASP.NET Core Runtime version 7.0.x or higher ( instruction for Windows/Linux/macOS [_here_](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) )
3. Clone repository `git clone https://github.com/kamiljanek/Tibia-EnemyOtherCharactersFinder.git`
4. Seq enviroment on Windows [_here_](https://docs.datalust.co/docs/getting-started) or docker container [_here_](https://docs.datalust.co/docs/getting-started-with-docker)
5. Your own Postgres Database
6. RabbitMq enviroment or docker container
7. Create database in Postgres and configure `appsettings.json` or if you have Development enviroment copy `appsettings.Development-template.json` change file name to `appsettings.Development.json` and input your secrets
8. Configure `launchSettings.json`

---
## [Usage](https://github.com/kamiljanek/Tibia-EnemyOtherCharactersFinder)

1. Firstly you need to build project - go into repo directory, open CMD and type (`dotnet build`)
2. Than `dotnet publish -c Release -o /app`
3. Next you should firstly run `TibiaEnemyOtherCharactersFinder.Api` to add all migrations - go into `./app`, open CMD and type `dotnet TibiaEnemyOtherCharactersFinder.Api.dll`
4. Last step is to configure `cron` on your machine with periods as below:

- `CharacterAnalyser` - (`dotnet CharacterAnalyser.dll`) - ones per day
- `WorldScanSeeder` - (`dotnet WorldScanSeeder.dll`) - minimum ones per 5 min
- `DbCleaner` - (`dotnet DbCleaner.dll`) - ones per day/week
- `WorldSeeder` - (`dotnet WorldSeeder.dll`) - best practise ones per day
- `ChangeNameDetector` - (`dotnet ChangeNameDetector.dll`) - best practise ones per month

Also 2 projects should run all the time:

- `TibiaEnemyOtherCharactersFinder.Api.dll` - (`dotnet TibiaEnemyOtherCharactersFinder.Api.dll`)
- `RabbitMqSubscriber.dll` - (`dotnet RabbitMqSubscriber.dll`)


### Development
Want to contribute? Great!

To fix a bug, enhance an existing module or add something new, follow these steps:

- Fork the repo
- Create a new branch (`git checkout -b feature/<new_feature_or_improve_name>`)
- Make the appropriate changes in the files
- Add changes to reflect the changes made
- Commit your changes (`git commit -am 'Add xxx feature'`)
- Push to the branch (`git push origin feature/<new_feature_or_improve_name>`)
- Create a Pull Request


---
## [Docker Usage](https://github.com/kamiljanek/Tibia-EnemyOtherCharactersFinder/pkgs/container/tibia-eocf)

1. Firstly pull image
2. Create database in Postgres 
3. Than create and configure file `.env` with enviroment variables as [_here_](https://github.com/kamiljanek/Tibia-EnemyOtherCharactersFinder/blob/develop/.env-template)
4. Than open CMD and run container `docker run --env-file .env -p <port1>:80 --network <seq_container_network> --name tibia_eocf_api -d --restart always ghcr.io/kamiljanek/tibia-eocf:latest dotnet TibiaEnemyOtherCharactersFinder.Api.dll`
5. And `docker run --env-file .env -p <port2>:80 --network <seq_container_network> --name tibia_rabbit_mq_subscriber -d --restart always ghcr.io/kamiljanek/tibia-eocf:latest dotnet RabbitMqSubscriber.dll`
6. Last step is to configure `cron` on your machine with periods as below:
- `docker run --env-file .env -p <port3>:80 --network <seq_container_network> --name tibia_character_analyser --rm -d ghcr.io/kamiljanek/tibia-eocf:latest dotnet CharacterAnalyser.dll`) - ones per day
- `docker run --env-file .env -p <port4>:80 --network <seq_container_network> --name tibia_world_scan_seeder --rm -d ghcr.io/kamiljanek/tibia-eocf:latest dotnet WorldScanSeeder.dll`) - minimum ones per 5 min
- `docker run --env-file .env -p <port5>:80 --network <seq_container_network> --name tibia_db_cleaner --rm -d ghcr.io/kamiljanek/tibia-eocf:latest dotnet DbCleaner.dll`) - ones per day/week
- `docker run --env-file .env -p <port6>:80 --network <seq_container_network> --name tibia_world_seeder --rm -d ghcr.io/kamiljanek/tibia-eocf:latest dotnet WorldSeeder.dll`) - best practise ones per day
- `docker run --env-file .env -p <port7>:80 --network <seq_container_network> --name tibia_change_name_detector --rm -d ghcr.io/kamiljanek/tibia-eocf:latest dotnet ChangeNameDetector.dll`) - best practise ones per month

### Create Docker Image:

- Go into Actions section on Github
- Than `Deploy`
- Than `Run workflow`
- Choose branch and input version of the Docker image

---
## Project Status

Project is: _still in progress_ . 


---
## Room for Improvement

### To do:
- Add autorization and autentication
- Health Checks
- Kubernetes
- Frontend


---
## License

You can check out the full license [_here_](LICENSE.md)

This project is licensed under the terms of the **_MIT_** license.

>Created by [@kamiljanek](https://github.com/kamiljanek) GitHub
