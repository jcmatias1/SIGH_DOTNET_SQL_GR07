// SIGH - Hospital General Monster Salud
// Talleres Login y CRUD MVC con ASP.NET Core y SQL Server - Grupo GR07
// Crea la base y carga los datos iniciales al arrancar (sin ejecutar scripts .sql).
using Microsoft.EntityFrameworkCore;

namespace SIGH_DOTNET_GR07.Data;

/// <summary>
/// Igual que Hibernate con ddl-auto=update en Java: si la base bd_monster_gr07 no existe
/// (o existe vacía), EnsureCreated la crea con todas las tablas a partir de las clases del
/// modelo y del SighDbContext. Después se insertan los datos base solo si faltan
/// (INSERT ... WHERE NOT EXISTS), así que se puede ejecutar en cada arranque sin duplicar nada.
/// Contraseña de los usuarios de prueba: Monster2026 (SHA-256).
/// </summary>
public static class DatosIniciales
{
    private const string HashMonster2026 = "eae5f19894f1c02d982e2df6434b2dcbf4844a6afa52b96bd2fac1dc6c6ede38";

    public static void Inicializar(SighDbContext db, ILogger logger)
    {
        bool creada = db.Database.EnsureCreated();
        if (creada)
        {
            logger.LogInformation("[SIGH] Base de datos creada a partir del modelo de Entity Framework");
        }

        int total = 0;
        foreach (string sql in Sentencias())
        {
            total += db.Database.ExecuteSqlRaw(sql);
        }
        logger.LogInformation("[SIGH] Datos iniciales: {Total} registros nuevos", total);
    }

