﻿

let datatable;

$(document).ready(function () {
    loadDataTable();
})



function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros Por Pagina",
            "zeroRecords": "Ningun Registro",
            "info": "Mostrar pagina _PAGE_ de _PAGES_",
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
            "url": "/Admin/Marca/ObtenerTodos"
        },
        "columns": [
            { "data": "nombre", "width": "20%" },
            { "data": "descripcion", "width": "40%" },
            {
                "data": "estado",
                "render": function (data) {
                    if (data == true) { return "Activo"; } else { return "Inactivo"; }
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (datas) {
                    return `
                    <div class="text-center">
                    <a href="/Admin/Marca/Upsert/${datas}" class="btn btn-success text-white" style="cursor:pointer">
                    <i class="bi bi-pencil-square"></i>
                    </a>
                    <a onclick=Delete("/Admin/Marca/Delete/${datas}") class="btn btn-danger text-white" style"cursor:pointer">
                    <i class="bi bi-trash3"></i>
                    </a>
                    </div>
                    `;
                }, "width": "20%"
            }
        ]

    });
}
function Delete(url) {
    swal({
        title: "Estas seguro de eliminar la Marca?",
        text: "Este registro no se podra recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    });
}