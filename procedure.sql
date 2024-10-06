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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[int_transporte]
    @Patente NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @Activo BIT,
    @Mensaje VARCHAR(500) OUTPUT,
    @Resultado INT OUTPUT,
    @IdTransporte INT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Resultado = 0;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar si el transporte ya existe
        IF EXISTS (SELECT 1
    FROM Transportes
    WHERE Patente = @Patente)
        BEGIN
        -- Si existe, devolver mensaje y resultado
        SET @Resultado = -1;
        -- C�digo de error para indicar que ya existe
        SET @Mensaje = 'El transporte con la patente ya existe.';
    END
        ELSE
        BEGIN
        -- Insertar nuevo transporte
        INSERT INTO Transportes
            (Patente, Nombre, Activo)
        VALUES
            (@Patente, @Nombre, @Activo);

        -- Obtener el Id del nuevo transporte
        SET @Resultado = SCOPE_IDENTITY();
        -- Devuelve el ID del nuevo transporte
        SET @Mensaje = 'Transporte registrado exitosamente.';
    END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Revertir la transacci�n en caso de error
        ROLLBACK TRANSACTION;
        SET @Mensaje = ERROR_MESSAGE();
        SET @Resultado = -2;  -- C�digo de error gen�rico
    END CATCH
END;
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prc_get_Transportistas]
AS
BEGIN
    SELECT IdTransporte, Nombre, Patente, Activo
    FROM transportes
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prc_int_transporte]
    @Apellido NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @Activo BIT,
    @Mensaje VARCHAR(500) OUTPUT,
    @Resultado INT OUTPUT,
    @IdChofer INT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Resultado = 0;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar si el transporte ya existe
        IF EXISTS (SELECT 1
    FROM Choferes
    WHERE Nombre = @Nombre AND Apellido = @Apellido)
        BEGIN
        -- Si existe, devolver mensaje y resultado
        SET @Resultado = -1;
        -- C�digo de error para indicar que ya existe
        SET @Mensaje = 'El chofer con la nombre y apoellido ya existe.';
    END
        ELSE
        BEGIN
        -- Insertar nuevo transporte
        INSERT INTO Choferes
            (Apellido, Nombre, Activo)
        VALUES
            (@Apellido, @Nombre, @Activo);

        -- Obtener el Id del nuevo transporte
        SET @Resultado = SCOPE_IDENTITY();
        -- Devuelve el ID del nuevo transporte
        SET @Mensaje = 'chofer registrado exitosamente.';
    END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Revertir la transacci�n en caso de error
        ROLLBACK TRANSACTION;
        SET @Mensaje = ERROR_MESSAGE();
        SET @Resultado = -2;  -- C�digo de error gen�rico
    END CATCH
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prc_upd_Choferes]
    @Apellido NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @Activo BIT,
    @Mensaje VARCHAR(500) OUTPUT,
    @Resultado INT OUTPUT,
    @IdChofer INT
-- Mantenerlo como par�metro de entrada
AS
BEGIN
    SET NOCOUNT ON;
    SET @Resultado = 0;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar si el transportista ya existe
        SELECT @IdChofer = IdChofer
    FROM Choferes


        IF @IdChofer IS NOT NULL
        BEGIN
        -- Actualizar el transportista existente
        UPDATE Choferes
            SET Nombre = @Nombre,
                Activo = @Activo
             -- Usar el nombre correcto aqu�
          WHERE IdChofer = @IdChofer;
        SET @Resultado = @IdChofer;
        -- ID del transportista actualizado
        SET @Mensaje = 'chofer actualizado exitosamente.';
    END
        ELSE
        BEGIN
        -- Insertar nuevo transportista
        INSERT INTO Choferes
            (Apellido, Nombre, Activo)
        VALUES
            (@Apellido, @Nombre, @Activo);

        -- Obtener el ID del nuevo transportista
        SET @IdChofer = SCOPE_IDENTITY();
        -- Devuelve el ID del nuevo transportista
        SET @Resultado = @IdChofer;
        -- Establecer resultado
        SET @Mensaje = 'chofer registrado exitosamente.';
    END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Revertir la transacci�n en caso de error
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;  -- Aseg�rate de revertir solo si hay una transacci�n activa

        SET @Mensaje = ERROR_MESSAGE();  -- Guardar el mensaje de error
        SET @Resultado = -1;  -- Indicar que ocurri� un error
        SET @IdChofer = NULL;  -- Aseg�rate de que sea NULL en caso de error
    END CATCH
