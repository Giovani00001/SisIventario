
var datatable;

$(document).ready(function () {
    var url = window.location.search;///obtiene la ruta de la vista actual
    if (url.includes("aprobado")) {
        loadDataTable("ObtenerListta?estado=aprobado");
    }
    else {
        if (url.includes("completado")) {
            loadDataTable("ObtenerListta?estado=completado");
        }
        else {
            loadDataTable("ObtenerListta?estado=todas");
        }
    }

});

function loadDataTable(url) {
    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros Por Pagina",
            "zeroRecords": "Ningun Registro",
            "info": "Mostrar page _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "ajax": {
            "url": "/Admin/Orden/" + url
        },
        "columns": [
            { "data": "id" },
            { "data": "nombreCliente" },
            { "data": "telefono" },
            { "data": "usuarioAplicacion.email" },
            { "data": "estadoOrden" },
            {
                "data": "totalOrden", "className": "text-end",
                "render": function (data) {
                    var d = data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                    return d;
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Orden/Detalle/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-ticket-detailed"></i>
                            </a>                           
                        </div>
                        `;
                }
            }
        ]
    });
}

