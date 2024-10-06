CREATE PROCEDURE prc_get_Login
    @pUser      NVARCHAR(50)
AS
BEGIN
    SELECT IdUsuario, [User], Pass, Nombre, Apellido, Activo, Bloqueado, ResetearPass, IntentosFallidos
    FROM Usuarios
    WHERE [User] = @pUser
END
GO

CREATE PROCEDURE prc_upd_UsuariosIncrementarIntentosFallidos
    @pIdUsuario INT
AS
DECLARE @intentosFallidos INT

SELECT @intentosFallidos = intentosfallidos + 1
FROM Usuarios
WHERE [IdUsuario] = @pIdUsuario

UPDATE Usuarios SET
        IntentosFallidos = @intentosFallidos,
        Bloqueado = IIF(@intentosFallidos < 3, Bloqueado, 1)
    WHERE [IdUsuario] = @pIdUsuario

RETURN @intentosFallidos
GO

CREATE PROCEDURE prc_upd_UsuariosResetearIntentosFallidos
    @pIdUsuario INT
AS
UPDATE Usuarios SET IntentosFallidos = 0 WHERE [IdUsuario] = @pIdUsuario
GO

CREATE PROCEDURE prc_upd_UsuariosResetearPass
    @pIdUsuario INT,
    @pPass      NVARCHAR(255)
AS
UPDATE Usuarios SET
        Pass = @pPass,
        ResetearPass = 0
    WHERE IdUsuario = @pIdUsuario
GO

CREATE PROCEDURE prc_get_Menu
    @pIdUsuario INT
AS
BEGIN
    SELECT p.IdPermiso, p.Nombre, p.Controller, p.Action, p.Icon, p.IdPadre
    FROM Usuarios u INNER JOIN Roles r ON u.IdRol = r.IdRol
        INNER JOIN RolesPorPermiso rpp ON r.IdRol = rpp.IdRol
        INNER JOIN Permisos p ON rpp.IdPermiso = p.IdPermiso
    WHERE IdUsuario = @pIdUsuario
    ORDER BY p.IdPermiso, p.IdPadre
END
GO

CREATE PROCEDURE prc_get_ValidarAcceso
    @pIdUsuario INT,
    @pPermiso NVARCHAR(50)
AS

DECLARE @result BIT = 0

SELECT @result =
    CASE 
        WHEN COUNT(*) > 0 THEN 1
        ELSE 0
    END
FROM Permisos p
    INNER JOIN RolesPorPermiso rpp on p.IdPermiso = rpp.IdPermiso
    INNER JOIN roles r ON rpp.IdRol = r.IdRol
    INNER JOIN Usuarios u ON r.IdRol = u.IdRol
WHERE u.IdUsuario = @pIdUsuario AND p.Nombre = @pPermiso

SELECT @result Result;
GO

CREATE PROCEDURE prc_get_Usuarios
    @pIdUsuario INT = NULL,
    @pUser      NVARCHAR(50) =  NULL
AS
BEGIN
    SELECT IdUsuario, [User], Pass, Nombre, Apellido, Activo, Bloqueado, ResetearPass, u.IdRol, r.Descripcion [DescripcionRol]
    FROM Usuarios u INNER JOIN Roles r on u.IdRol = r.IdRol
    WHERE (@pIdUsuario IS NULL OR IdUsuario = @pIdUsuario)
        and (@pUser is null or [User] = @pUser)
END
GO

CREATE PROCEDURE [dbo].[prc_ins_Usuario]
    @pUser          NVARCHAR(50),
    @pPass          NVARCHAR(50),
    @pNombre        NVARCHAR(50),
    @pApellido      NVARCHAR(50),
    @pActivo        BIT,
    @pIdRol         INT
AS
BEGIN
    INSERT INTO Usuarios
        ([User], Pass, Nombre, Apellido, Activo, IdRol, Bloqueado, ResetearPass, IntentosFallidos)
    VALUES(@pUser, @pPass, @pNombre, @pApellido, @pActivo, @pIdRol, 0, 1, 0)

    RETURN @@IDENTITY
END
GO

CREATE PROCEDURE prc_upd_Usuario
    @pIdUsuario     INT,
    @pUser          NVARCHAR(50),
    @pNombre        NVARCHAR(50),
    @pApellido      NVARCHAR(50),
    @pActivo        BIT,
    @pIdRol         INT
AS
BEGIN
    UPDATE Usuarios
    SET
        [User] = @pUser,
        Nombre = @pNombre,
        Apellido = @pApellido,
        Activo = @pActivo,
        IdRol = @pIdRol
    WHERE IdUsuario = @pIdUsuario
END
GO

CREATE PROCEDURE prc_del_Usuario
    @pIdUsuario     INT
AS
BEGIN
    DELETE Usuarios WHERE IdUsuario = @pIdUsuario
