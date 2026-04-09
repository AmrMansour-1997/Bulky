var DataTable;

$(document).ready(function () {
    loaddatatable();
});

function loaddatatable() {
    DataTable = $('#tblData').DataTable({
        "ajax": { "url": "/Admin/User/Getall" },

        "columns":
            [
                { data: 'name', "Width": "25%" },
                { data: 'email', "Width": "15%" },
                { data: 'phoneNumber', "Width": "10%" },
                { data: 'company.name', "Width": "15%" },
                { data: 'role', "Width": "10%" },
                {
                    data: { id: "id", lockoutEnd: "lockoutEnd" },

                    "render": function (data) {
                        var today = new Date().getTime();
                        var Lockout = new Date(data.lockoutEnd).getTime();

                        if (today < Lockout)
                        {
                            return `
                            <div class = "text-center">
                        <a OnClick="LockUnlock('${data.id}')" class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                        <i class="bi bi-unlock-fill"></i>Unlock</a>
                        <a href="/admin/user/RoleManagement?UserID=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                        <i></i>Permission</a>
                        </div>`
                        }
                        else
                        {
                            return `
                            <div class = "text-center">
                        <a OnClick="LockUnlock('${data.id}')" class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                        <i class="bi bi-lock-fill"></i>Lock</a>
                        <a href="/admin/user/RoleManagement?UserID=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                        <i></i>Permission</a>
                        </div>`
                        }

                    },

                    "Width": "25%"
                }
            ]
    }
    );
}

function LockUnlock(id) {

    $.ajax({
        type: 'POST',
        url: '/Admin/User/LockUnlock',
        data: {id : id},
        success: function (data) {
            DataTable.ajax.reload();
            toastr.success(data.message);

        }
    })
}
