DELETE FROM LogsExecucao;
DELETE FROM PassosOrquestracao;
DELETE FROM Orquestracoes;
DELETE FROM Conectores;
DELETE FROM Integracoes;

DBCC CHECKIDENT ('Integracoes', RESEED, 0);
DBCC CHECKIDENT ('Conectores', RESEED, 0);
DBCC CHECKIDENT ('Orquestracoes', RESEED, 0);
DBCC CHECKIDENT ('PassosOrquestracao', RESEED, 0);
DBCC CHECKIDENT ('LogsExecucao', RESEED, 0);

SELECT * FROM Orquestracoes;
SELECT * FROM PassosOrquestracao;
SELECT * FROM Integracoes;
SELECT * FROM Conectores;
SELECT * FROM LogsExecucao;

-- Cenário 1 – Envio de SMS (Fluxo simples HTTP POST)
-- Integração
INSERT INTO Integracoes (Nome, Descricao, Ativo)
VALUES ('Envio de SMS', 'Envia SMS de confirmação de pagamento', 1);

-- Conector
INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (1, 'HTTP', 'https://httpbin.org/post', 'POST');

-- Orquestração
INSERT INTO Orquestracoes (Nome, Descricao)
VALUES ('Orquestração SMS', 'Executa o envio de SMS via conector HTTP');

-- Passo da Orquestração
INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (1, 1, 1);

-- Teste no Swagger: POST /api/orquestrador/1
-- Deve executar o Conector 101, chamar https://httpbin.org/post, e gravar um log de sucesso.


------------------------------------------------------------------------------------------

--Cenário 2 – Consulta de CEP (HTTP GET)
-- Integração
INSERT INTO Integracoes (Nome, Descricao, Ativo)
VALUES ('Consulta CEP', 'Consulta endereço pelo CEP via API ViaCEP', 1);

-- Conector
INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (2, 'HTTP', 'https://viacep.com.br/ws/01001000/json/', 'GET');

-- Orquestração
INSERT INTO Orquestracoes (Nome, Descricao)
VALUES ('Orquestração CEP', 'Consulta endereço via ViaCEP');

-- Passo
INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (2, 1, 2);

-- Teste no Swagger: POST /api/orquestrador/2
-- Deve retornar o JSON da API ViaCEP e gravar o log como sucesso.

-----------------------------------------------------------------------------------------------------

-- Cenário 3 – Fluxo com 2 passos (GET + POST) - Simula um processo com dois conectores encadeados.
-- Integração
INSERT INTO Integracoes (Nome, Descricao, Ativo)
VALUES ('Fluxo Duplo HTTP', 'Executa uma requisição GET seguida de POST', 1);

-- Conectores
INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (3, 'HTTP', 'https://httpbin.org/get', 'GET');

INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (3, 'HTTP', 'https://httpbin.org/post', 'POST');

-- Orquestração
INSERT INTO Orquestracoes (Nome, Descricao)
VALUES ('Orquestração GET+POST', 'Fluxo encadeado de duas chamadas HTTP');

-- Passos
INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (3, 1, 3);

INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (3, 2, 1);

-- Teste no Swagger: POST /api/orquestrador/3
-- Deve executar GET → POST na sequência e gerar 2 logs (um para cada passo).

---------------------------------------------------------------------------------------------------------

-- Cenário 4 – Simulação de erro (endpoint inválido) - Testa tratamento de erro e registro no log.
-- Integração
INSERT INTO Integracoes (Nome, Descricao, Ativo)
VALUES ('Integração com Erro', 'Endpoint inexistente para teste de falha', 1);

-- Conector
INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (4, 'HTTP', 'https://urlinexistente1234.com.br/api', 'GET');

-- Orquestração e Passo
INSERT INTO Orquestracoes (Nome, Descricao)
VALUES ('Orquestração com Erro', 'Teste de erro de conexão');

INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (4, 1, 401);

-- Teste no Swagger: POST /api/orquestrador/4
-- Deve gerar log com erro (Status = 'Erro') no SQL.

---------------------------------------------------------------------------------------------------------

-- Cenário 5 – Orquestração com 3 passos (simulação de processo de venda) 
-- Exemplo mais completo, usado para demonstrar em apresentação.
-- Integração
INSERT INTO Integracoes (Nome, Descricao, Ativo)
VALUES ('Processo de Venda', 'Simula fluxo de consulta cliente, cálculo e confirmação', 1);

-- Conectores
INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (5, 'HTTP', 'https://httpbin.org/get', 'GET'); -- Consulta cliente

INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (5, 'HTTP', 'https://httpbin.org/post', 'POST'); -- Envia pedido

INSERT INTO Conectores (IntegracaoID, Tipo, Endpoint, Metodo)
VALUES (5, 'HTTP', 'https://httpbin.org/post', 'POST'); -- Confirma pagamento

-- Orquestração e Passos
INSERT INTO Orquestracoes (Nome, Descricao)
VALUES ('Orquestração Venda', 'Fluxo de três etapas simulando processo de venda');

INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (5, 1, 501);

INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (5, 2, 502);

INSERT INTO PassosOrquestracao (OrquestracaoID, Ordem, ConectorID)
VALUES (5, 3, 503);

-- Teste no Swagger: POST /api/orquestrador/5
-- Deve gerar 3 logs (sucesso para cada passo) e exibir execução completa.