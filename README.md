# PedidosApi

## Sobre

A aplicação `PedidosApi` é um serviço para gerenciar pedidos, utilizando .NET, MongoDB e RabbitMQ. Esta aplicação fornece uma API REST para consultar informações sobre pedidos.

## Rodando os Testes

Para rodar os testes automatizados da aplicação, utilize o comando:

```bash
dotnet test
```

Isso executará todos os testes definidos no projeto de testes.

## Rodando a Aplicação

Para iniciar a aplicação, utilize o Docker Compose. Execute o comando:

```bash
docker-compose up -d
```

## Acessar o Swagger

Você pode acessar a interface Swagger da aplicação para explorar e testar os endpoints da API em:

http://localhost:5000/swagger/index.html

## Criar um Novo Pedido no RabbitMQ

Para criar um novo pedido e enviá-lo para uma fila no RabbitMQ, siga as instruções abaixo:

### Acessar a Interface Web do RabbitMQ

1. Abra o navegador e vá para a interface web do RabbitMQ em [http://localhost:15672](http://localhost:15672).

2. Faça login com as credenciais padrão:
   - **Username:** `guest`
   - **Password:** `guest`

### Publicar uma Mensagem na Fila

1. No menu lateral, clique em **"Queues and Streams"** para acessar a lista de filas.

2. Selecione a fila `pedidos`.

3. Na seção **"Publish Message"**, preencha os detalhes da mensagem:

   - **Payload:** Insira o corpo da mensagem no formato desejado. Aqui está um exemplo de payload para um pedido:
     ```json
     {
       "codigoPedido": 1001,
       "codigoCliente": 1,
       "itens": [
         {
           "produto": "lápis",
           "quantidade": 100,
           "preco": 1.1
         },
         {
           "produto": "caderno",
           "quantidade": 10,
           "preco": 1.0
         }
       ]
     }
     ```

4. Clique no botão **"Publish message"** para enviar a mensagem para a fila.

Isso fechará a aplicação e removerá os containers criados.

## Endpoints da API

### `GET /pedidos/{codigoCliente}`

- **Descrição:** Retorna todos os pedidos para um cliente específico identificado pelo código do cliente (`codigoCliente`).
- **Parâmetros:**
  - `codigoCliente` (int): Código do cliente para o qual os pedidos devem ser retornados.
- **Resposta:** Lista de pedidos para o cliente com o código especificado.

### `GET /pedidos/valorTotal/{codigoPedido}`

- **Descrição:** Retorna o valor total de um pedido específico identificado pelo código do pedido (`codigoPedido`).
- **Parâmetros:**
  - `codigoPedido` (int): Código do pedido para o qual o valor total deve ser calculado.
- **Resposta:** Valor total do pedido com o código especificado.

### `GET /pedidos/quantidadePorCliente/{codigoCliente}`

- **Descrição:** Retorna a quantidade total de pedidos realizados por um cliente específico identificado pelo código do cliente (`codigoCliente`).
- **Parâmetros:**
  - `codigoCliente` (int): Código do cliente para o qual a quantidade de pedidos deve ser retornada.
- **Resposta:** Quantidade total de pedidos realizados pelo cliente com o código especificado.

## Fechando a Aplicação

Para parar e remover os containers, utilize o comando:

```bash
docker-compose down
```
