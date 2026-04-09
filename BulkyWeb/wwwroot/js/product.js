var DataTable;

$(document).ready(function () {
    loaddatatable();
});

function loaddatatable() {
    DataTable = $('#tblData').DataTable({
        "ajax": { "url": "/Admin/Product/Getall" },

        "columns":
            [
                { data: 'title', "Width": "25%" },
                { data: 'isbn', "Width": "15%" },
                { data: 'listPrice', "Width": "10%" },
                { data: 'author', "Width": "15%" },
                { data: 'category.name', "Width": "10%" },
                {
                    data: 'id',

                    "render": function (data) {
                        return `<div class = "w-75 btn-group" role="group">
                        <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>Edit</a>
                        <a OnClick=Delete("/admin/product/delete?id=${data}") class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i>Delete</a>
                        </div>`
                    },

                    "Width": "25%"
                }
            ]
    }
    );
}


function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    DataTable.ajax.reload();
                    toastr.success(data.message);
                   
                }
            })
        }
    });
}