END;
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prc_upd_Transportista]
    @Patente NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @Activo BIT,
    @Mensaje VARCHAR(500) OUTPUT,
    @Resultado INT OUTPUT,
    @IdTransporte INT
-- Mantenerlo como par�metro de entrada
AS
BEGIN
    SET NOCOUNT ON;
    SET @Resultado = 0;
    SET @Mensaje = '';

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar si el transportista ya existe
        SELECT @IdTransporte = IdTransporte
    -- Cambiar 'Id' por 'TransportistaId' (o el nombre correcto)
    FROM Transportes
    WHERE Patente = @Patente;

        IF @IdTransporte IS NOT NULL
        BEGIN
        -- Actualizar el transportista existente
        UPDATE Transportes
            SET Nombre = @Nombre,
                Activo = @Activo
             -- Usar el nombre correcto aqu�
          WHERE IdTransporte = @IdTransporte;
        SET @Resultado = @IdTransporte;
        -- ID del transportista actualizado
        SET @Mensaje = 'Transportista actualizado exitosamente.';
    END
        ELSE
        BEGIN
        -- Insertar nuevo transportista
        INSERT INTO Transportes
            (Patente, Nombre, Activo)
        VALUES
            (@Patente, @Nombre, @Activo);

        -- Obtener el ID del nuevo transportista
        SET @IdTransporte = SCOPE_IDENTITY();
        -- Devuelve el ID del nuevo transportista
        SET @Resultado = @IdTransporte;
        -- Establecer resultado
        SET @Mensaje = 'Transportista registrado exitosamente.';
    END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Revertir la transacci�n en caso de error
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;  -- Aseg�rate de revertir solo si hay una transacci�n activa

        SET @Mensaje = ERROR_MESSAGE();  -- Guardar el mensaje de error
        SET @Resultado = -1;  -- Indicar que ocurri� un error
        SET @IdTransporte = NULL;  -- Aseg�rate de que sea NULL en caso de error
    END CATCH
END;
GO

-- ALTER PROCEDURE [dbo].[prc_ins_Rol]
--     @Descripcion NVARCHAR(50),
--     @Mensaje VARCHAR(500) OUTPUT,
--     @Resultado INT OUTPUT,
--     @IdRol INT OUTPUT,  -- Cambiado a OUTPUT para devolver el IdRol
--     @Permisos NVARCHAR(MAX)  -- Lista de permisos en formato JSON o similar
-- AS
-- BEGIN
--     SET NOCOUNT ON;
--     SET @Resultado = 0;
--     SET @Mensaje = '';

--     BEGIN TRY
--         BEGIN TRANSACTION;

--         -- Verificar si el rol existe
--         IF EXISTS (SELECT 1 FROM Roles WHERE IdRol = @IdRol)
--         BEGIN
--             -- Actualizar rol existente
--             UPDATE Roles
--             SET Descripcion = @Descripcion
--             WHERE IdRol = @IdRol;

--             -- Mensaje y resultado para actualización
--             SET @Resultado = @IdRol;
--             SET @Mensaje = 'Rol actualizado exitosamente.';

--             -- Eliminar permisos existentes del rol
--             DELETE FROM RolesPorPermiso
--             WHERE IdRol = @IdRol;
--         END
--         ELSE
--         BEGIN
--             -- Insertar nuevo rol
--             INSERT INTO Roles (Descripcion)
--             VALUES (@Descripcion);

--             -- Obtener el Id del nuevo rol
--             SET @Resultado = SCOPE_IDENTITY();
--             SET @IdRol = @Resultado;  -- Actualiza el IdRol para uso en permisos
--             SET @Mensaje = 'Rol registrado exitosamente.';
--         END

--         -- Insertar permisos asociados al rol
--         IF @Permisos IS NOT NULL AND LEN(@Permisos) > 0
--         BEGIN
--             -- Utilizar STRING_SPLIT para dividir la lista de permisos y agregar cada permiso
--             INSERT INTO RolesPorPermiso (IdRol, IdPermiso)
--             SELECT @IdRol, TRY_CAST(value AS INT)  -- Asegurarse de que los permisos sean válidos
--             FROM STRING_SPLIT(@Permisos, ',')
--             WHERE TRY_CAST(value AS INT) IS NOT NULL;  
--         END

--         COMMIT TRANSACTION;
--     END TRY
--     BEGIN CATCH
--         -- Revertir la transacción en caso de error
--         ROLLBACK TRANSACTION;
--         SET @Mensaje = ERROR_MESSAGE();
--     END CATCH
-- END;
-- GO