insert into Permisos
    (nombre, controller, [action], icon, idpadre)
values
    ('Home', 'Home', 'Index', 'fa-solid fa-house', null),
    ('Usuario', 'Usuario', 'Index', 'fa-solid fa-user', null),
    ('SKU', 'SKU', 'Index', 'fa-solid fa-box-archive', null)

insert into Roles
    (descripcion)
values
    ('Administrador')

declare @idRol as int

select @idRol = IdRol
from Roles
where Descripcion = 'Administrador'

insert into RolesPorPermiso
    (IdRol, IdPermiso)
select @idRol, idpermiso
from Permisos

insert into Usuarios
    ([User], Pass, Nombre, Apellido, Activo, Bloqueado, ResetearPass, IntentosFallidos, IdRol)
values('admin', '', 'Pepe', 'Admin', 1, 0, 0 , 0, @idRol)

insert into Marcas
VALUES
    ('Marca A'),
    ('Marca B')

insert into TipoEntidad
values
    ('Usuario'),
    ('Rol'),
    ('SKU'),
    ('Canal de venta')

insert into TipoOperacion
    (Descripcion)
VALUEs
    ('Creación'),
    ('Edición'),
    ('Borrado')