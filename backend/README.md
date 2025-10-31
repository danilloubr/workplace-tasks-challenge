# Desafio Técnico - Workplace Tasks (Backend)

Esta é a API de backend para a aplicação "Workplace Tasks", desenvolvida como parte do desafio técnico.

O backend é construído em **.NET 8** seguindo os princípios da **Clean Architecture** para garantir uma clara separação de responsabilidades (SOLID), testabilidade e manutenção.

## Tecnologias Utilizadas

* **.NET 8** (C#)
* **Entity Framework Core 8** (ORM)
* **PostgreSQL** (Banco de Dados)
* **Autenticação JWT** (Para segurança da API)

## Arquitetura

O projeto está dividido em quatro camadas principais (Domain, Application, Infrastructure, Api) para isolar as responsabilidades:

1. **`Domain`**: Contém as entidades de negócio puras (ex: `User`, `Task`) e não depende de nenhuma outra camada.
2. **`Application`**: Contém a lógica de negócios (Services) e os contratos (Interfaces, DTOs). É o "cérebro" da aplicação.
3. **`Infrastructure`**: Contém as implementações técnicas, como o acesso ao banco de dados (Repositórios, `AppDbContext`) e o `Data Seeding`.
4. **`Api`**: Camada de "entrada" que expõe os endpoints HTTP (Controladores) e lida com o tratamento de erros (Middleware).

**Por que esta arquitetura?**
Esta abordagem (Padrão de Repositório + Camada de Serviço) foi escolhida para desacoplar totalmente a lógica de negócios dos detalhes de implementação. Os Controladores não sabem nada sobre o Entity Framework; eles apenas conversam com os "Serviços" (`ITaskService`, `IUserService`), o que torna o código limpo, fácil de manter e de testar.

## Pré-requisitos (Local)

Para executar este projeto localmente, você precisará de:

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
2. Um servidor **PostgreSQL** local.
    * **macOS:** A recomendação é o [Postgres.app](https://postgresapp.com/).
    * **Windows:** A recomendação é o [instalador oficial da EDB](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads) ou o [Docker Desktop](https://www.docker.com/products/docker-desktop/).

## Passos para Executar Localmente

1. **Clonar e Navegar:**
    Clone o repositório e navegue até a pasta do backend:

    ```bash
    # (Exemplo)
    git clone [https://github.com/seu-usuario/workplace-tasks-challenge.git](https://github.com/seu-usuario/workplace-tasks-challenge.git)
    cd workplace-tasks-challenge/backend
    ```

2. **Configurar o Banco de Dados (Depende do seu SO):**

    * **Se estiver no macOS (com Postgres.app):**
        * Abra o Postgres.app.
        * Clique em "Initialize" (se for a primeira vez).
        * Na barra lateral, clique no `+` para criar um novo banco de dados.
        * Use o nome `workplace_tasks_dev`.
        * (O seu usuário padrão será o seu nome de usuário do Mac, ex: `danillou`).

    * **Se estiver no Windows (com Instalador EDB):**
        * Durante a instalação do PostgreSQL, você definirá uma senha de superusuário (para o usuário `postgres`). **Anote esta senha.**
        * Após a instalação, use `pgAdmin` (que vem com o instalador) para criar um novo banco de dados chamado `workplace_tasks_dev`.

3. **Configurar os "Segredos" da Aplicação:**
    Usamos o `user-secrets` para armazenar dados sensíveis. Navegue até a pasta da API:

    ```bash
    # Na pasta 'backend', execute:
    cd src/WorkplaceTasks.Api
    ```

    * **Se estiver no macOS (com Postgres.app):**
        (Substitua `SEU_USUARIO_MAC` pelo seu nome de usuário)

        ```bash
        dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=workplace_tasks_dev;Username=SEU_USUARIO_MAC"
        ```

    * **Se estiver no Windows (com Instalador EDB):**
        (Substitua `SUA_SENHA_POSTGRES` pela senha que você anotou)

        ```bash
        dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=workplace_tasks_dev;Username=postgres;Password=SUA_SENHA_POSTGRES"
        ```

    * **Execute para todos os sistemas (JWT):**
        (Substitua a `Jwt:Key` por sua própria frase secreta longa)

        ```bash
        dotnet user-secrets set "Jwt:Key" "MINHA_CHAVE_SECRETA_MUITO_LONGA_E_SEGURA_COM_MAIS_DE_32_CARACTERES"
        dotnet user-secrets set "Jwt:Issuer" "http://localhost:5110"
        dotnet user-secrets set "Jwt:Audience" "http://localhost:4200"
        ```

4. **Restaurar Pacotes e Aplicar Migrações:**
    *Volte para a pasta `backend` (a raiz do .sln):*

    ```bash
    cd ../.. 
    ```

    *Instale as dependências:*

    ```bash
    dotnet restore
    ```

    *Aplique as migrações ao banco (isso criará as tabelas e "semeará" os usuários de teste):*

    ```bash
    dotnet ef database update --startup-project src/WorkplaceTasks.Api
    ```

5. **Rodar a API:**
    *Navegue de volta para a pasta da API:*

    ```bash
    cd src/WorkplaceTasks.Api
    ```

    *Execute a aplicação:*

    ```bash
    dotnet run
    ```

A API estará rodando (geralmente em `http://localhost:5110` ou `https://localhost:7123`).

## Testando a API (Swagger e RBAC)

### 1. Acessar o Swagger

Abra seu navegador e vá para **`http://localhost:5110/swagger`** (use a porta informada no seu terminal).

### 2. Contas de Teste (RBAC)

O banco de dados é "semeado" (Data Seeding) com 3 contas de teste, conforme solicitado.

| Role (Papel) | Email | Senha |
| :--- | :--- | :--- |
| **Admin** | `admin@example.com` | `admin1203` |
| **Manager**| `manager@example.com`| `manager123` |
| **Member** | `member@example.com` | `member123` |

### 3. Como Testar

1. No Swagger, vá até o endpoint `POST /api/auth/login`.
2. Use uma das contas de teste (ex: `admin@example.com`) para logar.
3. Copie o `token` JWT da resposta (copie *apenas* o token em si, sem aspas).
4. No topo da página do Swagger, clique no botão **"Authorize"**.
5. **Na caixa de texto "Value:", cole o token que você copiou.**
6. Clique em "Authorize" e depois em "Close".
7. Agora você está autenticado e pode testar os endpoints protegidos (`/api/tasks`, `/api/users`).
8. Faça "Logout" (clicando em "Authorize" > "Logout") e repita o processo com as contas `manager` e `member` para validar as regras de permissão (RBAC).