END
GO

CREATE PROCEDURE prc_get_Roles
    @pIdRol         INT = NULL,
    @pDescripcion   NVARCHAR(50) = NULL
AS
BEGIN
    select IdRol, Descripcion
    from Roles
    where (@pIdRol IS NULL OR IdRol = @pIdRol)
        and (@pDescripcion IS NULL OR Descripcion = @pDescripcion)
END
GO

CREATE PROCEDURE prc_ins_Rol
    @pDescripcion NVARCHAR(50)
AS
BEGIN
    insert into Roles
        (Descripcion)
    VALUES
        (@pDescripcion)

    RETURN @@IDENTITY
END
GO

CREATE PROCEDURE prc_upd_Rol
    @pIdRol INT,
    @pDescripcion NVARCHAR(50)
AS
BEGIN
    UPDATE Roles SET
    Descripcion = @pDescripcion
    where IdRol = @pIdRol
END
GO

CREATE PROCEDURE prc_get_RolesPorPermiso
    @pIdRol INT
AS
BEGIN
    select IdRol, IdPermiso
    from RolesPorPermiso
    where IdRol = @pIdRol
END
GO

CREATE PROCEDURE prc_ins_RolesPorPermiso
    @pIdRol INT,
    @pIdPermiso INT
AS
BEGIN
    insert into RolesPorPermiso
        (IdRol, IdPermiso)
    VALUES
        (@pIdRol, @pIdPermiso)
END
GO

CREATE PROCEDURE prc_del_RolesPorPermiso
    @pIdRol INT
AS
BEGIN
    DELETE RolesPorPermiso WHERE IdRol = @pIdRol
END
GO

CREATE PROCEDURE prc_get_Permisos
AS
select IdPermiso, Nombre, IdPadre
from Permisos
ORDER BY IdPermiso
Go

CREATE PROCEDURE prc_ins_Bitacora
    @pDescripcion       NVARCHAR(100),
    @pFecha             DATETIME,
    @pIdTipoEntidad     INT,
    @pIdTipoOperacion   INT,
    @pIdUsuario         INT
AS
BEGIN
    INSERT INTO Bitacora
        (Descripcion, Fecha, IdTipoEntidad, IdTipoOperacion, IdUsuario)
    VALUES
        (@pDescripcion, @pFecha, @pIdTipoEntidad, @pIdTipoOperacion, @pIdUsuario)
END
GO

CREATE PROCEDURE prc_get_Bitacora
    @pIdtipoEntidad INT = NULL
AS
select b.Descripcion, b.Fecha, te.Descripcion DescripcionTipoEntidad, u.Nombre, u.Apellido
from Bitacora b inner join TipoEntidad te on b.IdTipoEntidad = te.IdTipoEntidad
    inner join Usuarios u on b.IdUsuario = u.IdUsuario
where (@pIdtipoEntidad is null or b.IdTipoEntidad = @pIdtipoEntidad)
GO

CREATE PROCEDURE prc_get_Marcas
AS
SELECT IdMarca, Descripcion
FROM Marcas
GO

CREATE PROCEDURE prc_get_SKUs
    @pIdSKU         INT = NULL,
    @pCodigo        INT = NULL,
    @pNombre        NVARCHAR(50) = NULL
AS
BEGIN
    SELECT IdSKU, Codigo, Nombre, s.Descripcion, Activo, Especial, UnidadesPorBandeja, Stock, s.IdMarca, m.Descripcion DescripcionMarca
    FROM SKU s INNER JOIN Marcas m on s.IdMarca = m.IdMarca
    WHERE (@pIdSKU is null or IdSKU = @pIdSKU)
        AND (@pCodigo is null or Codigo = @pCodigo)
        AND (@pNombre is null or Nombre = @pNombre)
END
GO

CREATE PROCEDURE prc_ins_SKU
    @pCodigo                INT,
    @pNombre                NVARCHAR(50),
    @pDescripcion           NVARCHAR(100) = NULL,
    @pActivo                BIT,
    @pEspecial              BIT,
    @pUnidadesPorBandeja    INT,
    @pIdMarca               INT,
    @pIdUsuario             INT,
    @pAltaRegistro          DATETIME
AS
INSERT into SKU
    (Codigo, Nombre, Descripcion, Activo, Especial, UnidadesPorBandeja, IdMarca, IdUsuario, AltaRegistro)
VALUES
    (@pCodigo, @pNombre, @pDescripcion, @pActivo, @pEspecial, @pUnidadesPorBandeja, @pIdMarca, @pIdUsuario, @pAltaRegistro)

RETURN @@IDENTITY
GO

