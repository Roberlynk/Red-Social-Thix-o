create database Thixio

use Thixio

create table Usuario(
	idUsuario int identity(1,1) primary key,
	nombre varchar(45),
	apellido varchar(45),
	telefono varchar(12),
	correo varchar(45),
	nombreUsuario varchar(45),
	contraseña varchar(255),
	statuss bit null default '0'
)
create table amigo(
	idAmigo int,
	idUsuario int,
	constraint FK_Usuario_Amigo foreign key (idUsuario) references Usuario(idUsuario),
	constraint PK_Amigo primary key (idUsuario, idAmigo)	
)
create table Publicacion(
	idPublicacion int identity(1,1) primary key,
	contenido varchar(300), 
	fechaHora datetime default GETDATE(),
	idUsuario int,
	constraint FK_Usuario_publicacion foreign key (idUsuario) references Usuario(idUsuario)
)
create table Comentario(
	idComentario int identity(1,1) primary key,
	idUsuario int,
	idPublicacion int,
	contenido varchar(300),
	fechaHora datetime default GETDATE(),
	constraint FK_Usuario foreign key (idUsuario) references Usuario(idUsuario),
	constraint FK_Publicacion foreign key (idPublicacion) references Publicacion(idPublicacion)
)
