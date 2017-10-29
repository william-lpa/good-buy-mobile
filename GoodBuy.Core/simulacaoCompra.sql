-- Leite Integral Parmalat 1L
-- Leite Desnatado Tirol 1L
-- Batata 500 gr
-- Detergente 

select * from UnidadeMedida
-- c51b2057-0a0f-42f9-af2e-cb167cde6c21 - Litro
-- 53b91aba-b8ad-4549-8608-20a72f85d4cb - ml
-- a97740e5-3488-4eed-97bb-44072d9c6f8d - gr

select * from Tipo
insert into Tipo (nome) values ('Desnatado')
insert into Tipo (nome) values ('Integral')
insert into Tipo (nome) values ('Semi-desnatado')
insert into Tipo (nome) values ('Esterelizado')
insert into Tipo (nome) values ('Doce')
--------------------------------------------------------------------------------
select Produto.ID,Produto.Nome, Tipo.nome from Produto left join Tipo on (Produto.IdTipo = Tipo.id)
insert into Produto (Nome,IdUnidadeMedida,IdTipo,QuantidadeMensuravel) values ('Leite', 'c51b2057-0a0f-42f9-af2e-cb167cde6c21','E0E08FD2-2DFC-4342-BB8F-C32E8F3BDD05', 1) -- Leite Integral 1L
insert into Produto (Nome,IdUnidadeMedida,IdTipo,QuantidadeMensuravel) values ('Leite', 'c51b2057-0a0f-42f9-af2e-cb167cde6c21','6781781C-D56F-467D-8C29-B7A5D4929227', 1) -- Leite Desnatado 1L
insert into Produto (Nome,IdUnidadeMedida,IdTipo,QuantidadeMensuravel) values ('Leite', 'c51b2057-0a0f-42f9-af2e-cb167cde6c21','90C617D1-D49D-4F5D-8170-2F68D1D38071', 1) -- Leite Semi-desnatado 1L
insert into Produto (Nome,IdUnidadeMedida,IdTipo,QuantidadeMensuravel) values ('Batata', '53b91aba-b8ad-4549-8608-20a72f85d4cb',null, 1) -- Batata 1 ml
insert into Produto (Nome,IdUnidadeMedida,IdTipo,QuantidadeMensuravel) values ('Batata', 'a97740e5-3488-4eed-97bb-44072d9c6f8d','8208E1BF-DE11-4B84-B681-2E721D9A1063', 500) -- Batata Doce 500 gr
insert into Produto (Nome,IdUnidadeMedida,IdTipo,QuantidadeMensuravel) values ('DETERGENTE', '53b91aba-b8ad-4549-8608-20a72f85d4cb', null, 500) -- Detergente 500ml
insert into Produto (Nome,IdUnidadeMedida,IdTipo,QuantidadeMensuravel) values ('detergente', 'c51b2057-0a0f-42f9-af2e-cb167cde6c21', 'FE155EF0-6D00-4C52-8FAF-D3CD04E314CB', 1) -- Detergente Glicêmico 1L
--------------------------------------------------------------------------------
select * from Marca
insert into Marca(nome) values ('Parmatat')
insert into Marca(nome) values ('Aurora')
insert into Marca(nome) values ('Tirol')
insert into Marca(nome) values ('Ypê')
insert into Marca(nome) values ('Limpol')
--------------------------------------------------------------------------------
select CarteiraProduto.ID, Produto.Nome + ' - ' + ISNULL(Tipo.Nome,'') + '-' + Marca.Nome
		from CarteiraProduto
		left join Produto on (Produto.id = CarteiraProduto.IdProduto)
		left join Tipo on (Produto.IdTipo = Tipo.id)
		left join Marca on (Marca.id= CarteiraProduto.IdMarca)				