    private static IEnumerable<string> Sentencias()
    {
        // ---------- Catálogos ----------
        yield return """
            INSERT INTO XEEST_ESTAD (XEEST_CODIGO, XEEST_DESCRI)
            SELECT v.c, v.d FROM (VALUES ('A', 'ACTIVO'), ('I', 'INACTIVO'), ('B', 'BLOQUEADO')) AS v(c, d)
            WHERE NOT EXISTS (SELECT 1 FROM XEEST_ESTAD t WHERE t.XEEST_CODIGO = v.c)
            """;
        yield return """
            INSERT INTO PEGEN_GENER (PEGEN_CODIGO, PEGEN_DESCRI)
            SELECT v.c, v.d FROM (VALUES ('M', N'Masculino'), ('F', N'Femenino')) AS v(c, d)
            WHERE NOT EXISTS (SELECT 1 FROM PEGEN_GENER t WHERE t.PEGEN_CODIGO = v.c)
            """;
        yield return """
            INSERT INTO PEESC_ESTCI (PEESC_CODIGO, PEESC_DESCRI)
            SELECT v.c, v.d FROM (VALUES ('S', N'Soltero/a'), ('C', N'Casado/a'), ('D', N'Divorciado/a'),
                                         ('V', N'Viudo/a'), ('U', N'Unión de hecho')) AS v(c, d)
            WHERE NOT EXISTS (SELECT 1 FROM PEESC_ESTCI t WHERE t.PEESC_CODIGO = v.c)
            """;

        // ---------- Departamentos y cargos (listbox del Taller 1) ----------
        yield return """
            INSERT INTO PEDEP_DEPAR (PEDEP_CODIGO, PEDEP_DESCRI)
            SELECT v.c, v.d FROM (VALUES ('ADM', N'Administración'), ('CEX', N'Consulta externa'), ('EME', N'Emergencia'),
                                         ('HOS', N'Hospitalización'), ('LAB', N'Laboratorio'), ('IMA', N'Imagenología'),
                                         ('FAR', N'Farmacia'), ('QUI', N'Quirófano'), ('FAC', N'Facturación')) AS v(c, d)
            WHERE NOT EXISTS (SELECT 1 FROM PEDEP_DEPAR t WHERE t.PEDEP_CODIGO = v.c)
            """;
        yield return """
            INSERT INTO PECAR_CARGO (PEDEP_CODIGO, PECAR_CODIGO, PECAR_DESCRI)
            SELECT v.dep, v.car, v.d FROM (VALUES ('ADM', 'ADS', N'Administrador de sistemas'), ('CEX', 'MED', N'Médico'),
                                                  ('HOS', 'ENF', N'Enfermero/a'), ('FAR', 'FAR', N'Farmacéutico/a')) AS v(dep, car, d)
            WHERE NOT EXISTS (SELECT 1 FROM PECAR_CARGO t WHERE t.PEDEP_CODIGO = v.dep AND t.PECAR_CODIGO = v.car)
            """;

        // ---------- Empleados ----------
        yield return """
            INSERT INTO PEEMP_EMPLE (PEEMP_CODIGO, PEGEN_CODIGO, PEESC_CODIGO, PEDEP_CODIGO, PECAR_CODIGO, PEEMP_CEDULA,
                                     PEEMP_APELLI, PEEMP_NOMBRE, PEEMP_FECNAC, PEEMP_EMAIL, PEEMP_TELEFO, PEEMP_DIRECC,
                                     PEEMP_REGPRO, PEEMP_ESTADO)
            SELECT v.* FROM (VALUES
              ('EMP0000001', 'M', 'C', 'ADM', 'ADS', '1712345678', N'PEREZ LOPEZ', N'JUAN CARLOS', CAST('1988-04-12' AS DATE),
               'jperez@monstersalud.ec', '0991234567', N'Av. Amazonas N24-15, Quito', NULL, 'A'),
              ('EMP0000002', 'F', 'S', 'CEX', 'MED', '1723456789', N'TORRES VACA', N'MARIA JOSE', CAST('1990-09-03' AS DATE),
               'mtorres@monstersalud.ec', '0987654321', N'Calle Rio Coca E5-20, Quito', 'MSP-17-04521', 'A'),
              ('EMP0000003', 'F', 'C', 'HOS', 'ENF', '1734567890', N'GUERRERO PAZ', N'ANA LUCIA', CAST('1993-01-25' AS DATE),
               'aguerrero@monstersalud.ec', '0976543210', N'Av. 6 de Diciembre, Quito', 'MSP-17-08830', 'A'),
              ('EMP0000004', 'M', 'S', 'FAR', 'FAR', '1745678901', N'ANDRADE SALAZAR', N'LUIS FERNANDO', CAST('1985-11-30' AS DATE),
               'landrade@monstersalud.ec', '0965432109', N'Sangolqui, Pichincha', 'MSP-17-01177', 'A'),
              ('EMP0000005', 'F', 'S', 'CEX', 'MED', '1756789012', N'MORALES CEVALLOS', N'DIANA CAROLINA', CAST('1991-06-14' AS DATE),
               'dmorales@monstersalud.ec', '0954321098', N'Cumbaya, Quito', 'MSP-17-06612', 'A'),
              ('EMP0000006', 'M', 'C', 'HOS', 'ENF', '1767890123', N'CHAVEZ ORTIZ', N'PABLO ANDRES', CAST('1995-02-08' AS DATE),
               'pchavez@monstersalud.ec', '0943210987', N'Conocoto, Quito', 'MSP-17-09105', 'A')
            ) AS v(cod, gen, esc, dep, car, ced, ape, nom, fec, mail, tel, dir, reg, est)
            WHERE NOT EXISTS (SELECT 1 FROM PEEMP_EMPLE t WHERE t.PEEMP_CODIGO = v.cod)
            """;

        // ---------- Seguridad: perfiles, usuarios, sistemas, opciones y permisos ----------
        yield return """
            INSERT INTO XEPER_PERFI (XEPER_CODIGO, XEPER_DESCRI, XEPER_OBSERV)
            SELECT v.c, v.d, v.o FROM (VALUES
              ('ADM', N'ADMINISTRADOR DEL SISTEMA', N'Gestiona usuarios, perfiles, opciones y auditoría'),
              ('MED', N'MEDICO', N'Atiende consultas, emite recetas y ordena exámenes'),
              ('ENF', N'ENFERMERO', N'Registra signos vitales y apoya la atención'),
              ('FAR', N'FARMACEUTICO', N'Dispensa recetas y controla inventario')) AS v(c, d, o)
            WHERE NOT EXISTS (SELECT 1 FROM XEPER_PERFI t WHERE t.XEPER_CODIGO = v.c)
            """;
        yield return $"""
            INSERT INTO XEUSU_USUAR (XEUSU_CODIGO, PEEMP_CODIGO, XEEST_CODIGO, XEUSU_PASSWO, XEUSU_INTFAL, XEUSU_FECCRE, XEUSU_PIEFIR)
            SELECT v.u, v.e, 'A', '{HashMonster2026}', 0, GETDATE(), v.p FROM (VALUES
              ('admin', 'EMP0000001', N'Administrador del SIGH'),
              ('mtorres', 'EMP0000002', N'Dra. María José Torres - Medicina General'),
              ('aguerrero', 'EMP0000003', N'Lcda. Ana Guerrero - Enfermería'),
              ('landrade', 'EMP0000004', N'Q.F. Luis Andrade - Farmacia')) AS v(u, e, p)
            WHERE NOT EXISTS (SELECT 1 FROM XEUSU_USUAR t WHERE t.XEUSU_CODIGO = v.u)
            """;
        yield return """
            INSERT INTO XEUXP_USUPE (XEUSU_CODIGO, XEPER_CODIGO, XEUXP_FECASI)
            SELECT v.u, v.p, GETDATE() FROM (VALUES ('admin', 'ADM'), ('mtorres', 'MED'), ('aguerrero', 'ENF'),
                                                     ('landrade', 'FAR')) AS v(u, p)
            WHERE NOT EXISTS (SELECT 1 FROM XEUXP_USUPE t WHERE t.XEUSU_CODIGO = v.u AND t.XEPER_CODIGO = v.p)
            """;
        yield return """
            INSERT INTO XESIS_SISTE (XESIS_CODIGO, XESIS_DESCRI)
            SELECT v.c, v.d FROM (VALUES ('SEG', N'SEGURIDAD'), ('PER', N'PERSONAL')) AS v(c, d)
            WHERE NOT EXISTS (SELECT 1 FROM XESIS_SISTE t WHERE t.XESIS_CODIGO = v.c)
            """;
        yield return """
            INSERT INTO XEOPC_OPCIO (XEOPC_CODIGO, XESIS_CODIGO, XEOPC_DESCRI, XEOPC_URL)
            SELECT v.c, v.s, v.d, v.u FROM (VALUES
              ('S001', 'SEG', N'Administrar usuarios', '/seguridad/usuarios'),
              ('S002', 'SEG', N'Administrar perfiles', '/seguridad/perfiles'),
              ('S003', 'SEG', N'Consultar auditoría', '/seguridad/auditoria'),
              ('P001', 'PER', N'Administrar departamentos', '/personal/departamentos'),
              ('P002', 'PER', N'Administrar cargos', '/personal/cargos'),
              ('P003', 'PER', N'Mantenimiento de empleados', '/personal/empleados')) AS v(c, s, d, u)
            WHERE NOT EXISTS (SELECT 1 FROM XEOPC_OPCIO t WHERE t.XEOPC_CODIGO = v.c)
            """;
        yield return """
            INSERT INTO XEOXP_OPCPE (XEPER_CODIGO, XEOPC_CODIGO, XEOXP_FECASI)
            SELECT 'ADM', v.o, GETDATE() FROM (VALUES ('S001'), ('S002'), ('S003'), ('P001'), ('P002'), ('P003')) AS v(o)
            WHERE NOT EXISTS (SELECT 1 FROM XEOXP_OPCPE t WHERE t.XEPER_CODIGO = 'ADM' AND t.XEOPC_CODIGO = v.o)
            """;
    }
}
