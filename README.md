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

---

## Projeto Frontend

O frontend é uma aplicação **Angular 18** que consome a API REST desenvolvida em .NET. A interface permite o gerenciamento completo de empresas e fornecedores, incluindo o cadastro, listagem, edição e exclusão de registros, além do gerenciamento de relacionamentos entre empresas e fornecedores.

### Tecnologias e ferramentas

- **Angular 18**: Framework principal para desenvolvimento da SPA (Single Page Application)
- **TypeScript 5.4**: Linguagem de programação
- **RxJS 7.8**: Biblioteca para programação reativa e gerenciamento de observables
- **Tailwind CSS 3.4**: Framework CSS utility-first para estilização
- **Angular Router**: Gerenciamento de rotas e navegação
- **Angular Forms**: Formulários reativos e validações

### Estrutura do projeto Frontend

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/           # Módulos e serviços principais da aplicação
│   │   ├── features/        # Módulos de funcionalidades
│   │   │   ├── empresas/   # Componentes de gerenciamento de empresas
│   │   │   └── fornecedores/ # Componentes de gerenciamento de fornecedores
│   │   ├── models/          # Interfaces e modelos TypeScript
│   │   ├── services/        # Serviços de comunicação com a API
│   │   │   ├── empresa.service.ts
│   │   │   ├── fornecedor.service.ts
│   │   │   ├── empresa-fornecedor.service.ts
│   │   │   └── cep-validator.service.ts
│   │   ├── shared/          # Componentes e recursos compartilhados
│   │   ├── app.component.* # Componente raiz da aplicação
│   │   └── app.module.ts   # Módulo principal
│   ├── styles.css          # Estilos globais (configuração Tailwind)
│   ├── index.html          # Página HTML principal
│   └── main.ts             # Ponto de entrada da aplicação
├── angular.json            # Configuração do Angular CLI
├── package.json            # Dependências e scripts npm
├── tailwind.config.js      # Configuração do Tailwind CSS
└── tsconfig.json           # Configuração do TypeScript
```

### Funcionalidades principais

1. **Gerenciamento de Empresas**
   - Cadastro de novas empresas com validação de CNPJ
   - Listagem com paginação
   - Edição de dados cadastrais
   - Exclusão de empresas
   - Associação e desassociação de fornecedores

2. **Gerenciamento de Fornecedores**
   - Cadastro de fornecedores pessoa física (CPF) ou jurídica (CNPJ)
   - Validação de CPF/CNPJ e RG
   - Listagem com paginação e filtros
   - Edição de dados cadastrais
   - Exclusão de fornecedores
   - Visualização de empresas associadas

3. **Validações**
   - Validação de CEP com integração à API externa
   - Validação de CPF e CNPJ no frontend
   - Validação de campos obrigatórios
   - Validação de formatos de e-mail

### Serviços e comunicação com API

Os serviços Angular (`*Service`) encapsulam toda a comunicação HTTP com a API backend:

- `EmpresaService`: operações CRUD de empresas
- `FornecedorService`: operações CRUD de fornecedores
- `EmpresaFornecedorService`: gerenciamento de relacionamentos
- `CepValidatorService`: validação e busca de endereços por CEP

Todos os serviços utilizam `HttpClient` do Angular e retornam `Observables` para trabalhar com programação reativa.

### Configuração e execução do Frontend

1. **Instalar dependências:**
   ```bash
   cd frontend
   npm install
   ```

2. **Executar em modo de desenvolvimento:**
   ```bash
   npm start
   ```
   A aplicação estará disponível em `http://localhost:4200`

3. **Build para produção:**
   ```bash
   npm run build
   ```
   Os arquivos compilados estarão em `dist/frontend`

4. **Build com watch (desenvolvimento):**
   ```bash
   npm run watch
   ```

### Configuração da API

O frontend está configurado para se comunicar com a API backend. Certifique-se de que:
- A API está executando (geralmente em `https://localhost:5001` ou `http://localhost:5000`)
- As configurações de CORS na API permitem requisições do frontend
- A URL base da API está corretamente configurada nos serviços Angular

### Estilização com Tailwind CSS

O projeto utiliza Tailwind CSS para estilização, oferecendo:
- Design responsivo e moderno
- Utility classes para desenvolvimento rápido
- Customização através do arquivo `tailwind.config.js`
- Consistência visual em todos os componentes

### Boas práticas implementadas

- **Separação de responsabilidades**: componentes, serviços e modelos bem definidos
- **Lazy Loading**: carregamento sob demanda de módulos de funcionalidades
- **Reactive Forms**: formulários reativos com validações robustas
- **Observables**: uso de RxJS para gerenciamento assíncrono
- **TypeScript**: tipagem forte para maior segurança e manutenibilidade
- **Componentização**: componentes reutilizáveis na pasta `shared`