/* SIGH - Mantenimiento de empleados (opción P003) */
const API = "/personal/empleados/api";
    let empleados = [];
    let catalogos = { generos: [], estadosCiviles: [], cargos: [], siguienteCodigo: "" };
    let seleccionado = null;

    const $ = function (id) { return document.getElementById(id); };

    document.addEventListener("DOMContentLoaded", async function () {
        try {
            await cargarCatalogos();
            await listar();
            limpiar();
        } catch (e) {
            SIGH.error(e.message);
        }
    });

    // Pestañas Inf personal / profesional / contacto
    document.querySelectorAll(".pestanas button").forEach(function (b) {
        b.addEventListener("click", function () { mostrarPestana(b.getAttribute("data-pestana")); });
    });
    function mostrarPestana(id) {
        document.querySelectorAll(".pestanas button").forEach(function (b) {
            b.classList.toggle("activa", b.getAttribute("data-pestana") === id);
        });
        document.querySelectorAll(".pestana").forEach(function (p) { p.classList.toggle("activa", p.id === id); });
    }

    // Solo dígitos en cédula y teléfono; mayúsculas en nombres
    ["fCedula", "fTelefono"].forEach(function (id) {
        $(id).addEventListener("input", function () { this.value = this.value.replace(/\D/g, ""); });
    });
    ["fApellidos", "fNombres", "fRegistro"].forEach(function (id) {
        $(id).addEventListener("input", function () { this.value = this.value.toUpperCase(); });
    });

    async function cargarCatalogos() {
        catalogos = await SIGH.api(API + "/catalogos");
        $("fGenero").innerHTML = opciones(catalogos.generos);
        $("fEstadoCivil").innerHTML = opciones(catalogos.estadosCiviles);
        // Cargos agrupados por departamento; el valor es DEPARTAMENTO|CARGO
        const grupos = {};
        catalogos.cargos.forEach(function (c) { (grupos[c.departamento] = grupos[c.departamento] || []).push(c); });
        $("fCargo").innerHTML = '<option value="">Seleccionar...</option>' + Object.keys(grupos).map(function (dep) {
            return '<optgroup label="' + SIGH.esc(dep) + '">' + grupos[dep].map(function (c) {
                return '<option value="' + SIGH.esc(c.codigoDepartamento + "|" + c.codigoCargo) + '">' + SIGH.esc(c.descripcion) + '</option>';
            }).join("") + '</optgroup>';
        }).join("");
    }

    function opciones(lista) {
        return '<option value="">Seleccionar...</option>' + lista.map(function (o) {
            return '<option value="' + SIGH.esc(o.codigo) + '">' + SIGH.esc(o.descripcion) + '</option>';
        }).join("");
    }

    // LISTAR
    async function listar() {
        empleados = await SIGH.api(API);
        pintar();
    }

    function filtrados() {
        const estado = $("filtroEstado").value;
        const texto = $("filtroTexto").value.trim().toLowerCase();
        return empleados.filter(function (e) {
            return (!estado || e.estado === estado) && (!texto || [e.codigoEmpleado, e.cedula, e.apellidos + " " + e.nombres]
                .some(function (v) { return (v || "").toLowerCase().includes(texto); }));
        });
    }

    function pintar() {
        const lista = filtrados();
        const activos = empleados.filter(function (e) { return e.estado === "A"; }).length;
        $("resumen").textContent = empleados.length + " empleados · " + activos + " activos · " + (empleados.length - activos) + " inactivos";
        if (!lista.length) {
            $("cuerpo").innerHTML = '<tr><td colspan="7" class="vacio">No hay empleados para mostrar.</td></tr>';
            return;
        }
        $("cuerpo").innerHTML = lista.map(function (e) {
            return '<tr data-codigo="' + SIGH.esc(e.codigoEmpleado) + '"' + (e.codigoEmpleado === seleccionado ? ' class="seleccionada"' : '') + '>'
                + '<td><span class="etiqueta etiqueta-azul">' + SIGH.esc(e.codigoEmpleado) + '</span></td>'
                + '<td>' + SIGH.esc(e.cedula) + '</td>'
                + '<td><strong>' + SIGH.esc(e.apellidos + " " + e.nombres) + '</strong></td>'
                + '<td>' + SIGH.esc(e.cargo) + '<small class="d-block text-suave">' + SIGH.esc(e.departamento) + '</small></td>'
                + '<td>' + SIGH.esc(e.telefono || "—") + '</td>'
                + '<td>' + SIGH.esc(e.email) + '</td>'
                + '<td class="text-center">' + (e.estado === "A"
                    ? '<span class="etiqueta etiqueta-verde">Activo</span>'
                    : '<span class="etiqueta etiqueta-gris">Inactivo</span>') + '</td>'
                + '</tr>';
        }).join("");
    }

    $("filtroEstado").addEventListener("change", pintar);
    $("filtroTexto").addEventListener("input", pintar);
    $("cuerpo").addEventListener("click", function (evento) {
        const fila = evento.target.closest("tr[data-codigo]");
        if (fila) { seleccionar(fila.getAttribute("data-codigo")); }
    });

    function seleccionar(codigo) {
        const e = empleados.find(function (x) { return x.codigoEmpleado === codigo; });
        if (!e) { return; }
        seleccionado = codigo;
        $("fCodigo").value = e.codigoEmpleado;
        $("fCedula").value = e.cedula;
        $("fApellidos").value = e.apellidos;
        $("fNombres").value = e.nombres;
        $("fFecha").value = e.fechaNacimiento || "";
        $("fGenero").value = e.codigoGenero || "";
        $("fEstadoCivil").value = e.codigoEstadoCivil || "";
        $("fRegistro").value = e.registroProfesional || "";
        $("fEstado").value = e.estado;
        $("fCargo").value = e.codigoDepartamento + "|" + e.codigoCargo;
        $("fDireccion").value = e.direccion || "";
        $("fTelefono").value = e.telefono || "";
        $("fEmail").value = e.email;
        modo();
        pintar();
        window.scrollTo({ top: 0, behavior: "smooth" });
    }

    // CANCELAR: deja el formulario listo para un registro nuevo
    function limpiar() {
        seleccionado = null;
        ["fCedula", "fApellidos", "fNombres", "fFecha", "fGenero", "fEstadoCivil", "fRegistro", "fCargo",
            "fDireccion", "fTelefono", "fEmail"].forEach(function (id) { $(id).value = ""; });
        $("fCodigo").value = catalogos.siguienteCodigo || "";
        $("fEstado").value = "A";
        mostrarPestana("tabPersonal");
        modo();
        pintar();
    }

    function modo() {
        const editando = !!seleccionado;
        $("tituloFicha").textContent = editando ? "Empleado " + seleccionado : "Nuevo empleado";
        $("modo").textContent = editando ? "Editando registro existente" : "Registro nuevo";
        $("modo").className = "etiqueta modo " + (editando ? "etiqueta-ambar" : "etiqueta-verde");
        $("btnGuardar").disabled = editando;
        $("btnActualizar").disabled = !editando;
        const e = empleados.find(function (x) { return x.codigoEmpleado === seleccionado; });
        $("btnBorrar").disabled = !editando || (e && e.estado === "I");
        $("fEstado").disabled = !editando; // un registro nuevo siempre nace Activo
    }

    function datos() {
        const cargo = $("fCargo").value.split("|");
        return {
            cedula: $("fCedula").value.trim(),
            apellidos: $("fApellidos").value.trim(),
            nombres: $("fNombres").value.trim(),
            fechaNacimiento: $("fFecha").value,
            codigoGenero: $("fGenero").value,
            codigoEstadoCivil: $("fEstadoCivil").value,
            registroProfesional: $("fRegistro").value.trim(),
            estado: $("fEstado").value,
            codigoDepartamento: cargo[0] || "",
            codigoCargo: cargo[1] || "",
            direccion: $("fDireccion").value.trim(),
            telefono: $("fTelefono").value.trim(),
            email: $("fEmail").value.trim()
        };
    }

    /** Validación rápida en pantalla; el servidor vuelve a validar todo. */
    function validar(d) {
        const faltan = [];
        if (!/^\d{10}$/.test(d.cedula)) { faltan.push("Cédula (10 dígitos)"); }
        if (d.apellidos.length < 2) { faltan.push("Apellidos"); }
        if (d.nombres.length < 2) { faltan.push("Nombres"); }
        if (!d.fechaNacimiento) { faltan.push("Fecha de nacimiento (Inf personal)"); }
        if (!d.codigoGenero) { faltan.push("Género (Inf personal)"); }
        if (!d.codigoEstadoCivil) { faltan.push("Estado civil (Inf personal)"); }
        if (!d.codigoCargo) { faltan.push("Cargo (Inf profesional)"); }
        if (!d.email) { faltan.push("Email (Inf contacto)"); }
        if (faltan.length) {
            SIGH.error("Complete los campos: " + faltan.join(", ") + ".");
            return false;
        }
        return true;
    }

    // GUARDAR (registro nuevo)
    $("btnGuardar").addEventListener("click", async function () {
        const d = datos();
        if (!validar(d)) { return; }
        try {
            const nuevo = await SIGH.api(API, { method: "POST", body: d });
            catalogos = await SIGH.api(API + "/catalogos");
            await listar();
            limpiar();
            SIGH.exito("Empleado registrado", "Se registró a " + nuevo.apellidos + " " + nuevo.nombres + " con el código " + nuevo.codigoEmpleado + ".");
        } catch (e) {
            SIGH.error(e.message);
        }
    });

    // ACTUALIZAR (registro seleccionado)
    $("btnActualizar").addEventListener("click", async function () {
        if (!seleccionado) { return; }
        const d = datos();
        if (!validar(d)) { return; }
        try {
            await SIGH.api(API + "/" + encodeURIComponent(seleccionado), { method: "PUT", body: d });
            const codigo = seleccionado;
            await listar();
            seleccionar(codigo);
            SIGH.exito("Empleado actualizado", "Los datos de " + codigo + " se guardaron correctamente.");
        } catch (e) {
            SIGH.error(e.message);
        }
    });

    // BORRAR (lógico: pasa a Inactivo)
    $("btnBorrar").addEventListener("click", async function () {
        const e = empleados.find(function (x) { return x.codigoEmpleado === seleccionado; });
        if (!e) { return; }
        const r = await Swal.fire({
            icon: "warning",
            title: "¿Borrar al empleado " + e.codigoEmpleado + "?",
            text: e.apellidos + " " + e.nombres + " pasará a estado Inactivo. El registro no se elimina y se puede reactivar con «Actualizar».",
            showCancelButton: true, confirmButtonText: "Borrar", cancelButtonText: "Cancelar", confirmButtonColor: "#b42318"
        });
        if (!r.isConfirmed) { return; }
        try {
            await SIGH.api(API + "/" + encodeURIComponent(e.codigoEmpleado), { method: "DELETE" });
            await listar();
            limpiar();
            SIGH.exito("Empleado desactivado", e.codigoEmpleado + " quedó Inactivo. Puede verlo con el filtro «Inactivos».");
        } catch (err) {
            SIGH.error(err.message);
        }
    });

    $("btnCancelar").addEventListener("click", limpiar);

    // IMPRIMIR: reporte de los empleados que muestra la tabla (con el filtro actual)
    $("btnImprimir").addEventListener("click", function () {
        const lista = filtrados();
        const filtro = $("filtroEstado").options[$("filtroEstado").selectedIndex].text;
        const filas = lista.map(function (e, i) {
            return '<tr><td>' + (i + 1) + '</td><td>' + SIGH.esc(e.codigoEmpleado) + '</td><td>' + SIGH.esc(e.cedula) + '</td><td>'
                + SIGH.esc(e.apellidos + " " + e.nombres) + '</td><td>' + SIGH.esc(e.departamento) + '</td><td>' + SIGH.esc(e.cargo)
                + '</td><td>' + SIGH.esc(e.telefono || "") + '</td><td>' + SIGH.esc(e.email) + '</td><td>'
                + (e.estado === "A" ? "Activo" : "Inactivo") + '</td></tr>';
        }).join("");
        const v = window.open("", "_blank");
        if (!v) { SIGH.error("El navegador bloqueó la ventana de impresión. Permita las ventanas emergentes."); return; }
        v.document.write('<!DOCTYPE html><html lang="es"><head><meta charset="UTF-8"><title>Reporte de empleados</title>'
            + '<style>body{font-family:Segoe UI,Arial,sans-serif;color:#1f2937;margin:24px}h1{font-size:20px;margin:0}'
            + 'p{margin:4px 0 16px;color:#555;font-size:13px}table{border-collapse:collapse;width:100%;font-size:12px}'
            + 'th,td{border:1px solid #cbd5e1;padding:6px;text-align:left}th{background:#e8f1fb}</style></head><body>'
            + '<h1>Hospital General Monster Salud · Reporte de empleados</h1>'
            + '<p>Filtro: ' + SIGH.esc(filtro) + ' · ' + lista.length + ' registro(s) · Generado: ' + new Date().toLocaleString("es-EC") + '</p>'
            + '<table><thead><tr><th>#</th><th>Código</th><th>Cédula</th><th>Apellidos y nombres</th><th>Departamento</th>'
            + '<th>Cargo</th><th>Teléfono</th><th>Email</th><th>Estado</th></tr></thead><tbody>'
            + (filas || '<tr><td colspan="9">Sin registros</td></tr>') + '</tbody></table></body></html>');
        v.document.close();
        v.focus();
        v.print();
    });
