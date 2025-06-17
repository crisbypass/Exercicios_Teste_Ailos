# Título do projeto

Testes da Ailos - Uma possível solução para o desafio proposto pela equipe.

## 🚀 Iniciando

Os projetos usam templates padrão do Visual Studio, incluindo o WebAPI RestFul. Suas configurações e recursos originais foram mantidas, na medidada do
possível, como o Swagger, entre outros. Foi preciso resolver conflitos de versionamento de pacotes do Nuget e outras dependências, além de algumas
inconsistências.

Há um uso um pouco intensivo de recursos específicos como reflection e árvores de expressões, mas como não foram utilizadas algumas ferramentas prontas e
embutidas, ou nativas do Visual Studio(em partes), ou algumas de terceiros, ou de grande praticidade como o EF CORE(Migrations e Afins), ASP.NET Identity entre outros, 
a velocidade de desenvolvimento foi bem reduzida, pois embora haja o lado positivo do uso das ferramentas sugeridas, como o Micro ORM Dapper, existem também outros
contratempos, sem tantas facilidades e recursos intuitivos.

### 📋 Pré-requisitos

Visual Studio 2022, acredito ser o suficiente. Mas foi utilizado também o <a href="https://sqlitestudio.pl/">editor do SqLite Studio</a>, que é
bastante prático, o <a href="https://www.linqpad.net/">LinqPad</a>, além do MS SQL Server(através do Management Studio).

### 🔩 Refinamento

Seria interessante pensar em um esquema de identidade e segurança, além do desempenho. Mas para verificar a idempotência, conforme solicitado nas instruções, basta recuperar uma 
chave(GUID) válida, retornada de uma operação de movimentos e aplicar nos parâmetros dos cabeçalhos de requisição pré-definidos para o endpoint de testes. Caso seja desejável testar
com outro endpoint, basta aplicar o atributo 'Idempotent' para a ação em questão, além de habilitar no Swagger. O código já vêm com um modelo.


### 🔩 Futuras Melhorias

Aplicação de um padrão mais consistente de arquitetura, refinando ainda mais o uso o CRQS(Command and Query Responsibility Segregation), EDA(Event Driven Architecture), entre outros.

### ⌨️ Sobre o código

Em sua maior parte, foi utilizada a linguagem C# e os recursos disponiblizados pela plataforma unificada Microsoft.Net. Algumas mudanças na estrutura original foram efetudas, 
para um funcionamento satisfatório. Sobre o uso de Idempotência, foi montada uma estrutura muito básica, porém customizada, para retenção do cache. Pelo que tenho percebido na
comunidade de desenvolvimento, não parece muito simples estabelecer se de fato existe um consenso quanto às convenções ou formatos padronizados utilizados na arquitetura limpa,
ou sobre o príncípio de responsabilidade única, orientação a eventos, entre outros. Sendo assim, ao menos para esta entrega, não houve subdivisão em Dll's desacopláveis.

##  Importante:

Parte do código contém observações relevantes e outros pontos de atenção, visando melhorias 
e esclarecimento de dúvidas. Agradeço muito a equipe pelo apoio e compreensão, para que fosse possível
um refinamento mais adequado, para a entrega dos testes.

## 🛠️ Construído com

Visual Studio(.Net), Windows Server e a plataforma de gerenciamento virtual Hyper-V.