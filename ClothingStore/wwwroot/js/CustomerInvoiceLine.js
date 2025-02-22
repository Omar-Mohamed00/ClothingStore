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
            title: 'Are you sure that you need to delete Customer Invoice ?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/admin/customerInvoiceLine/delete/${btn.data('id')}`,
                    method: 'DELETE',
                    success: function () {
                        swal.fire(
                            'Deleted!',
                            'Customer Invoice has been deleted.',
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
$(document).ready(function () {
    // Example: Automatically setting CustomerInvoiceId based on some logic
    $.ajax({
        url: '/Admin/CustomerInvoice/GetLatestInvoiceId', // Replace with your API endpoint
        type: 'GET',
        success: function (response) {
            if (response && response.id) {
                $('#customerInvoiceId').val(response.id);
            }
        },
        error: function () {
            console.log("Failed to fetch invoice ID");
        }
    });
});
$(document).ready(function () {
    $('#upsertForm').submit(function (e) {
        e.preventDefault(); // Prevent default form submission
        $.ajax({
            url: $(this).attr('action'),
            method: $(this).attr('method'),
            data: $(this).serialize(), // Serialize form data
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#addCustomerInvoiceLine').modal('hide');
                    // Reload or update data table here
                } else {
                    $('#addCustomerInvoiceLine .modal-content').html(response);
                }
            },
            error: function (error) {
                console.error("An error occurred:", error);
            }
        });
    });
});
