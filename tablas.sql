CREATE TABLE Permisos
(
    IdPermiso INT PRIMARY KEY IDENTITY (1, 1),
    Nombre NVARCHAR (50) NOT NULL,
    Controller NVARCHAR (50) NOT NULL,
    [Action] NVARCHAR (50) NOT NULL,
    Icon NVARCHAR(50) NULL,
    IdPadre INT NULL ,
    CONSTRAINT FK_Permisos_Permisos FOREIGN KEY (IdPadre) REFERENCES Permisos(IdPermiso),
);

CREATE TABLE Roles
(
    IdRol INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE RolesPorPermiso
(
    IdRol INT NOT NULL FOREIGN KEY REFERENCES Roles(IdRol),
    IdPermiso INT NOT NULL FOREIGN KEY REFERENCES Permisos(IdPermiso),
    PRIMARY KEY (IdRol, IdPermiso)
);

CREATE TABLE Usuarios
(
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    [User] NVARCHAR(50) NOT NULL UNIQUE,
    Pass NVARCHAR(255) NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Apellido NVARCHAR(50) NOT NULL,
    Activo BIT NOT NULL,
    Bloqueado BIT NOT NULL,
    ResetearPass BIT NOT NULL,
    IntentosFallidos INT NOT NULL,
    IdRol INT NOT NULL FOREIGN KEY REFERENCES Roles(IdRol)
);

CREATE TABLE TipoEntidad
(
    IdTipoEntidad INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(25) NOT NULL UNIQUE
);

CREATE TABLE TipoOperacion
(
    IdTipoOperacion INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(25) NOT NULL UNIQUE
);

CREATE TABLE Bitacora
(
    IdBitacora INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(100) NOT NULL,
    Fecha DATETIME NOT NULL,
    IdTipoEntidad INT NOT NULL FOREIGN KEY REFERENCES TipoEntidad(IdTipoEntidad),
    IdTipoOperacion INT NOT NULL FOREIGN KEY REFERENCES TipoOperacion(IdTipoOperacion),
    IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuarios(IdUsuario)
);

CREATE TABLE Marcas
(
    IdMarca INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE SKU
(
    IdSKU INT PRIMARY KEY IDENTITY(1,1),
    Codigo INT NOT NULL UNIQUE,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(100),
    Activo BIT NOT NULL,
    Especial BIT NOT NULL,
    -- TODO: revisar
    UnidadesPorBandeja INT NULL,
    -- TODO: revisar
    UnidadesPorCaja INT NULL,
    Stock INT NOT NULL,
    IdMarca INT NOT NULL FOREIGN KEY REFERENCES Marcas(IdMarca),
    IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuarios(IdUsuario),
    AltaRegistro DATETIME NOT NULL
);

CREATE TABLE CanalesVentas
(
    IdCanalVenta INT PRIMARY KEY IDENTITY(1,1),
    Codigo INT NOT NULL,
    Nombre NVARCHAR(25) NOT NULL UNIQUE,
    Descripcion NVARCHAR(50) NULL
);

CREATE TABLE Choferes
(
    IdChofer INT PRIMARY KEY NOT NULL IDENTITY(1,1),
    Nombre NVARCHAR(50) NOT NULL,
    Apellido NVARCHAR(50) NOT NULL,
    Activo BIT NOT NULL
);

CREATE TABLE Transportes
(
    IdTransporte INT PRIMARY KEY NOT NULL IDENTITY(1,1),
    Nombre NVARCHAR(50),
    Patente NVARCHAR(7),
    Activo BIT NOT NULL
);

CREATE TABLE EstadosRemitos
(
    IdEstado INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(25) NOT NULL
);

CREATE TABLE Remitos
(
    IdRemito int PRIMARY KEY IDENTITY(1,1),
    Numero NVARCHAR(100) NOT NULL UNIQUE,
    Fecha DATE NOT NULL,
    IdTransporte INT NULL FOREIGN KEY REFERENCES Transportes(IdTransporte),
    IdChofer INT NULL FOREIGN KEY REFERENCES Choferes(IdChofer),
    IdEstadoRemito INT NOT NULL FOREIGN KEY REFERENCES EstadosRemitos(IdEstado),
    IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuarios(IdUsuario),
    AltaRegistro DATETIME NOT NULL
);

create TABLE RemitosDetalles
(
    IdRemito INT NOT null FOREIGN KEY REFERENCES Remitos(IdRemito),
    IdSKU INT NOT null FOREIGN key REFERENCES SKU(IdSKU),
    NombreSKU NVARCHAR(50) NOT NULL,
    Cantidad int not null,
    PRIMARY KEY (IdRemito, IdSKU)
);

create table MovimientosStock
(
    IdMovimiento int PRIMARY key IDENTITY(1,1),
    NumeroRemito int not null UNIQUE,
    IdCanalVenta int not null FOREIGN KEY REFERENCES canalesventas(IdCanalVenta),
    TipoStock NVARCHAR(255) NULL,
    Motivo NVARCHAR(255) NULL,
    Fecha DATE NOT NULL,
    AltaRegistro DATETIME not NULL
);

CREATE table MovimientosStockDetalles
(
    IdMovimiento int NOT null FOREIGN key REFERENCES MovimientosStock(IdMovimiento),
    IdSKU INT NOT null FOREIGN key REFERENCES SKU(IdSKU),
    NombreSKU NVARCHAR(50) NOT NULL,
    Cantidad int not null,
    PRIMARY KEY (IdMovimiento, IdSKU)
);

CREATE table TiposDevolucion
(
    IdTipoDevolucion int PRIMARY key IDENTITY(1,1),
    Descripcion NVARCHAR(25) NOT NULL UNIQUE
);

create table Devoluciones
(
    IdDevolucion int NOT NULL PRIMARY KEY IDENTITY(1,1),
    Fecha DATE NOT NULL,
    IdTipoDevolucion int NOT NULL FOREIGN KEY REFERENCES TiposDevolucion(IdTipoDevolucion)
);

create TABLE DevolucionesDetalle
(
    IdDevolucion int NOT NULL FOREIGN KEY REFERENCES Devoluciones(IdDevolucion),
    IdSKU INT NOT null FOREIGN key REFERENCES SKU(IdSKU),
    NombreSKU NVARCHAR(50) NOT NULL,
    Cantidad int not null,
    PRIMARY KEY (IdDevolucion, IdSKU)
);

create TABLE TiposBaja
(
    IdTipoBaja int PRIMARY key IDENTITY(1,1),
    Descripcion NVARCHAR(25) NOT NULL UNIQUE
);

CREATE table Bajas
(
    IdBaja int not NULL PRIMARY key IDENTITY(1,1),
    Fecha date not null,
    IdTipoBaja int not null FOREIGN key REFERENCES TiposBaja(IdTipoBaja)
);

create TABLE BajasDetalle
(
    IdBaja int NOT NULL FOREIGN KEY REFERENCES Bajas(IdBaja),
    IdSKU INT NOT null FOREIGN key REFERENCES SKU(IdSKU),
    NombreSKU NVARCHAR(50) NOT NULL,
    Cantidad int not null,
    PRIMARY KEY (IdBaja, IdSKU)
);