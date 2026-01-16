# Projeto: Gestão de Empresas e Fornecedores

Este repositório contém uma aplicação em .NET 8 dividida em quatro projetos com responsabilidades separadas: API, Server (serviços e validações), Data (acesso a dados e migrations) e Intf (interfaces, DTOs e modelos). A aplicação gerencia `Empresa`, `Fornecedor` e o relacionamento `EmpresaFornecedor` (associação entre empresas e seus fornecedores).

## Estrutura do workspace

- `Api` — Projeto ASP.NET Core Web API (controladores e `Program.cs`).
- `Server` — Camada de serviços, validações e lógica de negócio (implementações de `I*Service`).
- `Data` — Configuração do Entity Framework Core, `AppDbContext`, repositórios e migrations.
- `Intf` — Modelos, DTOs, interfaces e contratos compartilhados entre projetos.

## Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (EF Core) com Migrations
- SQL Server (provider do EF Core)
- Swagger (OpenAPI) para documentação das APIs
- Injeção de dependência nativa do ASP.NET Core

## Arquitetura e separação de responsabilidades

A solução segue uma arquitetura em camadas simples com separação clara entre API, serviços, repositórios e contratos:

- `Api` — Responsável por expor endpoints REST e mapear requisições/ respostas (controllers). Utiliza controllers base (`CrudBaseController`) para operações CRUD comuns.
- `Server` — Contém a lógica de negócio e validações (ex.: `CpfValidate`, `CnpjValidate`, `CepValidate`). Os services implementam interfaces definidas em `Intf` e usam repositórios do `Data`.
- `Data` — Responsável por persistência com EF Core. Define `AppDbContext` e as entidades mapeadas por Fluent API em `OnModelCreating`. Também contém repositórios concretos para acesso a dados e as migrations geradas.
- `Intf` — Contratos entre camadas: modelos (`EmpresaModel`, `FornecedorModel`, `EmpresaFornecedorModel`), DTOs, interfaces de repositório e serviços.

## Principais design patterns e práticas aplicadas

- Repository Pattern
  - Interfaces de repositório (`IRepository`, `IEmpresaRepository`, `IFornecedorRepository`, `IEmpresaFornecedorRepository`) em `Intf`, implementações em `Data`.
- Service Layer
  - Serviços (`EmpresaService`, `FornecedorService`, `EmpresaFornecedorService`) encapsulam regras de negócio e orquestram chamadas aos repositórios.
- DTOs e modelos
  - `Intf` separa modelos persistidos (`*Model`) de DTOs (`*Dto`) usados na API quando necessário.
- Dependency Injection
  - Todos os repositórios e serviços são registrados no container do ASP.NET Core em `Program.cs`.
- Fluent API do EF Core
  - Mapeamentos e restrições (tamanhos de campo, obrigatoriedade, índices) configurados em `AppDbContext.OnModelCreating`.
- Migrations (EF Core)
  - As migrations ficam no projeto `Data/Migrations`.

## Modelagem e peculiaridades

- Entidades principais:
  - `EmpresaModel`: contém `Cnpj`, `Name`, `Cep` e coleção de `EmpresaFornecedor`.
  - `FornecedorModel`: pode ser `CPF` ou `CNPJ` (campo `Type` via `FornecedorTypeEnum`), contém `Name`, `Email`, `Cep`, possíveis `Cpf`/`Cnpj`/`Rg` e coleção de `EmpresaFornecedor`.
  - `EmpresaFornecedorModel`: entidade de junção explicitando o relacionamento muitos-para-muitos entre empresas e fornecedores. Possui índice único na combinação `EmpresaId` + `FornecedorId`.
- Campos de auditoria: `Id`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` estão presentes nos modelos e mapeados como opcionais quando apropriado.
- Regras de exclusão: relacionamentos configurados com `OnDelete(DeleteBehavior.Cascade)` para propagar remoções do lado principal.
- Validações específicas: existem validações customizadas em `Server/Validation` para `Cpf`, `Cnpj` e `Cep`.

## Configuração e execução

1. Ajuste a connection string no `appsettings.json` do projeto `Api` (ou use User Secrets / variáveis de ambiente). Exemplo (SQL Server):

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=MinhaBase;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;"
   }
   ```

2. Restaurar pacotes e compilar:

   - `dotnet restore`
   - `dotnet build`

3. Aplicar migrations e atualizar o banco de dados (a partir do diretório da solução ou do projeto `Data`):

   - `dotnet ef migrations add NomeDaMigration --project Data --startup-project Api --output-dir Data/Migrations`
   - `dotnet ef database update --project Data --startup-project Api`

4. Executar a API:

   - `dotnet run --project Api`

5. Durante o desenvolvimento, o Swagger estará disponível quando o ambiente for `Development` (configurado em `Program.cs`).

## CORS

- O projeto habilita CORS com política permissiva por padrão para desenvolvimento (todas as origens, cabeçalhos e métodos). Em produção, é recomendado restringir as origens permitidas.

## Padrões de implementação e pontos de atenção

- Controllers base: existe uma base genérica `CrudBaseController` usada para operações CRUD comuns — facilita reaproveitamento de código.
- Unicidade de par Empresa-Fornecedor: a tabela de junção possui índice único para evitar duplicidade.
- Campos opcionais: diversas propriedades de auditoria e campos de documento (`Cpf`, `Cnpj`, `Rg`) são opcionais — atentar para validações antes de persistir.
- Migrations já estão presentes em `Data/Migrations` — rever antes de recriar novas migrations para evitar conflitos.
- Configurações de produção: `UseHttpsRedirection` é aplicado somente fora de `Development` para evitar problemas de CORS em desenvolvimento.

## Testes e validação

- Não há projeto de testes incluso no workspace atual. Recomenda-se criar testes unitários para:
  - Validações de `Cpf`, `Cnpj` e `Cep`.
  - Serviços (`Server/*Service`) garantindo regras de negócio.
  - Repositórios com um banco em memória (`InMemoryDatabase`) do EF Core.

## Como contribuir / próximas melhorias

- Implementar autenticação e autorização (ex.: JWT) para proteger endpoints.
- Adicionar logs estruturados (ex.: `Serilog`).
- Implementar DTOs mais completos e AutoMapper para mapeamento entre `Model` e `Dto`.
- Criar testes automatizados (unit/integration).
- Restringir CORS em produção e adicionar políticas de rate limiting se necessário.

## Contato

Para dúvidas sobre o código ou arquitetura, consulte os arquivos de cada projeto:

- `Api/Program.cs` — configuração da aplicação e DI
- `Data/AppDbContext.cs` — mapeamento das entidades
- `Server/*Service.cs` — regras de negócio e validações
- `Intf/*` — contratos, modelos e DTOs

