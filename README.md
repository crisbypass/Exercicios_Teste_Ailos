# Título do projeto

Testes da Ailos - Uma possível solução para o desafio proposto pela equipe.

## 🚀 Iniciando

Os projetos usam um template padrão do VS, incluindo o WebAPI RestFul. Suas configurações e recursos originais foram mantidas, na medidada do
possível, como o Swagger, entre outros. Mas foi preciso resolver conflitos de versionamento de pacotes do Nuget e outras dependências, além de
algumas inconcistências de dependências e no código.

Há um uso um pouco intensivo de recursos específicos como reflection e árvores de expressões, mas como não foram utilizadas ferramentas
embutidas ou nativas do Visual Studio em partes, algumas de terceiros e de grande praticidade como o EF CORE(Migrations e Afins), ASP.NET Identity e outros, 
a velocidade e praticidade de desenvolvimento foram bem reduzidas, pois há o lado positivo do uso das ferramentas sugeridas como o Micro ORM Dapper, por exemplo,
mas também existem contratempos, como facilidades e recursos não intuitivos.

### 📋 Pré-requisitos

Visual Studio 2022, acredito ser o bastante. Mas foi utilizado também o <a href="https://sqlitestudio.pl/">editor do SqLite Studio</a>, que é
bastante prático, além do MS SQL Server(através do Management Studio).

### 🔩 Refinamento

Fazer um acompanhamento das necessidades de negócio e de crescimento da complexidade da aplicação,
para um melhor planejamento das mudanças.

### 🔩 Futuras Melhorias

Aplicação de um padrão mais consistente de arquitetura, com o uso o CRQS(Command and Query Responsibility Segregation), EDA(Event Driven Architecture), entre outros.

### ⌨️ Sobre o código

Em sua maior parte, usa a linguagem c# e os recursos disponiblizados pela plataforma .Net. Algumas mudanças na estrutura original foram efetudas, por necessidade.
Sobre o uso de Idempotência, foi montada uma estrutura muito básica, porém customizada, para retenção do cache. Os testes foram bastante

##  Importante:

Parte do código contém observações relevantes e outros pontos de atenção, visando melhorias 
e esclarecimento de dúvidas. Agradeço muito a toda equipe pelo apoio e compreensão, para que 
fosse possível um refinamento mais adequado, para a entrega dos testes.

## 🛠️ Construído com

Visual Studio(plataforma unificada .Net)
Docker Desktop(Windows) - Hyper-V
