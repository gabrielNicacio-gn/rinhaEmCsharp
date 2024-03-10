
CREATE TABLE "clientes"(
        "id" INT PRIMARY KEY
        "saldo" INT NOT NULL
        "limite" INT NOT NULL
);

CREATE TABLE "transacoes" (
        "id" SERIAL PRIMARY KEY,[]
        "valor" INT NOT NULL,
        "id_cliente" INT NOT NULL,
        "tipo_transacao" CHAR(1) NOT NULL,
        "descricao" VARCHAR(10) NOT NULL,
        "realizada_em" DATETIME NOT NULL,
        FOREIGN KEY ("id_cliente") REFERENCES "clientes" ("id")
);

CREATE INDEX idx_transacoes_id_cliente ON transacoes (id_cliente);
CREATE INDEX idx_transacoes_realizada_em ON transacoes (realizada_em DESC);

INSERT INTO clientes(id, saldo, limite) 
VALUES(
    (1, 0, 100000),
    (2, 0, 80000),
    (3, 0, 1000000),
    (4, 0, 10000000),
    (5, 0, 500000);
);
