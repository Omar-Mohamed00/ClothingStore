$(document).ready(function () {
    $('#categoryTable').DataTable({
        paging: true,          // Enable pagination
        searching: true,       // Enable search functionality
        ordering: true,        // Enable sorting
        info: true,            // Display table info
        lengthChange: true     // Allow changing the number of items per page
    });
});


$(document).ready(function () {
    $('.js-delete').on('click', function () {
        var btn = $(this);

        const swal = Swal.mixin({
            customClass: {
                confirmButton: 'btn btn-danger mx-2',
                cancelButton: 'btn btn-light'
            },
            buttonsStyling: false
        });

        swal.fire({
            title: 'Are you sure that you need to delete Category ?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/admin/category/delete/${btn.data('id')}`,
                    method: 'DELETE',
                    success: function () {
                        swal.fire(
                            'Deleted!',
                            'Category has been deleted.',
                            'success'
                        );

                        btn.parents('tr').fadeOut();
                    },
                    error: function () {
                        swal.fire(
                            'Oooops...',
                            'Something went wrong.',
                            'error'
                        );
                    }
                });
            }
        });
    });
});