insert into CarteiraProduto (IdProduto, IdMarca) values ('70CD926D-02F1-4CFC-BA18-6E02812DAE81', 'F4A27B0B-3856-4C80-8FEC-6D58E4DD24A8') -- Leite Integral 1L - Tirol
insert into CarteiraProduto (IdProduto, IdMarca) values ('70CD926D-02F1-4CFC-BA18-6E02812DAE81', '1843D1D9-BA50-487D-B247-B0B3551EFF14') -- Leite Integral 1L - Parmatat
insert into CarteiraProduto (IdProduto, IdMarca) values ('FAC7D64E-7D55-444D-86EC-55F88E617ED9', '1843D1D9-BA50-487D-B247-B0B3551EFF14') -- Leite Desnatado 1L - Parmatat
insert into CarteiraProduto (IdProduto, IdMarca) values ('FAC7D64E-7D55-444D-86EC-55F88E617ED9', 'F4A27B0B-3856-4C80-8FEC-6D58E4DD24A8') -- Leite Desnatado 1L - Tirol
insert into CarteiraProduto (IdProduto, IdMarca) values ('7A84356D-E3F4-4C3B-8B82-7CDF7F5B1B9A', '227402D3-A49D-406F-9405-29CF5A6CADB9') -- Leite Semi-desnatado 1L - Aurora
insert into CarteiraProduto (IdProduto, IdMarca) values ('BF96A7D5-9E17-4224-90A7-2E44E233C6F7', '6f7e882c-af0d-45e9-be82-975354f75b8f') -- Batata 1 ml - Sem Marca
insert into CarteiraProduto (IdProduto, IdMarca) values ('BA4ADF81-03A5-4544-9143-5BADB1C5E8C1', '6f7e882c-af0d-45e9-be82-975354f75b8f') -- Batata Doce 500 gr - Sem Marca
insert into CarteiraProduto (IdProduto, IdMarca) values ('469D8E36-5A46-486E-A6DA-AE8341C6109B', '3F796354-D571-444C-8EEA-8EC3443E12A9') -- Detergente 500ml - Ype
insert into CarteiraProduto (IdProduto, IdMarca) values ('469D8E36-5A46-486E-A6DA-AE8341C6109B', '8733735C-A2AA-4469-93C9-83C751FEBE27') -- Detergente 500ml - Limpol
insert into CarteiraProduto (IdProduto, IdMarca) values ('F9B893E5-B45D-4275-8355-EBAA46EA57C5', '3F796354-D571-444C-8EEA-8EC3443E12A9') -- Detergente Esterelizado 1L - Ype
insert into CarteiraProduto (IdProduto, IdMarca) values ('F9B893E5-B45D-4275-8355-EBAA46EA57C5', '8733735C-A2AA-4469-93C9-83C751FEBE27') -- Detergente Esterelizado 1L - Limpol
--------------------------------------------------------------------------------
select * from Estabelecimento
--------------------------------------------------------------------------------
select * from Oferta
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('1A6595F0-3F91-4EB4-B02E-1D856AA162F1','1c55534e-25a6-4152-86ab-736d8268d2e0',1.99)-- Leite Integral 1L - Tirol - BIG					R$: 1.99
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('1A6595F0-3F91-4EB4-B02E-1D856AA162F1','c1d25de5-b531-4a1a-a40f-672e934ef75c',2.15)-- Leite Integral 1L - Tirol - Angeloni				R$: 2.15
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('1A6595F0-3F91-4EB4-B02E-1D856AA162F1','25b71d17-549b-4336-9f0e-2c3721eaf11c',3)-- Leite Integral 1L - Tirol - Bistek					R$: 3.00

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('DFF57768-1002-4DB3-A7DE-1A50225E1282','1c55534e-25a6-4152-86ab-736d8268d2e0',1.77)-- Leite Integral 1L - Parmatat - BIG				R$: 1.77
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('DFF57768-1002-4DB3-A7DE-1A50225E1282','c1d25de5-b531-4a1a-a40f-672e934ef75c',1.55)-- Leite Integral 1L - Parmatat - Angeloni			R$: 1.55
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('DFF57768-1002-4DB3-A7DE-1A50225E1282','25b71d17-549b-4336-9f0e-2c3721eaf11c',1.92)-- Leite Integral 1L - Parmatat - Bistek				R$: 1.92

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('47C8B088-4C46-49D8-959F-E3CBA1414987','1c55534e-25a6-4152-86ab-736d8268d2e0',1.77)-- Leite Desnatado 1L - Parmatat - BIG				R$: 1.77
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('47C8B088-4C46-49D8-959F-E3CBA1414987','c1d25de5-b531-4a1a-a40f-672e934ef75c',1.55)-- Leite Desnatado 1L - Parmatat - Angeloni			R$: 1.55
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('47C8B088-4C46-49D8-959F-E3CBA1414987','25b71d17-549b-4336-9f0e-2c3721eaf11c',1.92)-- Leite Desnatado 1L - Parmatat - Bistek			R$: 1.92

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('5187EA92-8C5E-4B0F-AB79-08F27D477373','1c55534e-25a6-4152-86ab-736d8268d2e0',1.99)-- Leite Desnatado 1L - Tirol - BIG					R$: 1.99
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('5187EA92-8C5E-4B0F-AB79-08F27D477373','c1d25de5-b531-4a1a-a40f-672e934ef75c',2.15)-- Leite Desnatado 1L - Tirol - Angeloni				R$: 2.15
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('5187EA92-8C5E-4B0F-AB79-08F27D477373','25b71d17-549b-4336-9f0e-2c3721eaf11c',1.91)-- Leite Desnatado 1L - Tirol - Bistek				R$: 1.91

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('350B432A-A850-4080-AFE2-22F8C95C563F','9f611854-819a-454b-9e50-a0347882000f',1)-- Leite Semi-desnatado 1L - Aurora - Galegão			R$: 1.00
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('350B432A-A850-4080-AFE2-22F8C95C563F','c1d25de5-b531-4a1a-a40f-672e934ef75c',1.5)-- Leite Semi-desnatado 1L - Aurora - Angeloni		R$: 1.50

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('F9CDEC06-9C4F-4743-A64D-2CCD538B1D10','9f611854-819a-454b-9e50-a0347882000f',0.5)-- Batata 1 ml - Sem Marca - Galegão					R$: 0.50

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('1C0C51FD-CABD-4BF8-B17E-4A52653DCBD3','1c55534e-25a6-4152-86ab-736d8268d2e0',1.78)-- Batata Doce 500 gr - Sem Marca - BIG				R$: 1.78
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('1C0C51FD-CABD-4BF8-B17E-4A52653DCBD3','c1d25de5-b531-4a1a-a40f-672e934ef75c',2.20)-- Batata Doce 500 gr - Sem Marca - Angeloni			R$: 2.20
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('1C0C51FD-CABD-4BF8-B17E-4A52653DCBD3','25b71d17-549b-4336-9f0e-2c3721eaf11c',2.30)-- Batata Doce 500 gr - Sem Marca - Bistek			R$: 2.30

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('A4427977-F084-4522-99CB-C27AD01686EB','1c55534e-25a6-4152-86ab-736d8268d2e0',0.86)-- Detergente 500ml - Ype - BIG						R$: 0.86
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('A4427977-F084-4522-99CB-C27AD01686EB','c1d25de5-b531-4a1a-a40f-672e934ef75c',0.67)-- Detergente 500ml - Ype - Angeloni					R$: 0.67
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('A4427977-F084-4522-99CB-C27AD01686EB','25b71d17-549b-4336-9f0e-2c3721eaf11c',0.99)-- Detergente 500ml - Ype - Bistek					R$: 0.99

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('5BF89AE5-C493-4516-B762-D9DF86139AA0','1c55534e-25a6-4152-86ab-736d8268d2e0',0.77)-- Detergente 500ml - Limpol - BIG					R$: 0.77
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('5BF89AE5-C493-4516-B762-D9DF86139AA0','c1d25de5-b531-4a1a-a40f-672e934ef75c',0.65)-- Detergente 500ml - Limpol - Angeloni				R$: 0.65
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('5BF89AE5-C493-4516-B762-D9DF86139AA0','25b71d17-549b-4336-9f0e-2c3721eaf11c',0.82)-- Detergente 500ml - Limpol - Bistek				R$: 0.82

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('A4C7C300-E069-47F0-9428-0F1F3C9A09C9','1c55534e-25a6-4152-86ab-736d8268d2e0',0.35) -- Detergente Esterelizado 1L - Ype - BIG			R$: 0.35
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('A4C7C300-E069-47F0-9428-0F1F3C9A09C9','c1d25de5-b531-4a1a-a40f-672e934ef75c',0.35) -- Detergente Esterelizado 1L - Ype - Angeloni		R$: 0.35
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('A4C7C300-E069-47F0-9428-0F1F3C9A09C9','25b71d17-549b-4336-9f0e-2c3721eaf11c',0.34) -- Detergente Esterelizado 1L - Ype - Bistek		R$: 0.34

insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('40C0C2B8-AB89-489B-91EF-FFB0C6F6AE42','1c55534e-25a6-4152-86ab-736d8268d2e0',0.37) -- Detergente Esterelizado 1L - Limpol - BIG		R$: 0.37
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('40C0C2B8-AB89-489B-91EF-FFB0C6F6AE42','c1d25de5-b531-4a1a-a40f-672e934ef75c',0.33) -- Detergente Esterelizado 1L - Limpol - Angeloni	R$: 0.33
insert into Oferta (IdCarteiraProduto,IdEstabelecimento,PrecoAtual) values ('40C0C2B8-AB89-489B-91EF-FFB0C6F6AE42','25b71d17-549b-4336-9f0e-2c3721eaf11c',0.32)--  Detergente Esterelizado 1L - Limpol - Bistek		R$: 0.32

--------------------------------------------------------------------------------
-- BIG	:												Total:	R$ 5.89		
-- Leite Integral Parmalat 1L -									R$ 1.77
-- Leite Desnatado Tirol 1L -									R$ 1.99
-- Batata 500 gr - (Batata doce)								R$ 1.78
-- Detergente  - ( Detergente Esterelizado 1L - Ype)			R$ 0.35
--------------------------------------------------------------------------------
-- Angeloni	:											Total:  R$ 6.23
-- Leite Integral Parmalat 1L -									R$ 1.55
-- Leite Desnatado Tirol 1L -									R$ 2.15
-- Batata 500 gr - (Batata doce)								R$ 2.20
-- Detergente  -  Detergente Esterelizado 1L - Limpol			R$ 0.33
--------------------------------------------------------------------------------
-- Bistek:												Total:  R$ 6.45
-- Leite Integral Parmalat 1L -									R$ 1.92
-- Leite Desnatado Tirol 1L -									R$ 1.91
-- Batata 500 gr - (Batata doce)								R$ 2.30
-- Detergente  -  Detergente Esterelizado 1L - Limpol			R$ 0.32

	

	