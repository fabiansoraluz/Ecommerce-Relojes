
USE BD_Ecommers_Store
go


/*clave = 123*/
insert into Persona(NombreCompleto,Correo,Clave,Rol) values
('administrador','admin@example.com','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','Usuario')
go

insert into Categoria(Nombre) values
('Basquet'),
('Correr'),
('Entrenamiento'),
('Futbol'),
('Stake'),
('variados')
GO




select * from Persona;