CREATE PROCEDURE prc_upd_SKU
    @pIdSKU                 INT,
    @pCodigo                INT,
    @pNombre                NVARCHAR(50),
    @pDescripcion           NVARCHAR(100) = NULL,
    @pActivo                BIT,
    @pEspecial              BIT,
    @pUnidadesPorBandeja    INT,
    @pIdMarca               INT
AS
UPDATE SKU SET
        Codigo = @pCodigo,
        Nombre = @pNombre,
        Descripcion = @pDescripcion,
        Activo = @pActivo,
        Especial = @pEspecial,
        UnidadesPorBandeja = @pUnidadesPorBandeja,
        IdMarca = @pIdMarca
    WHERE IdSKU = @pIdSKU
GO

CREATE PROCEDURE prc_get_SKUSugerencia
    @pSugerencia  NVARCHAR(50)
AS
SELECT TOP 10
    IdSKU, Codigo, Nombre
FROM SKU
WHERE
        CAST(codigo AS NVARCHAR(50)) = @pSugerencia
    OR
    Nombre LIKE '%' + @pSugerencia + '%'
GO

CREATE PROCEDURE [dbo].[prc_get_Choferes]
    @pNombre        NVARCHAR(50) = NULL,
    @pApellido      NVARCHAR(50) = NULL,
    @pActivo        BIT = NULL
AS
BEGIN
    SELECT IdChofer, Nombre, Apellido, Activo
    FROM Choferes
    where (@pNombre is null or Nombre = @pNombre)
        and (@pApellido is null or Apellido = @pApellido)
        and (@pActivo is null or Activo = @pActivo)
END
GO

CREATE PROCEDURE prc_ins_Choferes
    @pNombre        NVARCHAR(50),
    @pApellido      NVARCHAR(50),
    @pActivo        BIT
AS
BEGIN
    INSERT into Choferes
        (Nombre, Apellido, Activo)
    VALUES(@pNombre, @pApellido, @pActivo)

    RETURN @@IDENTITY
END
GO

CREATE PROCEDURE prc_upd_Choferes
    @pIdChofer      INT,
    @pNombre        NVARCHAR(50),
    @pApellido      NVARCHAR(50),
    @pActivo        BIT
AS
BEGIN
    UPDATE Choferes SET
    Nombre = @pNombre,
    Apellido = @pApellido,
    Activo = @pActivo
    WHERE IdChofer = @pIdChofer
END
GO

CREATE PROCEDURE [dbo].[prc_get_CanalesVentas]
    @pCodigo        INT = NULL,
    @pNombre        NVARCHAR(25) = NULL
AS
BEGIN
    SELECT IdCanalVenta, Nombre, Descripcion, Codigo
    FROM CanalesVentas
    WHERE (@pCodigo is null or Codigo = @pCodigo)
        AND (@pNombre is null or Nombre = @pNombre)
END
GO

create PROCEDURE prc_ins_CanalesVentas
    @pCodigo        int,
    @pNombre        NVARCHAR(25),
    @pDescripcion   NVARCHAR(50) = null
AS
insert into CanalesVentas
    (Codigo, Nombre, Descripcion)
VALUES
    (@pCodigo, @pNombre, @pDescripcion)

RETURN @@IDENTITY
GO

create PROCEDURE prc_upd_CanalesVentas
    @pIdCanalVenta  int,
    @pCodigo        int,
    @pNombre        NVARCHAR(25),
    @pDescripcion   NVARCHAR(50) = null
AS
UPDATE CanalesVentas SET
        Codigo = @pCodigo,
        Nombre = @pNombre,
        @pDescripcion = @pDescripcion
        where IdCanalVenta = @pIdCanalVenta
GO

CREATE PROCEDURE prc_ins_Transportes
    @pNombre    NVARCHAR(50),
    @pPatente   NVARCHAR(7),
    @pActivo    BIT
AS
BEGIN
    INSERT into Transportes
        (Nombre, Patente, Activo)
    VALUES(@pNombre, @pPatente, @pActivo)

    RETURN @@IDENTITY
END
GO

CREATE PROCEDURE prc_upd_Transportes
    @pIdTransporte  INT,
    @pNombre        NVARCHAR(50),
    @pPatente       NVARCHAR(7),
    @pActivo        BIT
AS
BEGIN
    UPDATE Transportes SET
    Nombre = @pNombre,
    Patente = @pPatente,
    Activo = @pActivo
    WHERE IdTransporte = @pIdTransporte
END
GO

CREATE PROCEDURE prc_get_Transportes
    @pNombre    NVARCHAR(50) = NULL,
    @pPatente   NVARCHAR(7) = NULL,
    @pActivo    BIT = NULL
AS
BEGIN
    SELECT IdTransporte, Nombre, Patente, Activo
    FROM Transportes
    WHERE (@pNombre is null or Nombre = @pNombre)
    and (@pPatente is null or Patente = @pPatente)
    and (@pActivo is null or Activo = @pActivo)
END
GO